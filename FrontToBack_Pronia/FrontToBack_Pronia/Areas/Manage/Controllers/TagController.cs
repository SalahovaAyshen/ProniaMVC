using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        public TagController(AppDbContext context)
        {
            _context = context;
            
        }
        public async Task<IActionResult> Index()
        {
            List<Tag> tags =await _context.Tags.Include(t=>t.ProductTags).ToListAsync();
            return View(tags);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Tags.Any(t => t.Name == tag.Name);
            if (result)
            {
                ModelState.AddModelError("Name", "This tag already exists");
                return View();
            }

            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return Redirect(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (tag == null) return NotFound();
            return View(tag);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Tag tag)
        {
            if (!ModelState.IsValid) return View();
            Tag exist = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (exist == null) return NotFound();
            bool result = await _context.Tags.AnyAsync(c => c.Name == tag.Name && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists");
                return View(exist);
            }
            exist.Name = tag.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Tag exist = await _context.Tags.FirstOrDefaultAsync(c => c.Id == id);
            if (exist == null) return NotFound();
            _context.Tags.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
