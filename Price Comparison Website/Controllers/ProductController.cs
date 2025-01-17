
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Controllers
{
    public class ProductController : Controller
    {
		private Repository<Product> products;
		private Repository<Category> categories;
        private Repository<PriceListing> priceListings;
        private Repository<Vendor> vendors;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            products = new Repository<Product>(context);
            categories = new Repository<Category>(context);
            priceListings = new Repository<PriceListing>(context);
            vendors = new Repository<Vendor>(context);
            _webHostEnvironment = webHostEnvironment;
        }

		public async Task<IActionResult> Index()
        {
            return View(await products.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int id)
        {
            ViewBag.Categories = await categories.GetAllAsync();
            if (id==0)
            {
                ViewBag.Operation = "Add";
                return View(new Product());
            }
            else
            {
                Product product = await products.GetByIdAsync(id, new QueryOptions<Product>
                {
                    Includes = "Category", Where = p => p.ProductId == id
                });
                ViewBag.Operation = "Edit";
                return View(product);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(Product product, int catId)
        {
            ViewBag.Categories = await categories.GetAllAsync();
            if (ModelState.IsValid)
            {
                // Add Operation
                if(product.ProductId == 0)
                {
                    product.CategoryId = catId;
                    await products.AddAsync(product);
                }

                // Edit Opertaion
                else
                {
                    var existingProduct = await products.GetByIdAsync(product.ProductId, new QueryOptions<Product>());

                    if (existingProduct == null)
                    {
                        ModelState.AddModelError("", "Product not found");
                        return View(product);
                    }

                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.CategoryId = catId;

                    try
                    {
                        await products.UpdateAsync(existingProduct);
                        return RedirectToAction("ViewProduct", "Product", new { id = existingProduct.ProductId });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", $"Error: {ex.GetBaseException().Message}");
                        return View(product);
                    }
                }
            }
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Delete Product
            try
            {
                await products.DeleteAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error deleting product: {ex.GetBaseException().Message}");
            }


            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> ViewProduct(int id)
        {
            ViewBag.Listings = await priceListings.GetAllByIdAsync<int>(id, "ProductId", new QueryOptions<PriceListing>
            {
                Includes = "Product",
                Where = pl => pl.ProductId == id
            });
            foreach (PriceListing listing in ViewBag.Listings)
            {
                listing.Vendor = await vendors.GetByIdAsync(listing.VendorId, new QueryOptions<Vendor>());
            }

            if (id == 0)
            {
				return RedirectToAction("Index", "Product");
			}
			Product product = await products.GetByIdAsync(id, new QueryOptions<Product>());
			return View(product);
        }
    }
}
