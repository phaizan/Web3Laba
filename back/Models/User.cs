using System.ComponentModel.DataAnnotations;

namespace back.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? City { get; set; }
        [Required]
        public string Password { get; set; }

        virtual public Cart? Cart { get; set; }
        
        

    }
}
