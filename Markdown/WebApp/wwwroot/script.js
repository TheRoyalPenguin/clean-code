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
const saveInputButton = document.getElementById('saveInputButton');
const shareDocumentSettingsButton = document.getElementById('shareDocumentSettingsButton');

const openProfileButton = document.getElementById('profileBtn');
const loginForm = document.getElementById('loginForm');
const registerForm = document.getElementById('registerForm');
const errorAuthMessage = document.getElementById('errorAuthMessage');


const accessFormEmail = document.getElementById("accessFormEmail");
const permission = document.getElementById("permission");
const accessForm = document.getElementById("accessForm");

const myDocumentGridTitle = document.getElementById("myDocumentGridTitle");
const availableDocumentGridTitle = document.getElementById("availableDocumentGridTitle");

let saveMarkdownTimer;
let setHtmlTimer;

// при перезагрузке страницы
documentClick(sessionStorage.getItem('currentDocumentId') || null);
setHtmlTextToOutputField();

inputField.addEventListener("input", async () => {
    await saveMarkdown();
    await setHtmlTextToOutputField();
});

// копирование html кода
copyHtmlButton.addEventListener('click', () => {
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
    const isAuthorized = await getProfile();
    if (isAuthorized) {
        if (profilePopup.style.display === 'block') {
            profilePopup.style.display = 'none';
        } else {
            profilePopup.style.display = 'block';
            authContainer.style.display = 'none';
        }
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

myDocumentsButton.addEventListener('click', async () => {
    profilePopup.style.display = 'none';
    await getDocuments();
})

shareDocumentSettingsButton.addEventListener('click', async () => {
    showShareDocumentSettings();
})

accessForm.addEventListener('submit', async (event) => {
    await shareDocument(event);
})

saveInputButton.addEventListener('click', async () => {
    await saveMarkdown();
})

async function saveMarkdown() {
    clearTimeout(saveMarkdownTimer);
    saveMarkdownTimer = setTimeout(async () => {
        const inputText = inputField.value;
        if (inputText) {
            const currentDocumentId = sessionStorage.getItem('currentDocumentId') || null;

            const inputText = inputField.value;
            const file = new File([inputText], "document.txt", { type: "text/plain" });

            const formData = new FormData();
            formData.append("Title", inputText.substring(0, 10));
            formData.append("File", file);
            if (currentDocumentId != null) {
                formData.append("Id", currentDocumentId);
            }

            try {
                const response = await fetch("api/documents/save", {
                    method: 'POST',
                    body: formData,
                    credentials: 'include',
                });

                const result = await response.json();
                if (response.ok) {
                    sessionStorage.setItem("currentDocumentId", result.value.id);
                    console.log('Успешно сохранено.');
                    changeTextTemporarily(messageField, 'Документ сохранён', 'green', 5000);
                } else {
                    changeTextTemporarily(messageField, result.errorMessage, 'red', 5000);
                    console.log('Ошибка сохранения.');
                    throw new Error("Ошибка при предоставлении доступа.");
                }
            } catch (error) {
                console.error(error);
            }
        }
    }, 500);
}

async function register() {
    const username = document.getElementById('registerUsername').value;
    const email = document.getElementById('registerEmail').value;
    const password = document.getElementById('registerPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    if (!username || !email || !password || !confirmPassword) {
        errorAuthMessage.textContent = "Все поля должны быть заполнены!";
        return;
    }
    if (password != confirmPassword) {
        errorAuthMessage.textContent = "Пароли не совпадают!";
        return;
    }
    try {
        const response = await fetch('api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ username, email, password }),
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Ошибка регистрации.");
        }

        const data = await response.json();
        login(email, password);
    } catch (error) {
        console.log(error.message || "Ошибка регистрации.");
    }
}
async function login(email = null, password = null) {
    email = email || document.getElementById('loginEmail').value;
    password = password || document.getElementById('loginPassword').value;

    if (!email || !password) {
        errorAuthMessage.textContent("Все поля должны быть заполнены!");
        return;
    }

    try {
        const response = await fetch('api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ email, password }),
            credentials: 'include' // для работы с куками
        });

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message || "Ошибка входа.");
        }

        const data = await response.json();
        window.location.href = "/";
    } catch (error) {
        errorAuthMessage.textContent = error.message || "Ошибка входа.";
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
        const response = await fetch("/markdown/convert", {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ InputText: inputText })
        });

        const result = await response.json();
        if (response.ok) {
            outputField.innerHTML = result.htmlText;
        } else {
            console.log('Ошибка.');
        }
    }
    catch {
        console.error(error);
    }
}
let timeoutId = null;
let isFirstCall = true;
let originalText, originalColor;
function changeTextTemporarily(element, newText, color, duration) {
    if (isFirstCall) {
        originalText = element.textContent;
        originalColor = element.style.color;
        isFirstCall = false;
    }

    if (timeoutId !== null) {
        clearTimeout(timeoutId);
    }

    element.textContent = newText;
    element.style.color = color;

    timeoutId = setTimeout(() => {
        element.textContent = originalText;
        element.style.color = originalColor;

        timeoutId = null;
    }, duration);
}

async function setHtmlTextToOutputField() {
    clearTimeout(setHtmlTimer);
    setHtmlTimer = setTimeout(async () => {
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
            changeTextTemporarily(messageField, 'Ошибка при копировании.', 'red', 5000);
            console.error('Ошибка при копировании: ', err);
        }
    }
}

function closeAuthContainer() {
    authContainer.style.display = 'none';
}

function closeShareDocumentSettingsContainer() {
    shareDocumentSettingsContainer.style.display = 'none';
}

async function getProfile() {
    try {
        const response = await fetch('api/auth/profile', {
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
        const response = await fetch('api/auth/logout', {
            method: 'POST',
            credentials: 'include', // включает куки в запрос
        });

        if (response.ok) {
            window.location.href = "/";
            sessionStorage.removeItem('currentDocumentId');
            console.log('Успешный выход.');
            return true;
        } else {
            console.log('Ошибка при выходе.');
            return false;
        }
    } catch (error) {
        console.error('Ошибка сети: ', error);
        return false;
    }
    
}

async function fetchDocuments(url) {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`Ошибка при запросе: ${url}`);
        }
        return await response.json();
    } catch (error) {
        console.error("Ошибка:", error.message);
        return [];
    }
}

async function getDocuments() {
    mainContainer.style.display = 'none';
    documentGrid.style.display = 'block';
    const ownerDocuments = await fetchDocuments(`api/documents/my`);
    renderDocuments(ownerDocuments, true);

    const availableDocuments = await fetchDocuments(`api/documents/available`);
    renderDocuments(availableDocuments, false);
}

async function deleteDocument(documentId) {
    try {
        const response = await fetch(`api/documents/${documentId}/delete`, {
            method: 'POST',
            credentials: 'include'
        });
        if (!response.ok) {
            const errorData = await response.json();
            alert(errorData.message);
            console.log(errorData.message);
            throw new Error("Ошибка удаления документа.");
        }

        await response;
        console.log('Документ удалён.');
        await getDocuments();
    } catch (error) {
        console.error("Ошибка:", error);
    }
}
function renderDocuments(documents, isOwnerDocuments) {
    let documentGrid = document.getElementById('availableDocumentGrid');
    documentGrid.innerHTML = "";

    myDocumentGridTitle.style.textAlign = "center";
    availableDocumentGridTitle.style.textAlign = "center";
    availableDocumentGridTitle.textContent = "Доступные документы";
    myDocumentGridTitle.textContent = "Мои документы";

    if (isOwnerDocuments) {
        documentGrid = document.getElementById('myDocumentGrid');
        documentGrid.innerHTML = "";

        const card = document.createElement("div");
        card.className = "card";
        const title = document.createElement("div");
        title.className = "cardTitle";
        title.textContent = "Создать документ";
        card.appendChild(title);
        card.addEventListener("click", async () => {
            sessionStorage.removeItem('currentDocumentId');
            await displayMainContainer("")
        });
        documentGrid.appendChild(card);
    }

    documentGrid.style.display = 'flex';

    documents.forEach((doc) => {
        const card = document.createElement("div");
        card.className = "card";
        card.dataset.id = doc.id;

        const deleteButton = document.createElement("button");
        deleteButton.className = "deleteButton";
        deleteButton.textContent = "×";
        deleteButton.title = "Удалить";

        deleteButton.addEventListener("click", async () => {
            await deleteDocument(doc.id);
        });

        const header = document.createElement("div");
        header.className = "cardHeader";

        const title = document.createElement("div");
        title.className = "cardTitle";
        title.textContent = doc.title;

        header.appendChild(title);
        if (isOwnerDocuments) {
            header.appendChild(deleteButton);
        }
        const dates = document.createElement("div");
        dates.className = "cardDates";
        let modifiedDate = doc.createdAt;
        if (!isNaN(new Date(doc.lastModifiedAt).getTime())) {
            modifiedDate = doc.lastModifiedAt
        }
        dates.innerHTML = `
            <div>Создано: ${new Date(doc.createdAt).toLocaleDateString()}</div>
            <div>Изменено: ${new Date(modifiedDate).toLocaleDateString()}</div>
        `;

        card.appendChild(header);
        card.appendChild(dates);

        card.addEventListener("click", () => documentClick(doc.id));

        documentGrid.appendChild(card);
    });
}

async function documentClick(documentId) {
    try {
        const response = await fetch(`api/documents/${documentId}/content`);
        if (!response.ok) {
            throw new Error("Ошибка загрузки документа.");
        }

        const text = await response.text();

        sessionStorage.setItem("currentDocumentId", documentId);
        displayMainContainer(text);
    } catch (error) {
        console.error("Ошибка:", error.message);
    }
}

async function displayMainContainer(text) {
    mainContainer.style.display = 'flex';
    documentGrid.style.display = 'none';
    inputField.value = text;
    await setHtmlTextToOutputField();
}
function showShareDocumentSettings() {
    if (shareDocumentSettingsContainer.style.display === 'block') {
        shareDocumentSettingsContainer.style.display = 'none';
    } else {
        shareDocumentSettingsContainer.style.display = 'block';
    }
    const currentDocumentId = sessionStorage.getItem('currentDocumentId') || null;
    fetchAndDisplayUsers(currentDocumentId);
}

async function shareDocument(event) {
    event.preventDefault(); // предотвращаем стандартное поведение формы

    const currentDocumentId = sessionStorage.getItem('currentDocumentId') || null;
    const formData = new FormData();
    formData.append("Email", accessFormEmail.value);
    formData.append("AccessLevel", permission.value);

    try {
        const response = await fetch(`api/documents/${currentDocumentId}/permissions/set`, {
            method: "POST",
            credentials: 'include',
            body: formData
        });

        if (!response.ok) {
            const result = await response.json();
            changeTextTemporarily(messageField, result.errorMessage, 'red', 5000);
            throw new Error("Ошибка при предоставлении доступа.");
        }

        changeTextTemporarily(messageField, 'Доступ успешно предоставлен!', 'green', 5000);
    } catch (error) {
        console.error(error.message);
    }
}

function renderUsersWithAccess(readAccessUsers, writeAccessUsers, documentId) {
    const readAccessUsersContainer = document.getElementById("readAccessUsers");
    const writeAccessUsersContainer = document.getElementById("writeAccessUsers");

    readAccessUsersContainer.innerHTML = "";
    writeAccessUsersContainer.innerHTML = "";

    document.getElementById("readAccessTitle").style.textAlign = "center";
    document.getElementById("writeAccessTitle").style.textAlign = "center";

    readAccessUsers.forEach(user => {
        const card = createUserCard(user, documentId, "Read");
        readAccessUsersContainer.appendChild(card);
    });

    writeAccessUsers.forEach(user => {
        const card = createUserCard(user, documentId, "Write");
        writeAccessUsersContainer.appendChild(card);
    });
}

function createUserCard(user, documentId, currentAccessLevel) {
    const card = document.createElement("div");
    card.className = "card";
    card.dataset.id = user.id;

    const header = document.createElement("div");
    header.className = "cardHeader";

    const title = document.createElement("div");
    title.className = "cardTitle";
    title.textContent = user.name || user.email;
    header.appendChild(title);

    const actions = document.createElement("div");
    actions.className = "cardActions";

    if (currentAccessLevel === "Read") {
        const changeToWriteButton = document.createElement("button");
        changeToWriteButton.textContent = "Изменить на запись";
        changeToWriteButton.addEventListener("click", async () => {
            await changeAccess(documentId, "Write");
        });
        actions.appendChild(changeToWriteButton);
    } else {
        const changeToReadButton = document.createElement("button");
        changeToReadButton.textContent = "Изменить на чтение";
        changeToReadButton.addEventListener("click", async () => {
            await changeAccess(documentId, "Read");
        });
        actions.appendChild(changeToReadButton);
    }

    const deleteButton = document.createElement("button");
    deleteButton.textContent = "Удалить доступ";
    deleteButton.addEventListener("click", async () => {
        await removeAccess(documentId, user.id);
    });
    actions.appendChild(deleteButton);

    header.appendChild(actions);
    card.appendChild(header);

    return card;
}

async function changeAccess(documentId, newAccessLevel) {
    const formData = new FormData();
    formData.append("Email", accessFormEmail.value);
    formData.append("AccessLevel", newAccessLevel)
    try {
        const response = await fetch(`/api/documents/${documentId}/permissions/set`, {
            method: "POST",
            body: formData
        });

        if (!response.ok) {
            throw new Error("Ошибка при изменении уровня доступа.");
        }

        fetchAndDisplayUsers(documentId); // обновление списков
    } catch (error) {
        console.error("Ошибка:", error.message);
    }
}

async function removeAccess(documentId, userId) {
    try {
        const response = await fetch(`/api/documents/${documentId}/permissions/remove`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ userId }),
        });

        if (!response.ok) {
            throw new Error("Ошибка при удалении доступа.");
        }

        console.log("Доступ успешно удалён.");
        fetchAndDisplayUsers(documentId); // обновление списков
    } catch (error) {
        console.error("Ошибка:", error.message);
    }
}


async function fetchAndDisplayUsers(documentId) {
    try {
        const readAccessResponse = await fetch(`/api/documents/${documentId}/users-with-read-permission`);
        if (!readAccessResponse.ok) {
            const errorData = await readAccessResponse.json(); 
            throw new Error(errorData.message || "Ошибка при получении пользователей с доступом для чтения.");
        }
        const readAccessUsers = await readAccessResponse.json();

        const writeAccessResponse = await fetch(`/api/documents/${documentId}/users-with-write-permission`);
        if (!writeAccessResponse.ok) {
            const errorData = await writeAccessResponse.json();
            throw new Error(errorData.message || "Ошибка при получении пользователей с доступом для редактирования.");
        }
        const writeAccessUsers = await writeAccessResponse.json();

        renderUsersWithAccess(readAccessUsers, writeAccessUsers, documentId);
    } catch (error) {
        console.error("Ошибка:", error.message);
    }
}
