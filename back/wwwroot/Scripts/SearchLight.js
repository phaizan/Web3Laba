document.addEventListener('DOMContentLoaded', () => {
    const searchForm = document.querySelector('.search-form');
    const searchInput = document.querySelector('#search-input');
    const container = document.querySelector('.tour-container');

    searchForm.addEventListener('submit', async (event) => {
        event.preventDefault();
        const query = searchInput.value.trim();
        if (!query) return;

        const loadingIndicator = document.querySelector('.loading-indicator');
        loadingIndicator.style.display = 'block';

        try {
            const response = await fetch(`http://localhost:5047/api/Tours/search?query=${encodeURIComponent(query)}`);
            const tours = await response.json();

            container.innerHTML = ''; // Очищаем контейнер
            if (tours.length === 0) {
                container.innerHTML = '<p>Нет туров, соответствующих вашему запросу.</p>';
                return;
            }

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
                        <a href="${tour.linkToCard}" class="read-more">Подробнее</a>
                    </div>
                `;
                container.innerHTML += tourCard;
            });
        } catch (error) {
            console.error('Ошибка при загрузке туров:', error);
            container.innerHTML = '<p>Ошибка при загрузке данных. Пожалуйста, попробуйте позже.</p>';
        } finally {
            loadingIndicator.style.display = 'none';
        }
    });
});