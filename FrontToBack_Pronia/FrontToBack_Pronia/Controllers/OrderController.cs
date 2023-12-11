using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Interfaces;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities.Enums;
using FrontToBack_Pronia.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FrontToBack_Pronia.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private IEmailService _emailService { get; }


        public OrderController(AppDbContext context, UserManager<AppUser> userManager, IEmailService emailService)
        {
           _context = context;
           _userManager = userManager;
            _emailService = emailService;
        }
        public async Task<IActionResult> Checkout()
        {
            OrderItemVM orderItemVM = new OrderItemVM();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(u=>u.BasketItems)
                    .FirstOrDefaultAsync(x=>x.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));
                orderItemVM.Name = user.Name;
                orderItemVM.Surname= user.Surname;
                orderItemVM.Email = user.Email;

                foreach (BasketItem item in user.BasketItems)
                {
                    Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        orderItemVM.CheckoutItems.Add(new CheckoutItemVM
                        {
                            Name = product.Name,
                            Price = product.Price,
                            Count = item.Count,
                        });
                    }
                }
            }
            else 
            {
                string json = Request.Cookies["Basket"];
                if (json != null)
                {
                    List<BasketCookieItemVM> basketCookieItemVMs = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(json);
                    foreach (var item in basketCookieItemVMs)
                    {
                        Product product = await _context.Products
                            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                            .FirstOrDefaultAsync(p => p.Id == item.Id);
                        if (product != null)
                        {
                            orderItemVM.CheckoutItems.Add(new CheckoutItemVM
                            {
                                Name = product.Name,
                                Price = product.Price,
                                Count = item.Count,
                            });
                        }

                    }

                }
            }



            return View(orderItemVM);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(OrderItemVM orderItemVM)
        {
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                   .Include(u => u.BasketItems)
                   .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (!ModelState.IsValid)
                {
                    orderItemVM.Name = user.Name;
                    orderItemVM.Surname = user.Surname;
                    orderItemVM.Email = user.Email;

                    foreach (BasketItem item in user.BasketItems)
                    {
                        Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId);
                        if (product != null)
                        {
                            orderItemVM.CheckoutItems.Add(new CheckoutItemVM
                            {
                                Name = product.Name,
                                Price = product.Price,
                                Count = item.Count,
                            });
                        }
                    }
                    return View(orderItemVM);
                }

                Order order = new Order
                {
                    AppUserId = user.Id,
                    Name = user.Name,
                    Surname = user.Surname,
                    Email = user.Email,
                    Address = orderItemVM.Address,
                    Status = OrderStatus.InAnticipation,
                    CreatedAt = DateTime.Now,
                    TotalAmount = 0,
                    OrderItems = new List<OrderItem>()
                };

                decimal total = 0;

                foreach (var item in user.BasketItems)
                {
                    Product product = await _context.Products
                            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                            .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                    total += item.Count * item.Price;

                    if (product != null)
                    {
                        order.OrderItems.Add(new OrderItem
                        {
                            Count = item.Count,
                            Price = product.Price,
                            Image = product.ProductImages[0].ImageUrl,
                            ProductId = product.Id,
                            Name = product.Name,
                            Total = item.Count * product.Price
                        });
                    }

                }
               

                order.TotalAmount = total;
                await _context.Orders.AddAsync(order);
                user.BasketItems = new List<BasketItem>();
                await _context.SaveChangesAsync();


            }
            else
            {
                string json = Request.Cookies["Basket"];
                List<BasketCookieItemVM> basketCookieItemVMs = new List<BasketCookieItemVM>();
                if (json != null)
                {
                    basketCookieItemVMs = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(json);
                }
                if (!ModelState.IsValid)
                {
                    foreach (var item in basketCookieItemVMs)
                    {
                        Product product = await _context.Products
                            .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                            .FirstOrDefaultAsync(p => p.Id == item.Id);
                        if (product != null)
                        {
                            orderItemVM.CheckoutItems.Add(new CheckoutItemVM
                            {
                                Name = product.Name,
                                Price = product.Price,
                                Count = item.Count,
                            });
                        }

                    }
                    return View(orderItemVM);
                }
                Order order = new Order
                {
                    Name = orderItemVM.Name,
                    Surname = orderItemVM.Surname,
                    Email = orderItemVM.Email,
                    Address = orderItemVM.Address,
                    Status = OrderStatus.InAnticipation,
                    CreatedAt = DateTime.Now,
                    TotalAmount = 0,
                    OrderItems = new List<OrderItem>()
                };

                decimal total = 0;

                if (json != null)
                {
                    foreach (var item in basketCookieItemVMs)
                    {
                        Product product = await _context.Products
                           .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                           .FirstOrDefaultAsync(p => p.Id == item.Id);
                        total += item.Count * product.Price;
                        if (product != null)
                        {
                            order.OrderItems.Add(new OrderItem
                            {
                                Count = item.Count,
                                Price = product.Price,
                                Image = product.ProductImages[0].ImageUrl,
                                ProductId = product.Id,
                                Name = product.Name,
                                Total = item.Count * product.Price
                            });
                        }
                    }
                    order.TotalAmount = total;
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();
                    Response.Cookies.Delete("Basket");
                }
               
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
