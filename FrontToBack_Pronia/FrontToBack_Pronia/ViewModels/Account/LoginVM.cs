using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage ="You must entire username or email ")]
        [MinLength(4, ErrorMessage ="The length of the username or email can't be less than 4")]
        [MaxLength(320, ErrorMessage = "The length of the username or email can't be more than 320")]
        public string UsernameOrEmail { get; set; }
        [Required(ErrorMessage ="You must entire password")]
        [MinLength(8,ErrorMessage ="The length of the password can't be less than 8")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemembered { get; set; }
    }
}
