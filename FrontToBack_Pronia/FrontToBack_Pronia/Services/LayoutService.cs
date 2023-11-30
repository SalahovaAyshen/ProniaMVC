using FrontToBack_Pronia.DAL;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Services
{
    public class LayoutService
    {
        public AppDbContext _context { get; set; }
        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string,string>> GetSettingsAsync()
        {
            Dictionary<string,string> settings = await _context.Settings.ToDictionaryAsync(s=>s.Key, s=>s.Value);
            return settings;
        }
    }
}
