using FrontToBack_Pronia.Areas.Manage.ViewModels;
using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    [AutoValidateAntiforgeryToken]
    public class ColorController : Controller
    {
        private readonly AppDbContext _context;
        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        public async Task<IActionResult> Index(int page=1)
        {
            int count = await _context.Colors.CountAsync();
            List<Color> colors = await _context.Colors.Skip((page-1)*3).Take(3).Include(c=>c.ProductColors).ToListAsync() ;

            PaginationVM<Color> pagination = new PaginationVM<Color>
            {
                Items = colors,
                TotalPage = Math.Ceiling((double)count / 3),
                CurrentPage = page
            };
            return View(pagination);
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Colors.Any(c => c.Name.Trim() == colorVM.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This color already exists");
                return View();
            }
            Color color = new Color
            {
                Name = colorVM.Name,
            };

            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
            return Redirect(nameof(Index));
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Update 
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Color color =await _context.Colors.FirstOrDefaultAsync(c=>c.Id== id);
            if(color == null) return NotFound();
            UpdateColorVM colorVM = new UpdateColorVM
            {
                Name = color.Name,
            };
            return View(colorVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
        {
            if(!ModelState.IsValid) return View();
            Color exist = await _context.Colors.FirstOrDefaultAsync(c=>c.Id == id);
            if(exist == null) return NotFound();
            bool result = await _context.Colors.AnyAsync(c=>c.Name.Trim() == colorVM.Name.Trim() && c.Id!=id);
            if (result)
            {
                ModelState.AddModelError("Name", "This color name already exists");
                return View();
            }
            exist.Name=colorVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = nameof(UserRole.Admin))]
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
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Detail
        public async Task<IActionResult> Detail(int id)
        {
            if(id<=0) return BadRequest();
            Color color = await _context.Colors.Include(c=>c.ProductColors).ThenInclude(c=>c.Product).FirstOrDefaultAsync(c=>c.Id==id);
            return View(color);
        }
    }
}
