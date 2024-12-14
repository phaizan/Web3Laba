function getUserId() {

    const token = localStorage.getItem('jwt');
    if (!token) {
        console.log('Токен не найден в localStorage');
        return null;
    }

    try {

        // Пример с использованием jwt-decode

        const decodedToken = jwt_decode(token);
        console.log('Декодированный токен:', decodedToken);
        return decodedToken.id; // Возвращаем userId из токена
    } catch (e) {
        console.error('Ошибка декодирования токена:', e);
        return null;
    }
}

// Функция для получения userId с сервера, если его нет в токене
async function fetchUserIdFromServer() {
    const token = localStorage.getItem('jwt');
    if (!token) {
        console.log('Токен не найден при запросе userId с сервера');
        return null;
    }

    try {
        const response = await fetch('http://localhost:5047/api/Auth/user', {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const data = await response.json();
            console.log('Получены данные с сервера:', data);
            return data.userId; // Возвращаем userId, полученный с сервера
        } else {
            console.error('Ошибка при получении данных с сервера:', response.statusText);
            return null;
        }
    } catch (error) {
        console.error('Не удалось получить userId с сервера:', error);
        return null;
    }
}

// Функция, которая сначала проверяет localStorage, а если userId нет, то запрашивает его с сервера
async function getUserIdWithFallback() {
    let userId = getUserId();
    if (!userId) {
        console.log('userId не найден в токене, пытаемся получить с сервера');
        userId = await fetchUserIdFromServer();
    }
    return userId;
}

// Добавление тура в корзину
async function addToCart(tourId) {
    const userId = await getUserIdWithFallback(); // Используем await для получения userId

    if (!userId) {
        alert("Вы должны быть авторизованы для добавления товара в корзину.");
        return;
    }

    try {
        const response = await fetch(`http://localhost:5047/api/cart/${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ tourId, quantity: 1 }) // Добавляем количество
        });

        if (!response.ok) {
            throw new Error("Ошибка при добавлении в корзину.");
        }

        const textResponse = await response.text(); // Получаем текстовый ответ
        console.log("Добавлено в корзину:", textResponse);

        // Проверяем текстовый ответ
        if (textResponse === "Item added to cart.") {
            alert("Товар успешно добавлен в корзину!");
        } else {
            console.warn("Неожиданный ответ от сервера:", textResponse);
        }
    } catch (error) {
        console.error("Ошибка:", error);
        alert("Не удалось добавить товар в корзину.");
    }
}
document.addEventListener('DOMContentLoaded', () => {
    const bookButton = document.querySelector('.add-to-cart');
    if (bookButton) {
        bookButton.addEventListener('click', () => {
            const tourId = bookButton.getAttribute('data-tour-id');
            addToCart(tourId); // Вызываем функцию из serch.js
        });
    }
});
