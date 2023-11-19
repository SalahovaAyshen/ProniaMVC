using FrontToBack_Pronia.Models;

namespace FrontToBack_Pronia.ViewModels
{
    public class HomeVM
    {
        public List<Shipping> Shipping { get; set; }
      
        public List<Slider> Slider { get; set; }

        public List<Product> Featured { get; set; }
        public List<Product> Latest { get; set; }
        public List<Product> NewProduct { get; set; }
        public List<ProductImage> ProductImages { get; set; }



    }
}
