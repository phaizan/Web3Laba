using back.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация подключения к базе данных
builder.Services.AddDbContext<MyDataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 40))));

// Добавление поддержки аутентификации JWT
var secretKey = "your-very-strong-and-secure-key-1234567890123456"; // Используется в AuthController
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // Не проверяем издателя (можно настроить на true, если есть конкретный издатель)
            ValidateAudience = false, // Не проверяем аудиторию (можно настроить на true, если есть конкретная аудитория)
            ValidateLifetime = true, // Проверяем срок действия токена
            ValidateIssuerSigningKey = true, // Проверяем подпись токена
            IssuerSigningKey = new SymmetricSecurityKey(key) // Ключ для подписи
        };
    });

// Добавление контроллеров
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Настройка обработки запросов и статических файлов
app.UseDefaultFiles(); // Для обработки index.html как стартовой страницы
app.UseStaticFiles();  // Для отдачи статических файлов

// Подключение Swagger (документация API)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger"; // Swagger будет доступен по адресу /swagger
});

// Включение HTTPS перенаправления
app.UseHttpsRedirection();

// Подключение аутентификации и авторизации
app.UseAuthentication(); // Добавлено для обработки JWT-токенов
app.UseAuthorization();

// Маршрутизация контроллеров
app.MapControllers();

// Запуск приложения
app.Run();
