using back.Models;
using Microsoft.EntityFrameworkCore;

namespace back.DataContext
{
    public class MyDataContext : DbContext
    {
        public MyDataContext(DbContextOptions<MyDataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=travel_agency;User=root;Password=yaprogerUmnik();",
                ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=travel_agency;User=root;Password=yaprogerUmnik();"));
        }
    }
}
