using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = _context.Products.Include(p => p.ProductImages).Include(p=>p.Category).FirstOrDefault(x => x.Id == id);
            if(product is null) return NotFound();

            
            List<Product> products =_context.Products.Include(p => p.Category).Include(p => p.ProductImages).Where(p=>p.CategoryId==product.CategoryId && p.Id!=product.Id).ToList();


            ProductVM productVM = new ProductVM
            {
                Products = product,
                ProductList = products,
               

            };
            return View(productVM);
        }
    }
}
