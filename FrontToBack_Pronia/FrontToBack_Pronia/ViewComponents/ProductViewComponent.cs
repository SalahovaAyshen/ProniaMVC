﻿using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        public AppDbContext _context { get; set; }
        public ProductViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(int key = 1)
        {
            List<Product> products;
            switch (key)
            {
                case 1:
                    products = await _context.Products.OrderBy(p => p.Name).Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;
                case 2:
                    products = await _context.Products.OrderByDescending(p=>p.Price).Take(8).Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary!=null)).ToListAsync();
                    break;
                case 3:
                    products = await _context.Products.OrderByDescending(p => p.Id).Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;

                default:
                    products = await _context.Products.Take(8).Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).ToListAsync();
                    break;
            }
            return View(products);
        }
    }
}
