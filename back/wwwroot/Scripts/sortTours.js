document.addEventListener("DOMContentLoaded", () => {
    const sortSelect1 = document.getElementById("sort-options-1");
    const sortSelect2 = document.getElementById("sort-options-2");
    const toursList = document.querySelector(".tour-container");

    if (!toursList || !sortSelect1 || !sortSelect2) {
        console.error("Не удалось найти контейнер туров или поле сортировки.");
        return;
    }

    let originalTours = [];
    // Функция сортировки
    const sortTours = () => {
        let sortedTours = [...originalTours]; // Копируем оригинальный массив

        const sortOption1 = sortSelect1.value; // Сортировка по алфавиту и цене
        const sortOption2 = sortSelect2.value; // Сортировка по категориям

        switch (sortOption1) {
            case "alphabetical":
                sortedTours.sort((a, b) =>
                    a.querySelector("h3").textContent.localeCompare(b.querySelector("h3").textContent)
                );
                break;
            case "price-asc":
                sortedTours.sort((a, b) => {
                    const priceA = parseInt(a.dataset.price);
                    const priceB = parseInt(b.dataset.price);
                    return priceA - priceB;
                });
                break;
            case "price-desc":
                sortedTours.sort((a, b) => {
                    const priceA = parseInt(a.dataset.price);
                    const priceB = parseInt(b.dataset.price);
                    return priceB - priceA;
                });
                break;
        }

        console.log("Выбранная категория: ", sortOption2);
        console.log("Все туры: ", originalTours.map(tour => tour.dataset.category)); // Показать все категории туров

        // Фильтрация по категориям
        if (sortOption2 !== "all") {
            sortedTours = sortedTours.filter(tour => {
                // Сопоставляем значения категорий из селекта и данных тура
                const categoryText = {
                    "many-days": "Многодневный тур",
                    "one-day": "Однодневная экскурсия",
                };

                return tour.dataset.category === categoryText[sortOption2];
            });
        }

        // Очистка и обновление контейнера
        toursList.innerHTML = "";
        sortedTours.forEach(tour => toursList.appendChild(tour));
    };

    // Подключение обработчика
    sortSelect1.addEventListener("change", sortTours);
    sortSelect2.addEventListener("change", sortTours);

    // После загрузки данных сохраняем список карточек
    document.addEventListener("toursLoaded", () => {
        originalTours = Array.from(toursList.querySelectorAll(".tour-preview"));
    });
});
