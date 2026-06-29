using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly NHibernate.ISession _session;
        private readonly ICategory _categoryService;

        public CategoryController(NHibernate.ISession session, ICategory categoryService)
        {
            _session = session;
            _categoryService = categoryService;
        }

        // READ
        public async Task<IActionResult> Index(string searchTerm)
        {
            ViewBag.SearchTerm = searchTerm;
            var categories = await _categoryService.SearchAsync(searchTerm);
            return View(categories);
        }

        // CREATE (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            using (var tx = _session.BeginTransaction())
            {
                await _session.SaveAsync(category);
                await tx.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _session.GetAsync<Category>(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Category updatedCategory)
        {
            var category = await _session.GetAsync<Category>(id);
            if (category == null) return NotFound();

            if (!ModelState.IsValid) return View(updatedCategory);

            using (var tx = _session.BeginTransaction())
            {
                category.Name = updatedCategory.Name;
                category.Description = updatedCategory.Description;

                await _session.UpdateAsync(category);
                await tx.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _session.GetAsync<Category>(id);
            if (category != null)
            {
                using (var tx = _session.BeginTransaction())
                {
                    await _session.DeleteAsync(category);
                    await tx.CommitAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}