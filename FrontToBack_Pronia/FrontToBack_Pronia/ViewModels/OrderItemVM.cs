using FrontToBack_Pronia.Models;
using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.ViewModels
{
    public class OrderItemVM
    {
        [Required]
        public string Address { get; set; }
        public List<BasketItem>? BasketItems { get; set; }
    }
}
