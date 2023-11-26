﻿using FrontToBack_Pronia.Areas.Manage.ViewModels;
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

        //Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateSizeVM sizeVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result =await _context.Sizes.AnyAsync(s => s.Name.Trim() == sizeVM.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This size already exists");
                return View();
            }
            Size size = new Size { Name = sizeVM.Name };

            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (size == null) return NotFound();
            UpdateSizeVM sizeVM = new UpdateSizeVM
            {
                Name= size.Name,
            };
            return View(sizeVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateSizeVM sizeVM)
        {
            if (!ModelState.IsValid) return View();
            Size exist = await _context.Sizes.FirstOrDefaultAsync(s=>s.Id == id);
            if(exist == null) return NotFound();
            bool result = await _context.Sizes.AnyAsync(s => s.Name.Trim() == sizeVM.Name.Trim() && s.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "This size name already exists");
                return View();
            }
            exist.Name= sizeVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            if(id<=0) return BadRequest();
            Size exist = await _context.Sizes.FirstOrDefaultAsync(s => s.Id == id);
            if (exist == null) return NotFound();
            _context.Sizes.Remove(exist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Detail
        public async Task<IActionResult> Detail(int id)
        {
            if (id <= 0) return BadRequest();
            Size size = await _context.Sizes.Include(s=>s.ProductSizes).ThenInclude(s=>s.Products).FirstOrDefaultAsync(s=>s.Id==id);
            if (size==null) return NotFound();
            return View(size);
        }
      
    }
}
