document.addEventListener('DOMContentLoaded', () => {

    const apiUrl = 'http://localhost:5047/api/Tours'; // API для загрузки туров
    const container = document.querySelector('.tour-container');
    const searchInput = document.getElementById('search-input');
    const searchForm = document.querySelector('.search-form');
    const sortOptions1 = document.getElementById('sort-options-1'); // Сортировка по алфавиту или цене
    const sortOptions2 = document.getElementById('sort-options-2'); // Фильтрация по категории
    let toursData = []; // Все туры
    let filteredTours = []; // Текущие отображаемые туры

    // Загрузка туров
    async function loadTours() {
        const loadingIndicator = document.querySelector('.loading-indicator');
        loadingIndicator.style.display = 'block';

        try {
            const response = await fetch(apiUrl);
            toursData = await response.json();
            filteredTours = [...toursData]; // Сохраняем исходный массив
            renderTours(filteredTours);
        } catch (error) {
            console.error('Ошибка при загрузке туров:', error);
            container.innerHTML = '<p>Ошибка при загрузке данных. Пожалуйста, попробуйте позже.</p>';
        } finally {
            loadingIndicator.style.display = 'none';
        }
    }

    // Получение userId из localStorage (или JWT токена)
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
            const response = await fetch('http://localhost:5047/api/auth/user', {
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
        console.log("User Id:", userId);

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

    // Отображение туров
    function renderTours(tours) {
        container.innerHTML = ''; // Очищаем контейнер перед рендерингом туров
        tours.forEach(tour => {
            const tourCard = `
            <div class="tour-preview" data-category="${tour.tourCategory}" data-price="${tour.price}">
                <a href="${tour.linkToCard}">
                    <img src="${tour.picture}" alt="${tour.tourName}">
                    <h3>${tour.tourName}</h3>
                </a>
                <p>${tour.shortDescription}</p>
                <div class="tour-details">
                    <p>Свободных мест: ${tour.amountofPlaces}</p>
                    <p class="tour-category">Категория: ${tour.tourCategory}</p>
                    <p class="tour-price">${tour.price.toLocaleString('ru-RU')} руб.</p>
                </div>
                <button class="add-to-cart" data-tour-id="${tour.id}">Добавить в корзину</button>
            </div>
        `;
            container.innerHTML += tourCard;
        });

        // Добавляем обработчик событий для кнопок "Добавить в корзину"
        const addToCartButtons = document.querySelectorAll('.add-to-cart');
        addToCartButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const tourId = button.getAttribute('data-tour-id');
                addToCart(tourId);
            });
        });
    }

    // Фильтрация по поисковому запросу
    function filterBySearch(query) {
        query = query.toLowerCase();
        filteredTours = toursData.filter(tour =>
            tour.tourName.toLowerCase().includes(query)
        );
        applyFiltersAndSort();
    }

    // Сортировка по выбранным параметрам
    function sortTours() {
        const sortOption = sortOptions1.value;
        if (sortOption === 'alphabetical') {
            filteredTours.sort((a, b) => a.tourName.localeCompare(b.tourName));
        } else if (sortOption === 'price-asc') {
            filteredTours.sort((a, b) => a.price - b.price);
        } else if (sortOption === 'price-desc') {
            filteredTours.sort((a, b) => b.price - a.price);
        }
    }

    // Фильтрация по категории
    function filterByCategory() {
        const category = sortOptions2.value;
        console.log("Выбранная категория: " + category);

        if (category === 'all') {
            filteredTours = [...toursData];
        } else {
            const categoryText = {
                'many-days': 'Многодневный тур',
                'one-day': 'Однодневная экскурсия',
            };
            filteredTours = toursData.filter(tour => tour.tourCategory === categoryText[category]);
        }
    }

    // Применение фильтров и сортировки
    function applyFiltersAndSort() {
        sortTours(); // Сначала сортируем
        renderTours(filteredTours); // Затем отображаем
    }

    // Обработка формы поиска
    searchForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const query = searchInput.value.trim();
        filterBySearch(query);
    });

    // Обработка изменения сортировки
    sortOptions1.addEventListener('change', applyFiltersAndSort);
    sortOptions2.addEventListener('change', () => {
        filterByCategory();
        applyFiltersAndSort();
    });

    // Загрузка данных при загрузке страницы
    loadTours();
});
