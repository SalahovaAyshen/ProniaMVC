using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => 
opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]));
builder.Services.AddScoped<LayoutService>();
var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "area",
        pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
        );
}); 
app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
