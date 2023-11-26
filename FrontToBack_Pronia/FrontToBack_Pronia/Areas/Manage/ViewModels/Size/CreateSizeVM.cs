using FrontToBack_Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Areas.Manage.ViewModels

{
    public class CreateSizeVM
    {

        [Required(ErrorMessage = "The size name can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the size name can't be more than 25")]
        public string Name { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
