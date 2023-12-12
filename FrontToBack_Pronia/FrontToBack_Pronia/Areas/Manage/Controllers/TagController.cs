using FrontToBack_Pronia.Areas.Manage.ViewModels;
using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    [AutoValidateAntiforgeryToken]

    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        public TagController(AppDbContext context)
        {
            _context = context;
            
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        public async Task<IActionResult> Index(int page =1)
        {
            int count = await _context.Tags.CountAsync();
            List<Tag> tags =await _context.Tags.Skip((page-1)*3).Take(3).Include(t=>t.ProductTags).ToListAsync();

            PaginationVM<Tag> pagination = new PaginationVM<Tag>
            {
                Items = tags,
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
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result =await _context.Tags.AnyAsync(t => t.Name.Trim() == tagVM.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This tag already exists");
                return View();
            }
            Tag tag = new Tag
            {
                Name = tagVM.Name
            };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return Redirect(nameof(Index));
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();
            UpdateTagVM tagVM = new UpdateTagVM
            {
                Name= tag.Name,
            };
            return View(tagVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid) return View();
            Tag exist = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (exist == null) return NotFound();
            bool result = await _context.Tags.AnyAsync(t => t.Name.Trim() == tagVM.Name.Trim() && t.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This tag name already exists");
                return View(tagVM);
            }
            exist.Name = tagVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = nameof(UserRole.Admin))]
        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Tag exist = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (exist == null) return NotFound();
            _context.Tags.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Detail
        public async Task<IActionResult> Detail(int id)
        {
            if(id<=0) return BadRequest();
            Tag tag = await _context.Tags.Include(t=>t.ProductTags).ThenInclude(t=>t.Product).FirstOrDefaultAsync(t=>t.Id == id); 
            if (tag == null) return NotFound();
            return View(tag);
        }
    }
}
