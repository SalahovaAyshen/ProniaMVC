using FrontToBack_Pronia.Models;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.DAL
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Featured> Featureds { get; set; }
        public DbSet<LatestProducts> Latests { get; set; }
        public DbSet<Slider> Sliders { get; set; }
    }
}
