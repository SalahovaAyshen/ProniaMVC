using FrontToBack_Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Areas.Manage.ViewModels
{
    public class CreateColorVM
    {
        [Required(ErrorMessage = "The color name can't be empty")]
        [MaxLength(30, ErrorMessage = "The length of the color name can't be more than 25")]
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
