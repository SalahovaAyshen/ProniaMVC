using FrontToBack_Pronia.Areas.Manage.ViewModels;
using FrontToBack_Pronia.DAL;
using FrontToBack_Pronia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrontToBack_Pronia.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Include(p => p.ProductImages.Where(pi => pi.IsPrimary == true))
                .Include(p => p.Category)
                .ToListAsync();
            return View(products);
        }

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
                ProductSizes = new List<ProductSize>()
            };
            foreach (int item in productVM.TagIds)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = item,

                };
                product.ProductTags.Add(productTag);
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

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Update
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.Include(p => p.ProductTags).Include(p => p.ProductColors).Include(p => p.ProductSizes).FirstOrDefaultAsync(x => x.Id == id);
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
            productVM.Categories = await _context.Categories.ToListAsync();
            productVM.Tags = await _context.Tags.ToListAsync();
            productVM.Colors = await _context.Colors.ToListAsync();
            productVM.Sizes = await _context.Sizes.ToListAsync();

            if (!ModelState.IsValid) return View(productVM);

            Product existed = await _context.Products
                .Include(p => p.ProductTags)
                .Include(p => p.ProductColors)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existed == null) return NotFound();

            if (await _context.Categories.FirstOrDefaultAsync(c => c.Id == productVM.CategoryId) is null)
            {
                ModelState.AddModelError("CategoryId", "Not found category id");
                return View(productVM);
            }

            foreach (ProductTag pTag in existed.ProductTags)
            {
                if (!productVM.TagIds.Exists(tId => tId == pTag.TagId))
                {
                    _context.ProductTags.Remove(pTag);
                }
            }

            foreach (int tId in productVM.TagIds)
            {
                if (!existed.ProductTags.Any(pt => pt.TagId == tId))
                {
                    bool result = await _context.Tags.AnyAsync(pt => pt.Id == tId);
                    if (!result)
                    {
                        ModelState.AddModelError("TagIds", "Not found tag id");
                        return View(productVM);
                    }
                    existed.ProductTags.Add(new ProductTag { TagId = tId });

                }
            }

            foreach (ProductColor pColor in existed.ProductColors)
            {
                if (!productVM.ColorIds.Exists(cId => cId == pColor.ColorId))
                {
                    _context.ProductColors.Remove(pColor);
                }
            }
            foreach (int cId in productVM.ColorIds)
            {
                if (!existed.ProductColors.Any(pc => pc.ColorId == cId))
                {
                    bool result = await _context.Colors.AnyAsync(pc => pc.Id == cId);
                    if (!result)
                    {
                        ModelState.AddModelError("ColorIds", "Not found color id");
                        return View(productVM);
                    }

                    existed.ProductColors.Add(new ProductColor { ColorId = cId });
                }
            }

            foreach (ProductSize pSize in existed.ProductSizes)
            {
                if (!productVM.SizeIds.Exists(sId => sId == pSize.SizeId))
                {
                    _context.ProductSizes.Remove(pSize);
                }
            }

            foreach (int sId in productVM.SizeIds)
            {
                if (!existed.ProductSizes.Any(ps => ps.SizeId == sId))
                {
                    bool result = await _context.Sizes.AnyAsync(ps => ps.Id == sId);
                    if (!result)
                    {
                        ModelState.AddModelError("SizeIds", "Not found size id");
                        return View(productVM);
                    }
                    existed.ProductSizes.Add(new ProductSize { SizeId = sId });
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
                return RedirectToAction(nameof(Index));
         }



        //Delete
            public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
    
