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

  const file1 = document.querySelector('#file_upload2')
  if (!file1) return
  file1.addEventListener('change', () => {
    const button = document.querySelector('#upload_button2')
    button.disabled = file.files.length === 0
  })
}

function fileUploadinit() {
  const button = document.querySelector('#upload_button')
  if (!button) return
  button.addEventListener('click', () => {
      getSolution('')
  })

  const button1 = document.querySelector('#upload_button2')
  if (!button1) return
  button1.addEventListener('click', () => {
      getSolution('2')
  })
}

async function getSolution(value) {
  const fileInput = document.querySelector(`#file_upload${value}`)
  if (!fileInput) return
  if (!fileInput.files || fileInput.files.length === 0) {
    console.error('Обери файл')
    return
  }

  try {
    const file = fileInput.files[0]
    const text = await file.text()
    const data = JSON.parse(text)

    drawSolutionInfo(value, data.result)
    drawBestSolution(value, data.result)
    drawGistogram(value, data.result)
    drawConvergence(value, data.result)
  } catch (error) {
    console.error('Помилка:', error)
  }
}

function drawSolutionInfo(value, results) {
  const best = results.best.fValue
  const worst = results.worst.fValue
  const difference = worst - best

  document.querySelector(`#time-value${value}`).textContent = results.time
  document.querySelector(`#best-value${value}`).textContent = best.toFixed(2)
  document.querySelector(`#worst-value${value}`).textContent = worst.toFixed(2)
  document.querySelector(`#difference-value${value}`).textContent = difference.toFixed(2)
}

const solutionCharts = {}
function drawBestSolution(value, results) {
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

  // if (solutionChart) {
  //   solutionChart.destroy()
  // }

  const ctx = document.querySelector(`#solution-chart${value}`)

  if(solutionCharts[`#solution-chart${value}`]){
      solutionCharts[`#solution-chart${value}`].destroy()
  }

  solutionCharts[`#solution-chart${value}`] = new Chart(ctx, {
    type: 'scatter',
    data: {
      datasets: datasets,
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
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

const qualityCharts = {}
function drawGistogram(value, results) {
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

  // if (qualityChart) {
  //   qualityChart.destroy()
  // }

  const ctx = document.querySelector(`#quality-chart${value}`)

  if(qualityCharts[`#quality-chart${value}`]){
      qualityCharts[`#quality-chart${value}`].destroy()
  }

  qualityCharts[`#quality-chart${value}`] = new Chart(ctx, {
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
      maintainAspectRatio: false,
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

const convergenceCharts = {}
function drawConvergence(value, results) {
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

  // if (convergenceChart) {
  //   convergenceChart.destroy()
  // }

  const ctx = document.querySelector(`#convergence-chart${value}`)
  
  if(convergenceCharts[`#convergence-chart${value}`]){
     convergenceCharts[`#convergence-chart${value}`].destroy()
  } 
  
  convergenceCharts[`#convergence-chart${value}`] = new Chart(ctx, {
    type: 'line',

    data: {
      datasets: datasets,
    },

    options: {
      responsive: true,
      maintainAspectRatio: false,
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
