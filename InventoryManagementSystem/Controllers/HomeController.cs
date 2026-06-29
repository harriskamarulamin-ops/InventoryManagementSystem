using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using InventoryManagementSystem.Models;
using System.Threading.Tasks;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NHibernate.ISession _session;

        public HomeController(ILogger<HomeController> logger, NHibernate.ISession session)
        {
            _logger = logger;
            _session = session;
        }

        public async Task<IActionResult> Index()
        {
            // Query dashboard stats
            var totalProducts = await _session.Query<Product>().CountAsync();
            var totalCategories = await _session.Query<Category>().CountAsync();
            var totalSuppliers = await _session.Query<Supplier>().CountAsync();
            
            // Total Stock: Sum of current stock
            var totalStockVolume = await _session.Query<Product>().SumAsync(p => (int?)p.CurrentStock) ?? 0;

            // Low Stock Items count (CurrentStock <= MinStockThreshold)
            var lowStockCount = await _session.Query<Product>().CountAsync(p => p.CurrentStock <= p.MinStockThreshold);

            // Total Sales Revenue
            var totalSalesRevenue = await _session.Query<Sale>().SumAsync(s => (decimal?)s.TotalAmount) ?? 0;

            // List of low stock products
            var lowStockProducts = await _session.Query<Product>()
                .Fetch(p => p.Category)
                .Where(p => p.CurrentStock <= p.MinStockThreshold)
                .Take(5)
                .ToListAsync();

            // Recent 5 sales
            var recentSales = await _session.Query<Sale>()
                .OrderByDescending(s => s.SaleDate)
                .Take(5)
                .ToListAsync();

            // Recent 5 stock transactions
            var recentTransactions = await _session.Query<StockTransaction>()
                .Fetch(t => t.Product)
                .OrderByDescending(t => t.TransactionDate)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalSuppliers = totalSuppliers;
            ViewBag.TotalStockVolume = totalStockVolume;
            ViewBag.LowStockCount = lowStockCount;
            ViewBag.TotalSalesRevenue = totalSalesRevenue;
            ViewBag.LowStockProducts = lowStockProducts;
            ViewBag.RecentSales = recentSales;
            ViewBag.RecentTransactions = recentTransactions;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
