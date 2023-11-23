using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        public SliderController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> slider = await _context.Sliders.ToListAsync();
            return View(slider);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slider slider) 
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!slider.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Photo must be image type");
                return View();
            }

            if (slider.Photo.Length > 2*1024*1024)
            {
                ModelState.AddModelError("Photo", "Photo can't be more than 2mb");
                return View();
            }
            FileStream file = new FileStream(@"C:\Users\hp\Desktop\FTB_13.11\FrontToBack_Pronia\FrontToBack_Pronia\wwwroot\assets\images\slider\bg\" + slider.Photo.FileName, FileMode.Create);
            slider.Photo.CopyTo(file);

            slider.ImageURL = slider.Photo.FileName;

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
