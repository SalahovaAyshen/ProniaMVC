using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="The category name can't be empty")]
        [MaxLength(25,ErrorMessage ="The length of the category name can't be more than 25")]
        public string Name { get; set; }
        public List<Product>? Products { get; set; }
    }
}
