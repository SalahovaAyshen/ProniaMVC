using FrontToBack_Pronia.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => 
opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]));
var app = builder.Build();
app.UseStaticFiles();
app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
    );

app.Run();
