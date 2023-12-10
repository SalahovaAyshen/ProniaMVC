using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Interfaces;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Security.Claims;

namespace FrontToBack_Pronia.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        private IEmailService _emailService { get; }

        public BasketController(AppDbContext context, UserManager<AppUser> userManager,IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<IActionResult> Index()
        {
            List<BasketItemVM> basketItemVMs = new List<BasketItemVM>();
            if (User.Identity.IsAuthenticated)
            {

                AppUser user = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
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
            if (product == null) return NotFound();


            if (User.Identity.IsAuthenticated)
            {

                AppUser user = await _userManager.Users
                    .Include(u => u.BasketItems.Where(bi => bi.OrderId == null))
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
                BasketItem basketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.Id == id && b.OrderId==null);
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
                BasketItem basketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.Id == id && b.OrderId == null);
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
                BasketItem basketItem = await _context.BasketItems.FirstOrDefaultAsync(b => b.Id == id && b.OrderId == null);
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
        [Authorize(Roles =nameof(UserRole.Member))]
        public async Task<IActionResult> Checkout()
        {
            AppUser user = await _userManager.Users
                .Include(u=>u.BasketItems.Where(bi => bi.OrderId == null))
                .ThenInclude(bi=>bi.Product)
                .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));

            OrderItemVM orderItemVM = new OrderItemVM
            {
               BasketItems = user.BasketItems

            };
            return View(orderItemVM);
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(OrderItemVM orderItemVM)
        {
            AppUser user = await _userManager.Users
                .Include(u => u.BasketItems.Where(bi=>bi.OrderId==null))
                .ThenInclude(bi => bi.Product)
                .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (!ModelState.IsValid)
            {
                orderItemVM.BasketItems = user.BasketItems;
                return View(orderItemVM);
            }
            decimal total = 0;
            foreach (BasketItem item in user.BasketItems)
            {
                item.Price = item.Product.Price;
                total += item.Price * item.Count;

            }
            Order order = new Order
            {
                Address = orderItemVM.Address,
                Status = null,
                AppUserId = user.Id,
                BasketItems = user.BasketItems,
                TotalPrice = total,
                PurchasedAt=DateTime.Now,

            };

            string body = @"
             <h3>Ur order successfully placed</h3>
                         <table border=""1"">
                             <thead>
                                  <tr>
                                     <th> Name </th>
                                     <th> Price </th>
                                     <th> Count </th>
                                  </tr>
                            </thead>
                                <tbody> ";
            foreach (var item in order.BasketItems)
            {
                body+=@$"  <tr>
                              <td>{item.Product.Name}</td>
                              <td>{item.Price}</td>
                              <td>{item.Count}</td>
                          </tr>";
            }
            body += @"   </tbody>
                      </table>";
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await _emailService.SendEmailAsync(user.Email, "Ur Order", body,true);
            return RedirectToAction("Index","Home");
        }
    }
}

//<table>
//    <thead>
//        <tr>
//            <th> Name </th>
//            <th> Price </th>
//            <th> Count </th>
//        </tr>
//    </thead>
//    <tbody>
//        <tr>
//            <td></td>
//            <td></td>
//            <td></td>
//        </tr>
//    </tbody>
//</table>
