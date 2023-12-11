using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.ViewModels
{
    public class OrderItemVM
    {
        [Required(ErrorMessage ="The name can't be empty")]
        [MinLength(3, ErrorMessage ="The length of the name can't be less than 3")]
        [MaxLength(25, ErrorMessage ="The length of the name can't be more than 25")]
        public string Name { get; set; }
        [Required(ErrorMessage ="The surname can't be empty")]
        [MinLength(3,ErrorMessage ="The length of the surname can't be less than 3")]
        [MaxLength(25, ErrorMessage ="The length of the surname can't be more than 25")]
        public string Surname { get; set; }
        [Required(ErrorMessage ="The email address can't be empty")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage ="Address can't be empty")]
        public string Address { get; set; }
        public List<CheckoutItemVM> CheckoutItems = new List<CheckoutItemVM>();
    }
}
