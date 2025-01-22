const inputField = document.getElementById('inputField');
const outputField = document.getElementById('outputField');

async function processMarkdownText() {
    try {
        const inputText = inputField.value;
        const response = await fetch("/markdown-to-html-convert", {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ InputText: inputText })
        });

        const result = await response.json();
        if (response.ok) {
            outputField.textContent = result.HtmlText;
        } else {
            console.log('Ошибка.');
        }
    }
    catch {
        console.error(error);
    }
}
