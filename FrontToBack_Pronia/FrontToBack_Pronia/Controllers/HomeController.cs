﻿using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
            
        }


        public async Task<IActionResult> Index()
        {
            List<Shipping> shippings =await _context.Shippings.ToListAsync();
            List<Slider> sliders =await _context.Sliders.ToListAsync();
            List<Product> featureds =await _context.Products
                .OrderBy(f => f.Order % 2 == 1)
                .Take(8).Include(f => f.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
            List<Product> latests =await _context.Products
                .OrderByDescending(l => l.Order)
                .Take(8)
                .Include(f => f.ProductImages.Where(pi=>pi.IsPrimary!=null)).ToListAsync();
            List<Product> newproducts =await _context.Products
                .OrderByDescending(l => l.Order).Take(4)
                .Include(f => f.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();

            HomeVM homeVM = new HomeVM 
            {
                Shipping = shippings,
                Slider = sliders,
                Featured = featureds,
                Latest = latests,
                NewProduct = newproducts
            };
            return View(homeVM);
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
