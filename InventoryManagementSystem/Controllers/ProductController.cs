using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHibernate;
using NHibernate.Linq;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using System.Threading.Tasks;
using System;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly NHibernate.ISession _session;
        private readonly IProducts _productsService;

        public ProductController(NHibernate.ISession session, IProducts productsService)
        {
            _session = session;
            _productsService = productsService;
        }

        // 1. READ: List all products (Asynchronous + Search)
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;
            var products = await _productsService.SearchAsync(searchTerm);
            return View(products);
        }

        // 2. CREATE: GET Form
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _session.Query<Category>().ToListAsync(), "Id", "Name");
            ViewBag.Suppliers = new SelectList(await _session.Query<Supplier>().ToListAsync(), "Id", "Name");
            return View();
        }

        // 3. CREATE: POST Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)   // ni boleh masukkan dia terus dalam Product Model
        {
            int categoryId = product.Category.Id;
            int supplierId = product.Supplier.Id;
            try
            {
                product.Category = await _session.LoadAsync<Category>(product.Category.Id);
                product.Supplier = await _session.LoadAsync<Supplier>(product.Supplier.Id);

                ModelState.Remove("Category");
                ModelState.Remove("Supplier");

                if (!ModelState.IsValid)
                {
                    ViewBag.Categories = new SelectList(await _session.Query<Category>().ToListAsync(), "Id", "Name", categoryId);
                    ViewBag.Suppliers = new SelectList(await _session.Query<Supplier>().ToListAsync(), "Id", "Name", supplierId);
                    return View(product);
                }

                using (var tx = _session.BeginTransaction())
                {
                    await _session.SaveAsync(product); // Async Save
                    await tx.CommitAsync();            // Async Commit
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving product: {ex.Message}");
                ViewBag.Categories = new SelectList(await _session.Query<Category>().ToListAsync(), "Id", "Name", categoryId);
                ViewBag.Suppliers = new SelectList(await _session.Query<Supplier>().ToListAsync(), "Id", "Name", supplierId);
                return View(product);
            }
        }

        // 4. EDIT: GET Form
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _session.GetAsync<Product>(id); // Async Get
            if (product == null) return NotFound();

            ViewBag.Categories = new SelectList(await _session.Query<Category>().ToListAsync(), "Id", "Name", product.Category?.Id);
            ViewBag.Suppliers = new SelectList(await _session.Query<Supplier>().ToListAsync(), "Id", "Name", product.Supplier?.Id);

            return View(product);
        }

        // 5. EDIT: POST Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product updatedProduct)
        {
            var product = await _session.GetAsync<Product>(id);
            if (product == null) return NotFound();

            int categoryId = updatedProduct.Category.Id;
            int supplierId = updatedProduct.Supplier.Id;



            product.Category = await _session.LoadAsync<Category>(categoryId);
            product.Supplier = await _session.LoadAsync<Supplier>(supplierId);

            ModelState.Remove("Category");
            ModelState.Remove("Supplier");

ModelState.Remove(nameof(updatedProduct.Variant));


            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _session.Query<Category>().ToListAsync(), "Id", "Name", categoryId);
                ViewBag.Suppliers = new SelectList(await _session.Query<Supplier>().ToListAsync(), "Id", "Name", supplierId);
                return View(updatedProduct);
            }

            using (var tx = _session.BeginTransaction())
            {
                product.Name = updatedProduct.Name;
                product.Variant = updatedProduct.Variant;
                product.CostPrice = updatedProduct.CostPrice;
                product.RetailPrice = updatedProduct.RetailPrice;
                product.CurrentStock = updatedProduct.CurrentStock;
                product.MinStockThreshold = updatedProduct.MinStockThreshold;

                await _session.UpdateAsync(product);
                await tx.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 6. DELETE: POST Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _session.GetAsync<Product>(id);
            if (product != null)
            {
                using (var tx = _session.BeginTransaction())
                {
                    await _session.DeleteAsync(product); // Async Delete
                    await tx.CommitAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}