using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
            
        }


        public IActionResult Index()
        {
            List<Shipping> shippings = _context.Shippings.ToList();
            List<Slider> sliders = _context.Sliders.ToList();
            List<Product> featureds = _context.Products.OrderBy(f=>f.Order%2==1).Take(8).Include(f => f.ProductImages).ToList();
            List<Product> latests = _context.Products.OrderByDescending(l => l.Order).Take(8).Include(f => f.ProductImages).ToList();
            List<Product> newproducts = _context.Products.OrderByDescending(l => l.Order).Take(4).Include(f => f.ProductImages).ToList();

            HomeVM homeVM = new HomeVM 
            {
                Shipping = shippings,
                Slider = sliders,
                Featured = featureds,
                Latest = latests,
                NewProduct = newproducts
            };
            return View(homeVM);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
