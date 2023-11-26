using FrontToBack_Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Areas.Manage.ViewModels
{
    public class CreateProductVM
    {
        [Required(ErrorMessage ="The product name can't be empty")]
        [MaxLength(25,ErrorMessage ="The length of the product name can't be more than 25 characters")]
        public string Name { get; set; }

        public decimal Price { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public string FullDescription { get; set; }
        public int? CategoryId { get; set; }
        public string? SKU { get; set; }
        public List<Category>? Categories { get; set; }
       
    }
}
