document.addEventListener('DOMContentLoaded', () => {
  fileInputinit()
  fileUploadinit()
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

async function getSolution() {
  const button = document.querySelector('#save_button')
  if (!button) return
  button.disabled = true

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

  const formData = new FormData()
  formData.append('file', fileInput.files[0])
  formData.append('multistartCount', multistartCount)

  try {
    const response = await fetch('/Home/Get', {
      method: 'POST',
      body: formData,
    })

    if (!response.ok) {
      throw new Error(`HTTP error: ${response.status}`)
    }

    const data = await response.json()

    console.log(data)

    drawSolutionInfo(data)
    drawBestSolution(data)
    saveBestSolution(data)
    drawGistogram(data)
    drawZbig(data)
  } catch (error) {
    console.error('Помилка:', error)
  }
}

function drawSolutionInfo(results) {
  const best = Math.min(...results.map((x) => x.fValue))
  const worst = Math.max(...results.map((x) => x.fValue))
  const difference = worst - best

  document.querySelector('#best-value').textContent = best.toFixed(2)
  document.querySelector('#worst-value').textContent = worst.toFixed(2)
  document.querySelector('#difference-value').textContent = difference.toFixed(2)
}

let solutionChart = null

function drawBestSolution(results) {
  const best = results.reduce((best, current) => (current.fValue < best.fValue ? current : best))

  const datasets = []

  best.solution.routes.forEach((route, index) => {
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
          text: `Найкраще рішення (FValue = ${best.fValue.toFixed(2)})`,
        },

        legend: {
          display: true,
        },
      },
    },
  })
}

function saveBestSolution(results) {
  const button = document.querySelector('#save_button')
  if (!button) return
  button.disabled = false

  button.onclick = () => {
    const best = results.reduce((best, current) => (current.fValue < best.fValue ? current : best))

    const json = JSON.stringify(best, null, 2)

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
  const values = results.map((x) => x.fValue)

  const n = values.length

  if (n === 0) {
    return
  }

  const classCount = Math.ceil(1 + 3.32 * Math.log10(n))

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

function drawZbig(results) {}
