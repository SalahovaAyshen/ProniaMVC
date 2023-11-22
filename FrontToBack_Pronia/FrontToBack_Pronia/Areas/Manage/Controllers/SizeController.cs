using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;
        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> sizes =await _context.Sizes.Include(s=>s.ProductSizes).ToListAsync();
            return View(sizes);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool resilt = _context.Sizes.Any(s => s.Name == size.Name);
            if (resilt)
            {
                ModelState.AddModelError("Name", "This size already exists");
                return View();  
            }
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return Redirect(nameof(Index));
        }
    }
}
