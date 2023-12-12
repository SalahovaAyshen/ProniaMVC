﻿using FrontToBack_Pronia.Areas.Manage.ViewModels;
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
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        public async Task<IActionResult> Index(int page=1)
        {
            int count = await _context.Categories.CountAsync();
            List<Category> categories = await _context.Categories.Skip((page-1)*3).Take(3).Include(c => c.Products).ToListAsync();

            PaginationVM<Category> pagination = new PaginationVM<Category>
            {
                Items = categories,
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
        public async Task<IActionResult> Create(CreateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = _context.Categories.Any(c => c.Name.Trim() == categoryVM.Name.Trim());
            if (result)
            {
                ModelState.AddModelError("Name", "This category already exists");
                return View();
            }
            Category category = new Category
            {
                Name = categoryVM.Name,
            };

           await _context.Categories.AddAsync(category);
           await _context.SaveChangesAsync();
            return Redirect(nameof(Index)); 
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Category category=await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            UpdateCategoryVM categoryVM = new UpdateCategoryVM
            {
                Name = category.Name,
            };
            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View();
            Category exist = await _context.Categories.FirstOrDefaultAsync(c=>c.Id == id);
            if (exist == null) return NotFound();
            bool result = await _context.Categories.AnyAsync(c => c.Name.Trim() == categoryVM.Name.Trim() && c.Id != id);
            if (result)
            {
                ModelState.AddModelError("Name", "Already exists");
                return View(categoryVM);
            }
            exist.Name = categoryVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = nameof(UserRole.Admin))]
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
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Detail
        public async Task<IActionResult> Detail(int id)
        {
           if (id<=0) return BadRequest();
           Category category = await _context.Categories.Include(c=>c.Products).ThenInclude(c=>c.ProductImages).FirstOrDefaultAsync(c=>c.Id==id);
            if (category == null) return NotFound();
            return View(category);
        }
    }
}
