using System.ComponentModel.DataAnnotations;

namespace FirstMVCWebApp.Dto
{
    public class UserDTO
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
