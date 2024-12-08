using back.DataContext;
using back.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MyDataContext _context;

    public AuthController(MyDataContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Возвращает ошибку, если модель данных некорректна
        }
        if (_context.Users.Any(u => u.Email == user.Email))
        {
            return BadRequest("Пользователь с таким Email уже существует.");
        }

        // Хэшируем пароль перед сохранением
        user.Password = HashPassword(user.Password);
        try
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            // Логируем ошибку, если она произошла
            Console.Error.WriteLine($"Ошибка при добавлении пользователя: {ex.Message}");
            return BadRequest("Произошла ошибка при сохранении пользователя.");
        }

        return Ok(new { message = "Регистрация прошла успешно" });

    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginData)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == loginData.Email);

        if (user == null || !VerifyPassword(user.Password, loginData.Password))
        {
            return Unauthorized("Неверный Email или пароль.");
        }

        // Здесь вы можете сгенерировать JWT-токен
        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private bool VerifyPassword(string hashedPassword, string enteredPassword)
    {
        return hashedPassword == HashPassword(enteredPassword);
    }

    private string GenerateJwtToken(User user)
    {
        // Реализация генерации JWT
        // Например, используя System.IdentityModel.Tokens.Jwt
        return "your-jwt-token";
    }

}
