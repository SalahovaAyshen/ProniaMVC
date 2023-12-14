using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search, int? order)
        {
            IQueryable<Product> query = _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary != null)).AsQueryable();

            switch (order)
            {
                case 1:
                    query = query.OrderBy(q => q.Name);
                     break;
                case 2:
                    query = query.OrderBy(q => q.Price);
                    break;
                case 3:
                    query = query.OrderByDescending(q => q.Price);
                    break;
                case 4:
                    query = query.OrderByDescending(q => q.Id);
                    break;

            }
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(q => q.Name.ToLower().Contains(search.ToLower()));
            }
            ShopVM shopVM = new ShopVM
            {
                Products = await query.ToListAsync(),
                Categories = await _context.Categories.Include(c => c.Products).ToListAsync(),
            };
            return View(shopVM);
        }
    }
}
