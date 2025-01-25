const inputField = document.getElementById('inputField');
const outputField = document.getElementById('outputField');

const profileUsernameField = document.getElementById('username');
const profileEmailField = document.getElementById('email');

const profilePopup = document.getElementById('profilePopup');

const inputContainer = document.getElementById('inputContainer');
const outputContainer = document.getElementById('outputContainer');
const authContainer = document.getElementById('authContainer');
const mainContainer = document.getElementById('mainContainer');

const copyHtmlButton = document.getElementById('copyHtmlButton');
const downloadHtmlButton = document.getElementById('downloadHtmlButton');
const messageField = document.getElementById('messageField');
const fullscreenInputButton = document.getElementById('fullscreenInputButton');
const fullscreenOutputButton = document.getElementById('fullscreenOutputButton');
const openFullscreenButton = document.getElementById('openFullscreenButton');
const logoutButton = document.getElementById('logoutButton');

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
        mainContainer.requestFullscreen();
        openFullscreenButton.textContent = 'Выйти из полноэкранного режима'
    } else {
        document.exitFullscreen();
        openFullscreenButton.textContent = 'Полноэкранный режим'
    }
});

openProfileButton.addEventListener('click', async () => {
    if (await getProfile()) {
        profilePopup.style.display = 'block';
        authContainer.style.display = 'none';
    } else {
        authContainer.style.display = 'block';
        profilePopup.style.display = 'none';
    }
});

loginForm.addEventListener('submit', async (event) => {
    event.preventDefault(); // предотвращает стандартное поведение формы
    await login();
});

registerForm.addEventListener('submit', async (event) => {
    event.preventDefault();
    await register();
});

logoutButton.addEventListener('click', () => {
    logout();
})
async function register() {
    const username = document.getElementById('registerUsername').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;

    if (!username || !email || !password) {
        alert("Все поля должны быть заполнены!");
        return;
    }

    try {
        const response = await fetch('https://localhost:7102/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, email, password }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Ошибка регистрации");
        }

        const data = await response.json();
        alert(data.message);
    } catch (error) {
        alert(error.message || "Ошибка регистрации");
    }
}
async function login() {
    const email = document.getElementById('loginEmail').value;
    const password = document.getElementById('loginPassword').value;

    if (!email || !password) {
        alert("Все поля должны быть заполнены!");
        return;
    }

    try {
        const response = await fetch('https://localhost:7102/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email, password }),
            credentials: 'include', // для работы с куками
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Ошибка входа");
        }

        const data = await response.json();
        alert("Вход выполнен успешно!");

        window.location.href = "/";
    } catch (error) {
        alert(error.message || "Ошибка входа");
    }
}

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

async function getProfile() {
    try {
        const response = await fetch('https://localhost:7102/api/auth/profile', {
            method: 'GET',
            credentials: 'include', // включает куки в запрос
        });

        if (response.ok) {
            const data = await response.json();
            profileUsernameField.innerText = data.username;
            profileEmailField.innerText = data.email;
            console.log('Авторизован');
            return true;
        } else {
            console.log('Не авторизован');
            return false;
        }
    } catch (error) {
        console.error('Ошибка проверки авторизации: ', error);
        return false;
    }

    return true;
}

async function logout() {
    try {
        const response = await fetch('https://localhost:7102/api/auth/logout', {
            method: 'POST',
            credentials: 'include', // включает куки в запрос
        });

        if (response.ok) {
            window.location.href = "/";
            console.log('Успешный выход');
            return true;
        } else {
            console.log('Ошибка при выходе');
            return false;
        }
    } catch (error) {
        console.error('Ошибка сети: ', error);
        return false;
    }
    
}
