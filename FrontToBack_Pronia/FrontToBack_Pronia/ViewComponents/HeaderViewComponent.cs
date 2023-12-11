using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace FrontToBack_Pronia.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        public AppDbContext _context { get; set; }
        private readonly IHttpContextAccessor _http;
        private readonly UserManager<AppUser> _userManager;
        public HeaderViewComponent(AppDbContext context, IHttpContextAccessor http, UserManager<AppUser> userManager)
        {
            _context = context;
            _http = http;
            _userManager = userManager;

        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
            List<BasketItemVM> basketItemVMs = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
               .Include(u => u.BasketItems)
               .ThenInclude(bi => bi.Product)
               .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
               .FirstOrDefaultAsync(u => u.Id == _http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                foreach (BasketItem item in user.BasketItems)
                {
                    basketItemVMs.Add(new BasketItemVM
                    {
                        Id = item.Id,
                        Price = item.Product.Price,
                        Count = item.Count,
                        Name = item.Product.Name,
                        Subtotal = item.Count * item.Product.Price,
                        Image = item.Product.ProductImages[0].ImageUrl

                    });
                }
                await _context.SaveChangesAsync();
            }
            else
            {

                if (_http.HttpContext.Request.Cookies["Basket"] != null)
                {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(_http.HttpContext.Request.Cookies["Basket"]);

                    foreach (BasketCookieItemVM cookie in cookies)
                        {

                            Product product = await _context.Products
                                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                                .FirstOrDefaultAsync(p => p.Id == cookie.Id);
                            if (product != null)
                            {
                                BasketItemVM basketItemVM = new BasketItemVM
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    Price = product.Price,
                                    Image = product.ProductImages[0].ImageUrl,
                                    Count = cookie.Count,
                                    Subtotal = product.Price * cookie.Count,
                                };
                                basketItemVMs.Add(basketItemVM);
                            }

                        }
                    



                }
            }
               
                HeaderAndBasketVM headerAndBasketVM = new HeaderAndBasketVM
                {
                    Header = settings,
                    Basket = basketItemVMs

                };
                return View(headerAndBasketVM);
            }
        }
    }
