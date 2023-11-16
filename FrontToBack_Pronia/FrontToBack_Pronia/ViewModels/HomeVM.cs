using FrontToBack_Pronia.Models;

namespace FrontToBack_Pronia.ViewModels
{
    public class HomeVM
    {
        public List<Featured> Featured { get; set; }
        public List<Shipping> Shipping { get; set; }
        public List<LatestProducts> Latest { get; set; }
        public List<Slider> Slider { get; set; }

        public List<LatestProducts> NewProduct { get; set; }
    }
}
