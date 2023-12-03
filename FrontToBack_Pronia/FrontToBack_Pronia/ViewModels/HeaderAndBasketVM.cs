using FrontToBack_Pronia.ViewComponents;

namespace FrontToBack_Pronia.ViewModels
{
    public class HeaderAndBasketVM
    {
        public Dictionary<string, string> Header { get; set; }
        public List<BasketItemVM> Basket { get; set; }
    }
}
