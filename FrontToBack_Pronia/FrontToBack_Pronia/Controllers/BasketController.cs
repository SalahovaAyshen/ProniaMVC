using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FrontToBack_Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketItemVMs = new List<BasketItemVM>();
            if (Request.Cookies["Basket"] != null)
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                if (cookies != null)
                {
                    foreach (BasketCookieItemVM cookie in cookies)
                    {

                        Product product = await _context.Products.Include(p=>p.ProductImages.Where(pi=>pi.IsPrimary==true)).FirstOrDefaultAsync(p => p.Id == cookie.Id);
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
            return View(basketItemVMs);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if(product == null) return NotFound();

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
                BasketCookieItemVM existed = basket.FirstOrDefault(b=>b.Id == id);
                if(existed == null)
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
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> GetBasket()
        {
            return Content(Request.Cookies["Basket"]);
        }

        public async Task<IActionResult> Minus(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            List<BasketItemVM> basket = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies["Basket"]);
                
            BasketItemVM existed = basket.FirstOrDefault(b => b.Id == id);

                if (existed.Count ==1 || existed.Count ==0)
                {
                   
                    basket.Remove(existed);
                }
                else
                {
                    existed.Count--;
                }
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            List<BasketItemVM> basket = JsonConvert.DeserializeObject<List<BasketItemVM>>(Request.Cookies["Basket"]);
            BasketItemVM existed = basket.FirstOrDefault(b => b.Id == id);
            basket.Remove(existed);
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index));
        }

    }
}
