// Program.cs
using Microsoft.AspNetCore.Authentication.Cookies;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.Services.Interfaces;
using NHibernate;

var builder = WebApplication.CreateBuilder(args);

// Add MVC Services
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserAccess", policy => policy.RequireRole("Admin", "User"));
});

// Pull connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register NHibernate SessionFactory as a Singleton (Created once on app boot-up)
builder.Services.AddSingleton(s => NHibernateHelper.CreateSessionFactory(connectionString));

// Register NHibernate Session as Scoped (Opened and closed automatically per web request)
builder.Services.AddScoped(s => s.GetRequiredService<ISessionFactory>().OpenSession());

// Register AccountService
builder.Services.AddScoped<IAccountService, AccountService>();

// Register Search Services
builder.Services.AddScoped<ICategory, CategoryService>();
builder.Services.AddScoped<ISupplier, SupplierService>();
builder.Services.AddScoped<IProducts, ProductsService>();
builder.Services.AddScoped<ISales, SalesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    try
    {
        // This explicitly wakes up NHibernate right now!
        var sessionFactory = scope.ServiceProvider.GetRequiredService<ISessionFactory>();
        using (var testSession = sessionFactory.OpenSession())
        {
            // Simply opening a temporary session triggers the DB Generation script.
        }
    }
    catch (Exception ex)
    {
        // If there's a typo in your connection string, it will break here and show you why.
        Console.WriteLine($"Database build failed: {ex.Message}");
    }
}

app.Run();
