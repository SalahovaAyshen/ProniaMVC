using FrontToBack_Pronia.Areas.Manage.ViewModels;
using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities;
using FrontToBack_Pronia.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    [AutoValidateAntiforgeryToken]

    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SliderController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        public async Task<IActionResult> Index()
        {
            List<Slider> slider = await _context.Sliders.ToListAsync();
            return View(slider);
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Create
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateSliderVM sliderVM) 
        {
            if (!ModelState.IsValid) return View();
            if(sliderVM.Order<0 || await _context.Sliders.FirstOrDefaultAsync(s=>s.Order==sliderVM.Order) is not null)
            {
                ModelState.AddModelError("Order", "Order is negative number or already existed");
                return View();
            }
            if (!sliderVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError("Photo", "Photo must be image type");
                return View();
            }
            if (!sliderVM.Photo.VaidateSize(2*1024))
            {
                ModelState.AddModelError("Photo", "Photo can't be more than 2mb");
                return View();
            }
           string fileName=await sliderVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
            Slider slider = new Slider
            {
                Title = sliderVM.Title,
                Offer = sliderVM.Offer,
                Description = sliderVM.Description,
                ImageURL = fileName,
                Button = sliderVM.Button,
                Order = sliderVM.Order
            };
            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = nameof(UserRole.Admin))]
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

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (existed == null) return NotFound();
            UpdateSliderVM sliderVM = new UpdateSliderVM
            {
                Title = existed.Title,
                Offer = existed.Offer,
                Description = existed.Description,
                Button = existed.Button,
                Order = existed.Order,
                ImageURL = existed.ImageURL
            };
            return View(sliderVM);

        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateSliderVM sliderVM)
        {
  
            if (!ModelState.IsValid) return View(sliderVM);
            Slider existed = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
           
            if (sliderVM.Photo != null)
            {
                if (!sliderVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("photo", "photo must be image type");
                    return View(sliderVM);
                }

                if (!sliderVM.Photo.VaidateSize(2 * 1024))
                {
                    ModelState.AddModelError("photo", "photo can't be more than 2mb");
                    return View(sliderVM);
                }

                string filename = await sliderVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
                existed.ImageURL.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
                existed.ImageURL = filename;
            }

            existed.Title=sliderVM.Title;
            existed.Offer=sliderVM.Offer;
            existed.Order=sliderVM.Order;
            existed.Description=sliderVM.Description;
            existed.Button=sliderVM.Button;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        public async Task<IActionResult> Detail(int id)
        {
            Slider slider = await _context.Sliders.FirstOrDefaultAsync(x => x.Id == id);
            return View(slider);
        }
    }
}
