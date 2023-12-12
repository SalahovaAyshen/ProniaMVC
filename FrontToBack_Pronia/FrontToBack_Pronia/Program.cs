using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Interfaces;
using FrontToBack_Pronia.Middlewares;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => 
opt.UseSqlServer(builder.Configuration["ConnectionStrings:Default"]));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz0123456789_.";
    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(7);

    options.SignIn.RequireConfirmedEmail = true;
}

).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders(); 


builder.Services.AddSingleton<IHttpContextAccessor,HttpContextAccessor>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddScoped<LayoutService>();
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleWare>();

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
