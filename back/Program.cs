using back.DataContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ������������ ����������� � ���� ������
builder.Services.AddDbContext<MyDataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 40))));

// ���������� ��������� �������������� JWT
var secretKey = "your-very-strong-and-secure-key-1234567890123456"; // ������������ � AuthController
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // �� ��������� �������� (����� ��������� �� true, ���� ���� ���������� ��������)
            ValidateAudience = false, // �� ��������� ��������� (����� ��������� �� true, ���� ���� ���������� ���������)
            ValidateLifetime = true, // ��������� ���� �������� ������
            ValidateIssuerSigningKey = true, // ��������� ������� ������
            IssuerSigningKey = new SymmetricSecurityKey(key) // ���� ��� �������
        };
    });

// ���������� ������������
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ��������� ��������� �������� � ����������� ������
app.UseDefaultFiles(); // ��� ��������� index.html ��� ��������� ��������
app.UseStaticFiles();  // ��� ������ ����������� ������

// ����������� Swagger (������������ API)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    c.RoutePrefix = "swagger"; // Swagger ����� �������� �� ������ /swagger
});

// ��������� HTTPS ���������������
app.UseHttpsRedirection();

// ����������� �������������� � �����������
app.UseAuthentication(); // ��������� ��� ��������� JWT-�������
app.UseAuthorization();

// ������������� ������������
app.MapControllers();

// ������ ����������
app.Run();
