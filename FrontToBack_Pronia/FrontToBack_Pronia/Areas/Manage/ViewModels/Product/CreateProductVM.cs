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
        [Required]
        public int? CategoryId { get; set; }
        public string? SKU { get; set; }
        public List<int> TagIds { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<int> ColorIds { get; set; }
        public List<Color>? Colors { get; set; }
        public List<int> SizeIds { get; set; }
        public List<Size>? Sizes { get; set; }
        public List<Category>? Categories { get; set; }
       
    }
}
