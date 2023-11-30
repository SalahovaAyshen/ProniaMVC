using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        public AppDbContext _context { get; set; }
        public HeaderViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key,s=>s.Value);
            return View(settings);
        }
    }
}
