using FrontToBack_Pronia.Areas.Manage.ViewModels;
using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using FrontToBack_Pronia.Utilities;
using FrontToBack_Pronia.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]

        public async Task<IActionResult> Index(int page=1)
        {
            int count = await _context.Products.CountAsync();
          
            List<Product> products = await _context.Products.Skip((page-1)*3).Take(3)
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .Include(p => p.Category)
                .ToListAsync();

            PaginationVM<Product> pagination = new PaginationVM<Product>
            {
                Items = products,
                TotalPage = Math.Ceiling((double)count / 3),
                CurrentPage = page

            };
            return View(pagination);
        }
        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        public async Task<IActionResult> Create()
        {
            CreateProductVM productVM = new CreateProductVM();
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();

            if (!ModelState.IsValid) return View(productVM);
            bool result = await _context.Categories.AnyAsync(c => c.Id == productVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError("CategoryId", "We don't have a category with this id");
                return View(productVM);
            }

            foreach (var item in productVM.ColorIds)
            {
                if (!await _context.Colors.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("ColorIds", "Not found tag id");
                    return View(productVM);
                }
            }
            foreach (var item in productVM.SizeIds)
            {
                if (!await _context.Sizes.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("SizeIds", "Not found tag id");
                    return View(productVM);
                }
            }

            if (!productVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("MainPhoto", "Image type error");
                return View(productVM);
            }
            if (!productVM.MainPhoto.VaidateSize(500))
            {
                ModelState.AddModelError("MainPhoto", "Image size error");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError("HoverPhoto", "Image type error");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.VaidateSize(500))
            {
                ModelState.AddModelError("HoverPhoto", "Image size error");
                return View(productVM); 
            }

            ProductImage mainPhoto = new ProductImage
            {
                IsPrimary = true,
                ImageUrl = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name

            };
            ProductImage hoverPhoto = new ProductImage
            {
                IsPrimary = false,
                ImageUrl = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name
            };
            Product product = new Product
            {
                Name = productVM.Name,
                Price = productVM.Price,
                Description = productVM.Description,
                FullDescription = productVM.FullDescription,
                SKU = productVM.SKU,
                Order = productVM.Order,
                CategoryId = productVM.CategoryId,
                ProductTags = new List<ProductTag>(),
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>(),
                ProductImages = new List<ProductImage> { mainPhoto, hoverPhoto},
            };
            if(productVM.TagIds != null)
            {
                foreach (var item in productVM.TagIds)
                {
                    if (!await _context.Tags.AnyAsync(x => x.Id == item))
                    {
                        ModelState.AddModelError("TagIds", "Not found tag id");
                        return View(productVM);
                    }
                }
                foreach (int item in productVM.TagIds)
                {
                    ProductTag productTag = new ProductTag
                    {
                        TagId = item,

                    };
                    product.ProductTags.Add(productTag);
                }
            }
            
            foreach (int item in productVM.ColorIds)
            {
                ProductColor productColor = new ProductColor
                {
                    ColorId = item,

                };
                product.ProductColors.Add(productColor);
            }
            foreach (int item in productVM.SizeIds)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId = item,
                };
                product.ProductSizes.Add(productSize);
            }
            TempData["Message"] = "";
           if(productVM.AdditionalPhotos != null)
            {
                foreach (IFormFile photos in productVM.AdditionalPhotos)
                {
                    if (!photos.ValidateType("image/"))
                    {
                        TempData["Message"] = $"<div class=\"alert alert-danger\" role=\"alert\">\r\n  {photos.FileName} the file type doesn't match the required one!\r\n</div>";
                        continue;
                    }
                    if (!photos.VaidateSize(500))
                    {
                        TempData["Message"] = $"<div class=\"alert alert-danger\" role=\"alert\">\r\n  {photos.FileName} the file size doesn't match the required one\r\n</div>";
                        continue;
                    }

                    product.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        ImageUrl = await photos.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        Alternative = productVM.Name
                    });
                }
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            TempData["Message"] = $"<div class=\"alert alert-success\" role=\"alert\">\r\n  Successfully created {productVM.Name} product\r\n</div>";
            return RedirectToAction(nameof(Index));

        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products
                .Include(p => p.ProductTags)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                FullDescription = product.FullDescription,
                SKU = product.SKU,
                Order = product.Order,
                CategoryId = (int)product.CategoryId,
                TagIds = product.ProductTags.Select(pt => pt.TagId).ToList(),
                ColorIds = product.ProductColors.Select(pc => pc.ColorId).ToList(),
                SizeIds = product.ProductSizes.Select(ps => ps.SizeId).ToList(),
                ProductImages = product.ProductImages,
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                
            };
            return View(productVM);
        }

        [HttpPost]

        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products
            .Include(p => p.ProductTags)
            .Include(p => p.ProductColors)
            .Include(p => p.ProductSizes)
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id);

            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();
            productVM.ProductImages = existed.ProductImages;
        
            if (existed == null) return NotFound();
            if (!ModelState.IsValid) return View(productVM);
            if (await _context.Categories.FirstOrDefaultAsync(c => c.Id == productVM.CategoryId) is null)
            {
                ModelState.AddModelError("CategoryId", "Not found category id");
                return View(productVM);
            }

            if(productVM.MainPhoto != null)
            {
                if (!productVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "The entered photo type does not match the required one");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.VaidateSize(500))
                {
                    ModelState.AddModelError("MainPhoto", "The size of the photo is larger than required");
                    return View(productVM);
                }
            }
            if(productVM.HoverPhoto != null)
            {
                if (!productVM.HoverPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError("HoverPhoto", "The entered photo type does not match the required one");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.VaidateSize(500))
                {
                    ModelState.AddModelError("HoverPhoto", "The size of the photo is larger than required");
                    return View(productVM);
                }
            }
            //For tag
            if(productVM.TagIds != null)
            {
                existed.ProductTags.RemoveAll(pt => !productVM.TagIds.Exists(x => x == pt.TagId));
                var tagList = productVM.TagIds.Where(ti => !existed.ProductTags.Any(x => x.TagId == ti)).ToList();
                foreach (var prod in tagList)
                {
                    existed.ProductTags.Add(new ProductTag { TagId = prod });
                }
            }
            else
            {
                existed.ProductTags = new List<ProductTag>();
            }

            //For Color
            existed.ProductColors.RemoveAll(pc => !productVM.ColorIds.Exists(x => x == pc.ColorId));
            var colorList = productVM.ColorIds.Where(ci => !existed.ProductColors.Any(x => x.ColorId == ci));
            foreach (var color in colorList)
            {
                existed.ProductColors.Add(new ProductColor { ColorId = color });
            }
           

            //For Size
            existed.ProductSizes.RemoveAll(ps => !productVM.SizeIds.Exists(x => x == ps.SizeId));
            var sizeList = productVM.SizeIds.Where(si => !existed.ProductSizes.Any(x => x.SizeId == si));
            foreach (var size in sizeList)
            {
                existed.ProductSizes.Add(new ProductSize { SizeId = size });
            }
          
            if(productVM.MainPhoto != null)
            {
                string main = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage exImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                exImage.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(exImage);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    ImageUrl = main,
                    Alternative = productVM.Name
                });
            }
            if(productVM.HoverPhoto != null)
            {
                string hover = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage exImage = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                exImage.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(exImage);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = false,
                    ImageUrl = hover,
                    Alternative = productVM.Name
                });
            }
          
            if(productVM.ImageIds == null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removeable = existed.ProductImages.Where(pi => !productVM.ImageIds.Exists(i => i == pi.Id) && pi.IsPrimary == null).ToList();
            foreach (ProductImage remIm in removeable)
            {
                remIm.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(remIm);
            }

            TempData["Message"] = "";
            if (productVM.AdditionalPhotos != null)
            {
                foreach (IFormFile photos in productVM.AdditionalPhotos)
                {
                    if (!photos.ValidateType("image/"))
                    {
                        TempData["Message"] = $"<div class=\"alert alert-danger\" role=\"alert\">\r\n  {photos.FileName} the file type doesn't match the required one!\r\n</div>";
                        continue;
                    }
                    if (!photos.VaidateSize(500))
                    {
                        TempData["Message"] = $"<div class=\"alert alert-danger\" role=\"alert\">\r\n  {photos.FileName} the file size doesn't match the required one\r\n</div>";
                        continue;
                    }

                    existed.ProductImages.Add(new ProductImage
                    {
                        IsPrimary = null,
                        ImageUrl = await photos.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                        Alternative = productVM.Name
                    });
                }
            }

                existed.Name = productVM.Name;
                existed.Price = productVM.Price;
                existed.Description = productVM.Description;
                existed.FullDescription = productVM.FullDescription;
                existed.SKU = productVM.SKU;
                existed.Order = productVM.Order;
                existed.CategoryId = productVM.CategoryId;

                await _context.SaveChangesAsync();
            TempData["Message"] = $"<div class=\"alert alert-success\" role=\"alert\">\r\n  Successfully updated {productVM.Name} product\r\n</div>";
            return RedirectToAction(nameof(Index));
         }



        [Authorize(Roles = nameof(UserRole.Admin))]
        //Delete
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.Include(p=>p.ProductImages).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
           if(product.ProductImages != null)
            {
                foreach (ProductImage item in product.ProductImages)
                {
                    item.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");

                }
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Moderator))]
        //Detail
        public async Task<IActionResult> Detail(int id)
        {
            Product product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.ProductTags)
                .ThenInclude(p => p.Tag)
                .Include(p => p.ProductColors)
                .ThenInclude(p => p.Color)
                .Include(p => p.ProductSizes)
                .ThenInclude(p => p.Size)
                .FirstOrDefaultAsync(p => p.Id == id);
            return View(product);
        }
    }
}
    
