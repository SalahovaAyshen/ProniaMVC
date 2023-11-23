using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slider> slider = await _context.Sliders.ToListAsync();
            return View(slider);
        }

        //Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slider slider) 
        {
            if (slider.Photo is null)
            {
                ModelState.AddModelError("Photo", "Photo must be chosen");
                return View();
            }
            if (!slider.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "Photo must be image type");
                return View();
            }
            if (!slider.Photo.VaidateSize(2*1024))
            {
                ModelState.AddModelError("Photo", "Photo can't be more than 2mb");
                return View();
            }
            slider.ImageURL =await slider.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (existed == null) return NotFound();

            existed.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
           
            _context.Sliders.Remove(existed);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (existed == null) return NotFound();
            return View(existed);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Slider slider)
        {
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s=>s.Id==id);
            if (existed is null) return NotFound();
            if (ModelState.IsValid) return View(existed);

            if (slider.Photo != null)
            {
                if (!slider.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("photo", "photo must be image type");
                    return View(existed);
                }

                if (!slider.Photo.VaidateSize(2 * 1024))
                {
                    ModelState.AddModelError("photo", "photo can't be more than 2mb");
                    return View(existed);
                }

                string filename = await slider.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
                existed.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
                existed.ImageURL = filename;
            }

            existed.Title=slider.Title;
            existed.Offer=slider.Offer;
            existed.Order=slider.Order;
            existed.Description=slider.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Detail(int id)
        {
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);
            return View(slider);
        }
    }
}
