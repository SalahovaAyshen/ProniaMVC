using System.ComponentModel.DataAnnotations;

namespace FrontToBack_Pronia.Models
{
    public class Color
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
