const inputField = document.getElementById('inputField');
const outputField = document.getElementById('outputField');

let requestTimer;

inputField.addEventListener("input", async () => {
    clearTimeout(requestTimer);
    requestTimer = setTimeout(async () => {
        const inputText = inputField.value;
        if (!inputText) {
            outputField.textContent = "";
        }
        else {
            await processMarkdownText(inputText);
        }
    }, 250);
});

async function processMarkdownText(inputText) {
    try {
        const response = await fetch("/markdown-to-html-convert", {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ InputText: inputText })
        });

        const result = await response.json();
        if (response.ok) {
            outputField.innerHTML = result.HtmlText;
        } else {
            console.log('Ошибка.');
        }
    }
    catch {
        console.error(error);
    }
}
