using back.DataContext;
using back.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly MyDataContext _context;
    private const string SecretKey = "your-very-strong-and-secure-key-1234567890123456"; // Минимум 32 символа

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
            Console.Error.WriteLine($"Ошибка при добавлении пользователя: {ex.Message}");
            return BadRequest("Произошла ошибка при сохранении пользователя.");
        }

        return Ok(new { message = "Регистрация прошла успешно" });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginData)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == loginData.Email);

        if (user == null || !VerifyPassword(user.Password, loginData.Password))
        {
            return Unauthorized("Неверный Email или пароль.");
        }

        // Генерация JWT-токена
        var token = GenerateJwtToken(user);
        return Ok(new
        {
            token,
            user = new
            {
                firstName = user.Name,
                lastName = user.Surname,
                email = user.Email
            }
        });
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.Name),
                new Claim("surname", user.Surname)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    private bool VerifyPassword(string hashedPassword, string enteredPassword)
    {
        return hashedPassword == HashPassword(enteredPassword);
    }

    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] string refreshToken)
    {
        var storedToken = _context.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

        if (storedToken == null || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return Unauthorized("Недействительный или истекший Refresh Token.");
        }

        var user = _context.Users.FirstOrDefault(u => u.Id == storedToken.UserId);

        var newJwtToken = GenerateJwtToken(user);
        return Ok(new { token = newJwtToken });
    }


}
