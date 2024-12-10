document.getElementById('feedbackForm').addEventListener('submit', async function (e) {
    e.preventDefault(); // Предотвращает перезагрузку страницы

// Собираем данные из формы
const formData = new FormData(this);
const data = Object.fromEntries(formData);

try {
        // Отправляем запрос на сервер
        const response = await fetch('api/feedback', {
    method: 'POST',
headers: {'Content-Type': 'application/json' },
body: JSON.stringify(data),
        });

if (response.ok) {
            const result = await response.json();
alert(result.message); // Показываем уведомление об успешной отправке
window.location.href = '../index.html'; // Переходим на главную страницу
        } else {
            const error = await response.json();
console.error('Ошибка:', error);
alert('Произошла ошибка при отправке отзыва. Проверьте данные.');
        }
    } catch (err) {
    console.error('Ошибка:', err);
alert('Произошла ошибка при отправке. Попробуйте позже.');
    }
});
