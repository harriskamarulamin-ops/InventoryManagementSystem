using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [Authorize] 
    public class SupplierController : Controller
    {
        private readonly NHibernate.ISession _session;
        private readonly ISupplier _supplierService;

        public SupplierController(NHibernate.ISession session, ISupplier supplierService)
        {
            _session = session;
            _supplierService = supplierService;
        }

        // 1. READ: List all suppliers (Asynchronous + Search)
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;
            var suppliers = await _supplierService.SearchAsync(searchTerm);
            return View(suppliers);
        }

        // 2. CREATE: GET Form
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // 3. CREATE: POST Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Supplier supplier)
        {
           
            if (!ModelState.IsValid) return View(supplier);

            using (var tx = _session.BeginTransaction())
            {
                _session.Save(supplier);
                _session.Flush();
                tx.Commit();
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. DELETE: Post action
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var supplier = _session.Get<Supplier>(id);
            if (supplier != null)
            {
                using (var tx = _session.BeginTransaction())
                {
                    _session.Delete(supplier);
                    _session.Flush();
                    tx.Commit();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // 5. EDIT: GET Form
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            var supplier = _session.Get<Supplier>(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        // 6. EDIT: POST Submission
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id) return NotFound();
            if (!ModelState.IsValid) return View(supplier);

            using (var tx = _session.BeginTransaction())
            {
                _session.Update(supplier);
                _session.Flush();
                tx.Commit();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
//flush boleh buang