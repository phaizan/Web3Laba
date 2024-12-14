document.addEventListener("DOMContentLoaded", () => {
    const userInfoElement = document.getElementById('user-info');
    const loginForm = document.getElementById('login-form');

    // Получение заголовков с токеном
    const getHeadersWithToken = () => {
        const token = localStorage.getItem('jwt');
        return token ? { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' } : { 'Content-Type': 'application/json' };
    };

    // Проверка токена и получение информации о пользователе
    const checkTokenAndFetchUser = async () => {
        const headers = getHeadersWithToken();
        if (headers.Authorization) {
            try {
                const response = await fetch('http://localhost:5047/api/auth/user', { headers });
                if (response.ok) {
                    const data = await response.json();
                    if (data && data.lastName && data.firstName) {
                        userInfoElement.innerHTML = `
                            Добро пожаловать, ${data.lastName} ${data.firstName[0]}. <br>
                            <div style="text-align: right; margin-top: 30px; ">
                            <a href="../cart.html"><img src="../../images/cart.png" style="position: absolute; top: 55px; right: 150px; height: 70px; "/></a>
                            <button id="logout-button" class="button1">Выход</button>
                            </div>
                            `;
                        loginForm.style.display = 'none';
                        document.getElementById('logout-button').addEventListener('click', logout);
                    }
                } else {
                    localStorage.removeItem('jwt'); // Удаляем токен, если он больше недействителен
                }
            } catch {
                userInfoElement.innerHTML = 'Не удалось загрузить информацию о пользователе.';
            }
        }
    };

    // Обработчик входа
    document.getElementById('login-button').addEventListener('click', async () => {
        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value.trim();

        if (!email || !password) {
            alert("Заполните все поля!");
            return;
        }

        try {
            const response = await fetch('http://localhost:5047/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password })
            });

            if (!response.ok) {
                const error = await response.json();
                console.error("Ошибка:", error);
                throw new Error(error.message || "Ошибка подключения");
            }

            const data = await response.json();
            localStorage.setItem('jwt', data.token);
            console.log("Успешный вход:", data);
            checkTokenAndFetchUser(); // Перезагружаем приветствие
        } catch (error) {
            console.error("Ошибка запроса:", error);
            alert("Не удалось подключиться к серверу.");
        }
    });

    // Обработчик выхода
    const logout = () => {
        localStorage.removeItem('jwt'); // Удаляем токен
        userInfoElement.innerHTML = ''; // Сбрасываем приветствие
        loginForm.style.display = 'block'; // Показываем форму входа
    };

    // Выполнение проверки токена при загрузке
    checkTokenAndFetchUser();
});

