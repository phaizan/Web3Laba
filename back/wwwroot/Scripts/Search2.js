async function searchTours() {
    const query = document.getElementById("search-input").value;
    const resultsContainer = document.getElementById("search-results");
    const resultsList = document.getElementById("results-list");

    if (!query) {
        resultsContainer.style.display = "none";
        return;
    }

    try {
        const response = await fetch(`http://localhost:5047/api/tours/search?query=${query}`);
        const tours = await response.json();
        console.dir(tours); // Покажет объект tours в виде дерева

        // Очистим предыдущие результаты
        resultsList.innerHTML = '';

        if (tours.length === 0) {
            resultsContainer.style.display = "none";
            return;
        }

        // Заполняем результаты поиска
        tours.forEach(tour => {
            console.log("Туры:" + tour.tourName);

            const listItem = document.createElement("li");
            listItem.textContent = tour.tourName; // Покажем название тура
            listItem.onclick = function () {
                window.location.href = tour.linkToCard; // Перенаправление на карточку тура
            };
            resultsList.appendChild(listItem);
        });

        resultsContainer.style.display = "block"; // Показываем список результатов
    } catch (error) {
        console.error("Ошибка при поиске туров:", error);
        resultsContainer.style.display = "none";
    }
}
