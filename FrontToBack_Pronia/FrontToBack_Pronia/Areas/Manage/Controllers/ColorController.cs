using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;
        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors.Include(c=>c.ProductColors).ToListAsync() ;
            return View(colors);
        }

        //Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Colors.Any(c => c.Name.Trim() == color.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This color already exists");
                return View();
            }
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
            return Redirect(nameof(Index));
        }

        //Update 
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Color color =await _context.Colors.FirstOrDefaultAsync(c=>c.Id== id);
            if(color == null) return NotFound();
            return View(color);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, Color color)
        {
            if(!ModelState.IsValid) return View();
            Color exist = await _context.Colors.FirstOrDefaultAsync(c=>c.Id == id);
            if(exist == null) return NotFound();
            bool result = await _context.Colors.AnyAsync(c=>c.Name.Trim() == color.Name.Trim() && c.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "This color name already exists");
                return View();
            }
            exist.Name=color.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();
            Color color=await _context.Colors.FirstOrDefaultAsync(c=>c.Id==id);
            if(color==null) return NotFound();
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
