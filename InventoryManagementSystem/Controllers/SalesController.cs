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
    public class SalesController : Controller
    {
        private readonly NHibernate.ISession _session;
        private readonly ISales _salesService;

        public SalesController(NHibernate.ISession session, ISales salesService)
        {
            _session = session;
            _salesService = salesService;
        }

        // 1. History Grid Index (Asynchronous + Search)
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;
            var salesHistory = await _salesService.SearchAsync(searchTerm);
            return View(salesHistory);
        }

        // 2. GET: New Checkout Terminal
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = new SelectList(_session.Query<Product>(), "Id", "Name");
            return View();
        }

        // 3. POST: Commit Invoice Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int productId, int quantitySold)
        {
            if (quantitySold <= 0)
            {
                ModelState.AddModelError("", "Quantity to sell must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = new SelectList(_session.Query<Product>(), "Id", "Name", productId);
                return View();
            }

            var product = _session.Get<Product>(productId);

            if (product == null || product.CurrentStock < quantitySold)
            {
                ModelState.AddModelError("", "Checkout stopped: Item doesn't exist or insufficient warehouse quantity available!");
                ViewBag.Products = new SelectList(_session.Query<Product>(), "Id", "Name", productId);
                return View();
            }

            using (var tx = _session.BeginTransaction())
            {
                // Create Master Sales Receipt record
                var sale = new Sale
                {
                    SaleDate = DateTime.Now,
                    TotalAmount = product.RetailPrice * quantitySold
                };

                // Create individual row item binding specs
                var saleItem = new SaleItem
                {
                    Sale = sale,
                    Product = product,
                    QuantitySold = quantitySold,
                    UnitPriceAtSale = product.RetailPrice // Lock current historical price
                };

                sale.SaleItems.Add(saleItem);

                // DEDUCT CORRESPONDING CURRENT STOCK METRICS
                product.CurrentStock -= quantitySold;

                _session.Save(sale);      // Saves sale cascading down to saleItem
                _session.Update(product);  // Commit the structural inventory reduction

                _session.Flush();
                tx.Commit();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}