namespace FrontToBack_Pronia.Models
{
    public class ProductSize
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Products { get; set; }
        public int SizeId { get; set; }
        public Size Size { get; set; }
    }
}
