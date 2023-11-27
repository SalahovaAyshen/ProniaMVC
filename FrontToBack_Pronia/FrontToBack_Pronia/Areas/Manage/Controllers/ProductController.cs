using FrontToBack_Pronia.Areas.Manage.ViewModels;
using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true))
                .Include(p=>p.Category)
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM();
            productVM.Categories = await _context.Categories.ToListAsync();
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid) return View(productVM);
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError("CategoryId", "We don't have a category with this id");
                return View(productVM);
            }
            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                Description = productVM.Description,
                FullDescription = productVM.FullDescription,
                SKU = productVM.SKU,
                Order = productVM.Order,
                CategoryId = productVM.CategoryId,

            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products
                .Include(p=>p.ProductImages)
                .Include(p=>p.Category)
                .Include(p=>p.ProductTags)
                .ThenInclude(p=>p.Tag)
                .Include(p=>p.ProductColors)
                .ThenInclude(p=>p.Color)
                .Include(p=>p.ProductSizes)
                .ThenInclude(p=>p.Size)
                .FirstOrDefaultAsync(p => p.Id == id);
            return View(product);
        }
    }
}   
