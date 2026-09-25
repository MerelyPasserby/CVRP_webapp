document.addEventListener("DOMContentLoaded", () => {
    fileInputinit()
    fileUploadinit()
})

function fileInputinit() {
    const file = document.querySelector("#file_upload");

    if (!file) return;

    file.addEventListener("change", () => {
        const button = document.querySelector("#upload_button");
        button.disabled = file.files.length === 0;
    })
}

function fileUploadinit() {
    const button = document.querySelector("#upload_button");
    if (!button) return;
    button.addEventListener("click", getSolution)
}

async function getSolution() {
    const fileInput = document.querySelector("#file_upload");
    if (!fileInput) return;

    if (!fileInput.files || fileInput.files.length === 0) {
        console.error("Обери файл");
        return;
    }

    const multistartInput = document.querySelector("#multistart_upload");
    if (!multistartInput) return;

    const multistartCount = Number(multistartInput.value);

    if (!Number.isInteger(multistartCount) || multistartCount <= 0) {
        console.error("Введи коректну кількість стартів");
        return;
    }

    const formData = new FormData();

    formData.append("file", fileInput.files[0]);
    formData.append("multistartCount", multistartCount);

    try {
        const response = await fetch("/Home/Get", {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            throw new Error(`HTTP error: ${response.status}`);
        }

        const data = await response.json();

        console.log(data);

        drawSolutionInfo(data);
        drawBestSolution(data);
        saveBestSolution(data);
        drawGistogram(data);
        drawZbig(data);
    }
    catch (error) {
        console.error("Помилка:", error);
    }
}

function drawSolutionInfo(results) {
    
}

function drawBestSolution(results){

}

function saveBestSolution(results){

}

function drawGistogram(results){

}

function drawZbig(results){

}