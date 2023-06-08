using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        [Required]
        public string username {get;set;}

        [Required]
        [StringLength(8,  MinimumLength = 6)]
        public string password {get;set;}
    }
}