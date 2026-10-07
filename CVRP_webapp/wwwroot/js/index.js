document.addEventListener('DOMContentLoaded', () => {
  fileInputinit()
  fileUploadinit()
  getJobinit()
  clearButtoninit()
  copyButtoninit()
})

function fileInputinit() {
  const file = document.querySelector('#file_upload')
  if (!file) return
  file.addEventListener('change', () => {
    const button = document.querySelector('#upload_button')
    button.disabled = file.files.length === 0
  })
}

function fileUploadinit() {
  const button = document.querySelector('#upload_button')
  if (!button) return
  button.addEventListener('click', getSolution)
}

function getJobinit() {
  const button = document.querySelector('#check_button')
  if (!button) return
  button.addEventListener('click', getStatus)
}

function clearButtoninit() {
  const button = document.querySelector('#clear_button')
  if (!button) return
  button.addEventListener('click', () => {
    document.querySelector('#job-id').value = ''
  })
}

function copyButtoninit() {
  const button = document.querySelector('#copy_button')
  if (!button) return
  button.addEventListener('click', async () => {
    const input = document.querySelector('#job-id')

    if (!input.value) return

    await navigator.clipboard.writeText(input.value)
  })
}

async function getSolution() {
  const fileInput = document.querySelector('#file_upload')
  if (!fileInput) return
  if (!fileInput.files || fileInput.files.length === 0) {
    console.error('Обери файл')
    return
  }

  const multistartInput = document.querySelector('#multistart_upload')
  if (!multistartInput) return
  const multistartCount = Number(multistartInput.value)

  if (!Number.isInteger(multistartCount) || multistartCount <= 0) {
    console.error('Введи коректну кількість стартів')
    return
  }

  const historyInput = document.querySelector('#history_upload')
  if (!historyInput) return
  const historyCount = Number(historyInput.value)

  if (!Number.isInteger(historyCount) || historyCount <= 0) {
    console.error('Введи коректну кількість історій')
    return
  }

  const solverInput = document.querySelector('#solver_upload')
  if (!solverInput) return
  const solver = solverInput.value

  const formData = new FormData()
  formData.append('file', fileInput.files[0])
  formData.append('multistartCount', multistartCount)
  formData.append('historyCount', historyCount)
  formData.append('solver', solver)

  try {
    const response = await fetch('/Home/Start', {
      method: 'POST',
      body: formData,
    })

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`)
    }

    const data = await response.json()
    console.log(data)

    const jobIdInput = document.querySelector('#job-id')
    if (!jobIdInput) return
    jobIdInput.value = data.jobId
  } catch (error) {
    console.error('Помилка:', error)
  }
}

async function getStatus() {
  const jobIdInput = document.querySelector('#job-id')
  if (!jobIdInput) return

  const id = jobIdInput.value

  if (!id) {
    return
  }

  try {
    const response = await fetch(`/Home/Status?guid=${encodeURIComponent(id)}`)

    const statusInfo = document.querySelector('#status-value')

    if (response.status === 404) {
      statusInfo.textContent = 'Task Not Found'
      return
    }

    const data = await response.json()

    if (data.jobStatus === 0) {
      statusInfo.textContent = 'Task is Waiting'
    } else if (data.jobStatus === 1) {
      statusInfo.textContent = 'Task is Running'
    } else if (data.jobStatus === 2) {
      statusInfo.textContent = 'Task Completed'

      drawSolutionInfo(data.result)
      drawBestSolution(data.result)
      saveResults(data)
      drawGistogram(data.result)
      drawConvergence(data.result)
    } else if (data.jobStatus === 3) {
      statusInfo.textContent = `Task Failed. Error = ${data.error}`
    }
  } catch (error) {
    console.error('Помилка:', error)
  }
}

function drawSolutionInfo(results) {
  const best = results.best.fValue
  const worst = results.worst.fValue
  const difference = worst - best

  document.querySelector('#time-value').textContent = results.time
  document.querySelector('#best-value').textContent = best.toFixed(2)
  document.querySelector('#worst-value').textContent = worst.toFixed(2)
  document.querySelector('#difference-value').textContent = difference.toFixed(2)
}

let solutionChart = null
function drawBestSolution(results) {
  const datasets = []

  results.best.solution.routes.forEach((route, index) => {
    const points = route.map((application) => ({
      x: application.point.x,
      y: application.point.y,
    }))

    datasets.push({
      label: `Маршрут ${index + 1}`,
      data: points,
      showLine: true,
      fill: false,
      tension: 0,
      pointRadius: 5,
      pointHoverRadius: 7,
    })
  })

  if (solutionChart) {
    solutionChart.destroy()
  }

  const ctx = document.querySelector('#solution-chart')

  solutionChart = new Chart(ctx, {
    type: 'scatter',
    data: {
      datasets: datasets,
    },
    options: {
      responsive: true,

      scales: {
        x: {
          title: {
            display: true,
            text: 'X',
          },
        },
        y: {
          title: {
            display: true,
            text: 'Y',
          },
        },
      },

      plugins: {
        title: {
          display: true,
          text: `Найкраще рішення (FValue = ${results.best.fValue.toFixed(2)})`,
        },

        legend: {
          display: true,
        },
      },
    },
  })
}

function saveResults(results) {
  const button = document.querySelector('#save_button')
  if (!button) return
  button.disabled = false

  button.onclick = () => {
    const json = JSON.stringify(
      {
        inputData: results.inputData,
        parameters: results.parameters,
        result: results.result,
      },
      null,
      2,
    )

    const blob = new Blob([json], {
      type: 'application/json',
    })

    const url = URL.createObjectURL(blob)

    const link = document.createElement('a')
    link.href = url

    const now = new Date()
    const timestamp =
      `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}_` +
      `${String(now.getHours()).padStart(2, '0')}-${String(now.getMinutes()).padStart(2, '0')}-${String(now.getSeconds()).padStart(2, '0')}`

    link.download = `best-solution-${timestamp}.json`

    link.click()

    URL.revokeObjectURL(url)
  }
}

let qualityChart = null
function drawGistogram(results) {
  const values = results.solutions.map((x) => x.fValue)

  const n = values.length

  if (n === 0) {
    return
  }

  let classCount = Math.floor(1 + 3.32 * Math.log10(n))
  if (classCount < 5) classCount = 5
  if (classCount % 2 == 0) classCount++

  const minValue = Math.min(...values)
  const maxValue = Math.max(...values)

  const classWidth = (maxValue - minValue) / classCount

  const frequencies = new Array(classCount).fill(0)

  for (const value of values) {
    let classIndex = Math.floor((value - minValue) / classWidth)

    if (classIndex >= classCount) {
      classIndex = classCount - 1
    }

    frequencies[classIndex]++
  }

  const labels = []

  for (let i = 0; i < classCount; i++) {
    const left = minValue + i * classWidth
    const right = left + classWidth
    const center = (left + right) / 2

    labels.push(center.toFixed(2))
  }

  if (qualityChart) {
    qualityChart.destroy()
  }

  const ctx = document.querySelector('#quality-chart')

  qualityChart = new Chart(ctx, {
    type: 'bar',

    data: {
      labels: labels,

      datasets: [
        {
          label: 'К-сть рішень',
          data: frequencies,
          barPercentage: 1,
          categoryPercentage: 1,
        },
      ],
    },

    options: {
      responsive: true,

      scales: {
        x: {
          title: {
            display: true,
            text: 'Якість рішення',
          },
        },

        y: {
          title: {
            display: true,
            text: 'К-сть рішень',
          },

          beginAtZero: true,
        },
      },

      plugins: {
        title: {
          display: true,
          text: 'Гістограма',
        },
      },
    },
  })
}

let convergenceChart = null
function drawConvergence(results) {
  const datasets = results.histories.map((values, index) => ({
    label: `Запуск ${index + 1}`,

    data: values.map((value, iteration) => ({
      x: iteration,
      y: value,
    })),

    showLine: true,
    fill: false,
    tension: 0,
  }))

  if (convergenceChart) {
    convergenceChart.destroy()
  }

  const ctx = document.querySelector('#convergence-chart')

  convergenceChart = new Chart(ctx, {
    type: 'line',

    data: {
      datasets: datasets,
    },

    options: {
      responsive: true,

      scales: {
        x: {
          type: 'linear',

          title: {
            display: true,
            text: 'Ітерація',
          },
        },

        y: {
          title: {
            display: true,
            text: 'Значення функції якості',
          },
        },
      },

      plugins: {
        title: {
          display: true,
          text: 'Графік збіжності',
        },

        legend: {
          display: true,
        },
      },
    },
  })
}
