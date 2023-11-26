using FrontToBack_Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Areas.Manage.ViewModels
{
    public class CreateTagVM
    {
        [Required(ErrorMessage = "The tag name can't be empty")]
        [MaxLength(25, ErrorMessage = "The length of the tag name can't be more than 25")]
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
