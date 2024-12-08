using System.ComponentModel.DataAnnotations;

namespace back.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string? City { get; set; }
        [Required]
        public string Password { get; set; }

        
        

    }
}
