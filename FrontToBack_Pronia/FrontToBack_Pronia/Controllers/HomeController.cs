using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            List<Featured> featureds = _context.Featureds.ToList();
            List<LatestProducts> latests = _context.Latests.OrderByDescending(l=>l.Id).Take(8).ToList();
            List<Slider>sliders = _context.Sliders.ToList();
            List<LatestProducts> newproducts = _context.Latests.Take(4).ToList();
            HomeVM homeVM = new HomeVM 
            {
                Shipping = shippings,
                Featured = featureds,
                Latest= latests,
                Slider= sliders,
                NewProduct= newproducts
            };
            return View(homeVM);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
