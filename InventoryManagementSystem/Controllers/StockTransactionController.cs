using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHibernate;
using NHibernate.Linq;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Constants;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class StockTransactionController : Controller
    {
        private readonly NHibernate.ISession _session;

        public StockTransactionController(NHibernate.ISession session)
        {
            _session = session;
        }

        // List transaction history ledger
        public IActionResult Index()
        {
            var history = _session.Query<StockTransaction>()
                .OrderByDescending(t => t.TransactionDate)
                .ToList();
            return View(history);
        }

        // GET: Create form
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Products = new SelectList(_session.Query<Product>(), "Id", "Name");
            return View();
        }

        // POST: Record an adjustment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StockTransaction transaction, int productId)
        {
            var product = _session.Get<Product>(productId);
            transaction.Product = product;

            ModelState.Remove(nameof(transaction.Product));

            if (!ModelState.IsValid || product == null)
            {
                ViewBag.Products = new SelectList(_session.Query<Product>(), "Id", "Name", productId);
                return View(transaction);
            }

            using (var tx = _session.BeginTransaction())
            {
                // INVENTORY ENGINE MATHEMATICS
                if (string.Equals(transaction.Type, LookupType.IN, StringComparison.OrdinalIgnoreCase))
    {
        product.CurrentStock += transaction.Quantity;
    }
    else if (string.Equals(transaction.Type, LookupType.OUT, StringComparison.OrdinalIgnoreCase))
    {
        product.CurrentStock -= transaction.Quantity;
    }

                _session.Save(transaction); // Save history line
                _session.Update(product);   // Update running stock level
                tx.Commit();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}