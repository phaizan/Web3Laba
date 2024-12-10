document.addEventListener("DOMContentLoaded", () => {
    const sortSelect = document.getElementById("sort-options");
    const toursList = document.querySelector(".tour-container");

    if (!toursList || !sortSelect) {
        console.error("Не удалось найти контейнер туров или поле сортировки.");
        return;
    }

    let originalTours = [];

    // Функция сортировки
    const sortTours = (option) => {
        let sortedTours = [...originalTours]; // Копируем оригинальный массив

        switch (option) {
            case "alphabetical":
                sortedTours.sort((a, b) =>
                    a.querySelector("h3").textContent.localeCompare(b.querySelector("h3").textContent)
                );
                break;
            case "price-asc":
                sortedTours.sort((a, b) => {
                    const priceA = parseFloat(a.dataset.price);
                    const priceB = parseFloat(b.dataset.price);
                    return priceA - priceB;
                });
                break;
            case "price-desc":
                sortedTours.sort((a, b) => {
                    const priceA = parseFloat(a.dataset.price);
                    const priceB = parseFloat(b.dataset.price);
                    return priceB - priceA;
                });
                break;
            case "mady-days":
                sortedTours = originalTours.filter(tour => tour.dataset.category === "Многодневный тур");
                break;
            case "one-day":
                sortedTours = originalTours.filter(tour => tour.dataset.category === "Однодневная экскурсия");
                break;
            default:
                break;
        }

        // Очистка и обновление контейнера
        toursList.innerHTML = "";
        sortedTours.forEach(tour => toursList.appendChild(tour));
    };

    // Подключение обработчика
    sortSelect.addEventListener("change", () => sortTours(sortSelect.value));

    // После загрузки данных сохраняем список карточек
    document.addEventListener("toursLoaded", () => {
        originalTours = Array.from(toursList.querySelectorAll(".tour-preview"));
    });
});
