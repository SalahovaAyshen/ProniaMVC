﻿using FrontToBack_Pronia.Models;

namespace FrontToBack_Pronia.ViewModels
{
    public class ShopVM
    {
        public int? Order { get; set; }
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }
    }
}
