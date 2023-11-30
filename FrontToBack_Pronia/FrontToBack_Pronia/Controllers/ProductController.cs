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
     

        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Product product =await _context.Products
                .Include(p => p.ProductImages)
                .Include(p=>p.Category)
                .Include(p => p.ProductTags)
                .ThenInclude(p => p.Tag)
                .Include(p=>p.ProductColors)
                .ThenInclude(p=>p.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(p=>p.Size)
                .FirstOrDefaultAsync(x => x.Id == id);
            if(product == null) return NotFound();

            
            List<Product> products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p=>p.CategoryId==product.CategoryId && p.Id!=product.Id)
                .ToListAsync();


            ProductVM productVM = new ProductVM
            {
                Products = product,
                ProductList = products
            };
            return View(productVM);
        }
    }
}
