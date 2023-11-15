using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrontToBack_Pronia.Controllers
{
    public class HomeController : Controller
    {
        List<Shipping> shippings = new List<Shipping>
        {
            new Shipping {Image="/assets/images/website-images//car.png", Title="Free Shipping", Description="Capped at $319 per order"},
            new Shipping {Image="/assets/images/website-images//card.png", Title="Safe payment", Description="With our payment gateway"},
            new Shipping {Image="/assets/images/website-images//service.png", Title="Best services", Description="Friendly and super services"}


        };

        List<Product> products = new List<Product>
        {
            new Product{FirstImage="/assets/images/website-images//1-1-270x300.jpg",SecondImage="/assets/images/website-images//1-2-270x300.jpg",Name="American Marigold",Price="$23.45"},
            new Product{FirstImage="/assets/images/website-images//1-2-270x300.jpg",SecondImage="assets/images/website-images//1-3-270x300.jpg",Name="Black Eyed Susan",Price="$25.45"},
            new Product{FirstImage="/assets/images/website-images//1-3-270x300.jpg",SecondImage="assets/images/website-images//1-4-270x300.jpg",Name="Bleeding Heart",Price="$30.45"},
            new Product{FirstImage="/assets/images/website-images//1-4-270x300.jpg",SecondImage="/assets/images/website-images//1-5-270x300.jpg",Name="Bloody Cranesbill",Price="$45.00"},
            new Product{FirstImage="/assets/images/website-images//1-5-270x300.jpg",SecondImage="/assets/images/website-images//1-6-270x300.jpg",Name="Butterfly Weed",Price="$50.45"},
            new Product{FirstImage="/assets/images/website-images//1-6-270x300.jpg",SecondImage="/assets/images/website-images//1-7-270x300.jpg",Name="Common Yarrow",Price="$65.00"},
            new Product{FirstImage="/assets/images/website-images//1-7-270x300.jpg",SecondImage="/assets/images/website-images//1-8-270x300.jpg",Name="Doublefile Viburnum",Price="$67.45"},
            new Product{FirstImage="/assets/images/website-images//1-8-270x300.jpg",SecondImage="/assets/images/website-images//1-1-270x300.jpg",Name="Feather Reed Grass",Price="$20.00"},

        };
        
        public IActionResult Index()
        {
            ViewBag.Products = products;
            return View(shippings);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
