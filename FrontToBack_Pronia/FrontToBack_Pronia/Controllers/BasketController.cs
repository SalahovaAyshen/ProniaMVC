using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FrontToBack_Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public BasketController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketItemVMs = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {

                AppUser user = await _userManager.Users
                    .Include(u => u.BasketItems)
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
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
            }
            else
            {
                if (Request.Cookies["Basket"] != null)
                {
                    List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    if (cookies != null)
                    {
                        foreach (BasketCookieItemVM cookie in cookies)
                        {

                            Product product = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == cookie.Id);
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

            }
           
            return View(basketItemVMs);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if(product == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(u=>u.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                BasketItem basketItem = user.BasketItems.FirstOrDefault(bi => bi.ProductId == product.Id);
                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        AppUserId = user.Id,
                        ProductId = product.Id,
                        Price = product.Price,
                        Count = 1
                    };
                    user.BasketItems.Add(basketItem);
                }
                else
                {
                    basketItem.Count++;
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketCookieItemVM> basket;
                if (Request.Cookies["Basket"] == null)
                {
                    basket = new List<BasketCookieItemVM>();
                    BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };

                    basket.Add(basketCookieItemVM);
                }
                else
                {
                    basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                    BasketCookieItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                    if (existed == null)
                    {
                        BasketCookieItemVM basketCookieItemVM = new BasketCookieItemVM
                        {
                            Id = id,
                            Count = 1
                        };
                        basket.Add(basketCookieItemVM);
                    }
                    else
                    {
                        existed.Count++;
                    }
                }

                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Basket", json);
            }

         

            //List<BasketItemVM> basketItemVMs = new List<BasketItemVM>();


            //        foreach (BasketCookieItemVM cookie in basket)
            //        {

            //            Product newProduct = await _context.Products.Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true)).FirstOrDefaultAsync(p => p.Id == cookie.Id);
            //            if (newProduct != null)
            //            {
            //                BasketItemVM basketItemVM = new BasketItemVM
            //                {
            //                    Id = newProduct.Id,
            //                    Name = newProduct.Name,
            //                    Price = newProduct.Price,
            //                    Image = newProduct.ProductImages[0].ImageUrl,
            //                    Count = cookie.Count,
            //                    Subtotal = newProduct.Price * cookie.Count,
            //                };
            //                basketItemVMs.Add(basketItemVM);
            //            }

            //        }



            //return PartialView("ProductPV",basketItemVMs);
            return RedirectToAction("Index", "Home");
        }

       
        public async Task<IActionResult> Minus(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            if (User.Identity.IsAuthenticated)
            {
                BasketItem basketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.Id == id);
                if (basketItem.Count <= 1)
                {
                    _context.Remove(basketItem);
                }
                else
                {
                    basketItem.Count--;
                }
                await _context.SaveChangesAsync();  
            }
            else
            {
                List<BasketItemVM> basket = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies["Basket"]);

                BasketItemVM existed = basket.FirstOrDefault(b => b.Id == id);

                if (existed.Count == 1 || existed.Count == 0)
                {

                    basket.Remove(existed);
                }
                else
                {
                    existed.Count--;
                }
                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Basket", json);
            }
          
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Plus(int id)
        {
            if (id <= 0) return BadRequest();
            Product book = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (book == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                BasketItem basketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.Id == id);
                basketItem.Count++;
                await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketItemVM> basket = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies["Basket"]);
                BasketItemVM existed = basket.FirstOrDefault(b => b.Id == id);

                if (existed.Id == id)
                {
                    existed.Count++;
                }


                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Basket", json);
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            if (User.Identity.IsAuthenticated)
            {
                BasketItem basketItem = await _context.BasketItems.FirstOrDefaultAsync(b=>b.Id == id);
                    _context.BasketItems.Remove(basketItem);
                    await _context.SaveChangesAsync();
            }
            else
            {
                List<BasketItemVM> basket = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies["Basket"]);
                BasketItemVM existed = basket.FirstOrDefault(b => b.Id == id);
                basket.Remove(existed);
                string json = JsonConvert.SerializeObject(basket);
                Response.Cookies.Append("Basket", json);
            }
           
            return RedirectToAction(nameof(Index));
        }

    }
}
