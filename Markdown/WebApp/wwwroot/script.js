const inputField = document.getElementById('inputField');
const outputField = document.getElementById('outputField');

const inputContainer = document.getElementById('inputContainer');
const outputContainer = document.getElementById('outputContainer');
const authContainer = document.getElementById('authContainer');

const copyHtmlButton = document.getElementById('copyHtmlButton');
const downloadHtmlButton = document.getElementById('downloadHtmlButton');
const messageField = document.getElementById('messageField');
const fullscreenInputButton = document.getElementById('fullscreenInputButton');
const fullscreenOutputButton = document.getElementById('fullscreenOutputButton');
const openFullscreenButton = document.getElementById('openFullscreenButton');

const openProfileButton = document.getElementById('profileBtn');
const loginForm = document.getElementById('loginForm');
const registerForm = document.getElementById('registerForm');
const errorAuthMessage = document.getElementById('errorAuthMessage');

let requestTimer;

// при перезагрузке страницы
setHtmlTextToOutputField();

inputField.addEventListener("input", async () => {
    await setHtmlTextToOutputField();
});

// копирование html кода
copyHtmlButton.addEventListener('click', () => {
    copyHtmlButton.disabled = true;
    setTimeout(() => {
        copyHtmlButton.disabled = false;
    }, 5000);
    copyHtmlContent();
});

// загрузка html файла
downloadHtmlButton.addEventListener('click', () => {
    const htmlContent = outputField.innerHTML;

    try {
        const blob = new Blob([htmlContent], { type: 'text/html' }); // массив байт
        const url = URL.createObjectURL(blob);

        const a = document.createElement('a');
        a.href = url;
        a.download = 'htmlContent.html';
        a.click();

        URL.revokeObjectURL(url); // удаляем объект URL

        changeTextTemporarily(messageField, 'Успешно загружено', 'green', 5000);
        console.log('Html страница успешно скачена!');
    } catch (err) {
        changeTextTemporarily(messageField, 'Ошибка при скачивании', 'red', 5000);
        console.error('Ошибка при скачивании: ', err);
    }
});

fullscreenInputButton.addEventListener('click', () => {
    if (inputContainer.classList.contains('fullscreen')) {
        inputContainer.classList.remove('fullscreen');
    } else {
        inputContainer.classList.add('fullscreen');
    }
});

fullscreenOutputButton.addEventListener('click', () => {
    if (outputContainer.classList.contains('fullscreen')) {
        outputContainer.classList.remove('fullscreen');
    } else {
        outputContainer.classList.add('fullscreen');
    }
});

openFullscreenButton.addEventListener('click', () => {
    if (!document.fullscreenElement) {
        document.documentElement.requestFullscreen();
        openFullscreenButton.textContent = 'Выйти из полноэкранного режима'
    } else {
        document.exitFullscreen();
        openFullscreenButton.textContent = 'Полноэкранный режим'
    }
});

openProfileButton.addEventListener('click', () => {
    authContainer.style.display = "block";
});


loginForm.addEventListener('submit', async (event) => {
    event.preventDefault(); // предотвращает стандартное поведение формы
    await login();
});

registerForm.addEventListener('submit', async (event) => {
    event.preventDefault();
    await register();
});

function showForm(formType) {
    const buttons = document.querySelectorAll('.tabButton');

    if (errorAuthMessage) errorAuthMessage.textContent = '';

    if (formType === 'login') {
        loginForm.classList.add('active');
        registerForm.classList.remove('active');
        buttons[0].classList.add('active');
        buttons[1].classList.remove('active');
    } else if (formType === 'register') {
        registerForm.classList.add('active');
        loginForm.classList.remove('active');
        buttons[1].classList.add('active');
        buttons[0].classList.remove('active');
    }
}

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

function changeTextTemporarily(element, newText, color, duration) {
    const originalText = element.textContent;
    const originalColor = element.style.color;

    element.textContent = newText;
    element.style.color = color;

    setTimeout(() => {
        element.textContent = originalText;
        element.style.color = originalColor;
    }, duration);
}

async function setHtmlTextToOutputField() {
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
}
function copyHtmlContent() {
    const htmlContent = outputField.innerHTML;
    htmlContent
    if (htmlContent) {
        try {
            const textarea = document.createElement('textarea');
            textarea.value = htmlContent;
            textarea.setAttribute('readonly', '');
            textarea.style.position = 'absolute';
            textarea.style.left = '-9999px'

            document.body.appendChild(textarea);
            textarea.select();

            const isCopied = document.execCommand('copy');
            if (isCopied) {
                changeTextTemporarily(messageField, 'Скопировано', 'green', 5000);
                console.log('Копирование в буфер обмена успешно!');
            }
            document.body.removeChild(textarea);
        } catch (err) {
            changeTextTemporarily(messageField, 'Ошибка при копировании', 'red', 5000);
            console.error('Ошибка при копировании: ', err);
        }
    }
}

function closeAuthContainer() {
    authContainer.style.display = 'none';
}
