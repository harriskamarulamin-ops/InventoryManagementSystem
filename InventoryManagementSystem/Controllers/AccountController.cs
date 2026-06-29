// Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using NHibernate;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Interfaces;

namespace InventoryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly NHibernate.ISession _session;
        private readonly IAccountService _accountService;

        public AccountController(NHibernate.ISession session, IAccountService accountService)
        {
            _session = session;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User model, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("password", "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = _session.Query<User>()
                .FirstOrDefault(u => u.Username == model.Username || u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Username or Email already registered.");
                return View(model);
            }

            using (var tx = _session.BeginTransaction())
            {
                // BCrypt automatically hashes and salts 
                model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

                _session.Save(model);
                tx.Commit();
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [Required(ErrorMessage = "Email is required.")][DataType(DataType.EmailAddress)] string email,
            [Required(ErrorMessage = "Password is required.")][DataType(DataType.Password)] string password)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _accountService.AuthenticateAsync(email, password);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Send them right into the main application space!
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}