using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using System.Security.Claims;

namespace FrontToBack_Pronia.Controllers
{
 
    public class WishListController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public WishListController(AppDbContext context, UserManager<AppUser> userManager)
        {
                _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<WishListItemVM> wishListItemVMs = new List<WishListItemVM>();

            if (Request.Cookies["WishList"] != null)
            {
                List<WishListCookieItemVM> cookies = JsonConvert.DeserializeObject<List<WishListCookieItemVM>>(Request.Cookies["WishList"]);
                if (cookies != null)
                {
                    foreach (WishListCookieItemVM cookie in cookies)
                    {

                        Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == cookie.Id);
                        if (product != null)
                        {
                            WishListItemVM wishListItemVM = new WishListItemVM
                            {
                                Id = product.Id,
                                Name = product.Name,
                                Price = product.Price,
                                Image = product.ProductImages[0].ImageUrl,
                            };
                            wishListItemVMs.Add(wishListItemVM);
                        }

                    }
                }
            }
            return View(wishListItemVMs);
        }

        public async Task<IActionResult> AddToWishList(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            List<WishListCookieItemVM> wishList = new List<WishListCookieItemVM>();
          
            if (Request.Cookies["WishList"] != null)
            {
               wishList = JsonConvert.DeserializeObject<List<WishListCookieItemVM>>(Request.Cookies["WishList"]);
            }

            WishListCookieItemVM existed = wishList.FirstOrDefault(b => b.Id == id);
           
                WishListCookieItemVM wishListCookieItemVM = new WishListCookieItemVM
                {
                    Id = id,
                };
                wishList.Add(wishListCookieItemVM);
            
            string json = JsonConvert.SerializeObject(wishList);
            Response.Cookies.Append("WishList", json);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Delete(int id)
        {
            List<WishListItemVM> wishList = JsonConvert.DeserializeObject<List<WishListItemVM>>(Request.Cookies["WishList"]);
            WishListItemVM existed = wishList.FirstOrDefault(b => b.Id == id);
            wishList.Remove(existed);
            string json = JsonConvert.SerializeObject(wishList);
            Response.Cookies.Append("WishList", json);
        
            return RedirectToAction(nameof(Index));
    }
    }
}
