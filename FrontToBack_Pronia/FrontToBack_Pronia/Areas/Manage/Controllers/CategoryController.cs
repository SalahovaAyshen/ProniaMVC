using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Category> categories = await _context.Categories.Include(c => c.Products).ToListAsync();

            return View(categories);
        }

        //Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Categories.Any(c => c.Name.Trim() == category.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }
           await _context.Categories.AddAsync(category);
           await _context.SaveChangesAsync();
            return Redirect(nameof(Index)); 
        }

        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category category=await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Category category)
        {
            if (!ModelState.IsValid) return View();
            Category exist = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == id);
            if (exist == null) return NotFound();
            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == category.Name.Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists");
                return View(exist);
            }
            exist.Name = category.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();
            Category exist =await _context.Categories.FirstOrDefaultAsync(c=> c.Id == id);
            if (exist == null) return NotFound();
            _context.Categories.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Detail
        public async Task<IActionResult> Detail(int id)
        {
            if (id<=0) return BadRequest();
            Category category = await _context.Categories.Include(c=>c.Products).ThenInclude(c=>c.ProductImages).FirstOrDefaultAsync(c=>c.Id==id);

           // List<Product> products = await _context.Products
           //     .Where(p=>p.CategoryId==id)
           //     .Include(p => p.ProductImages)
           //     .Include(p => p.Category)
           //     .Include(p => p.ProductTags)
           //     .ThenInclude(p => p.Tag)
           //     .Include(p => p.ProductColors)
           //     .ThenInclude(p => p.Color)
           //     .Include(p => p.ProductSizes)
           //     .ThenInclude(p => p.Size).ToListAsync();
           return View(category);
        }
    }
}
