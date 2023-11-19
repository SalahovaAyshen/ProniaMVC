using FrontToBack_Pronia.Models;

namespace FrontToBack_Pronia.ViewModels
{
    public class ProductVM
    {
        public Product Products { get; set; }
        public List<Product>? ProductList { get; set; }
        public Category Categoriess { get; set; }
    }
}
