using FoodieOnlineOrdering.Data;
using FoodieOnlineOrdering.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is missing.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    var hasher = new PasswordHasher<User>();

    if (!await db.Users.AnyAsync(x => x.Username == "Admin"))
    {
        var admin = new User
        {
            Name = "Administrator",
            Username = "Admin",
            Email = "admin@foodie.local",
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };
        admin.PasswordHash = hasher.HashPassword(admin, "Admin123@");
        db.Users.Add(admin);
    }

    if (!await db.Foods.AnyAsync())
    {
        db.Foods.AddRange(
            new Food { Name = "Classic Beef Burger", Category = "Burgers", Description = "Juicy beef patty, cheddar, lettuce and house sauce.", Price = 1450, ImageUrl = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?auto=format&fit=crop&w=900&q=80", IsAvailable = true },
            new Food { Name = "Margherita Pizza", Category = "Pizza", Description = "Tomato, mozzarella and fresh basil on a crispy base.", Price = 1850, ImageUrl = "https://images.unsplash.com/photo-1574071318508-1cdbab80d002?auto=format&fit=crop&w=900&q=80", IsAvailable = true },
            new Food { Name = "Chicken Kottu", Category = "Sri Lankan", Description = "Chopped roti, chicken, vegetables and aromatic spices.", Price = 1200, ImageUrl = "https://images.unsplash.com/photo-1601050690597-df0568f70950?auto=format&fit=crop&w=900&q=80", IsAvailable = true },
            new Food { Name = "Chicken Biryani", Category = "Rice", Description = "Fragrant basmati rice, spiced chicken and fresh salad.", Price = 1600, ImageUrl = "https://images.unsplash.com/photo-1563379091339-03246963d51a?auto=format&fit=crop&w=900&q=80", IsAvailable = true },
            new Food { Name = "French Fries", Category = "Sides", Description = "Golden crispy fries with a light seasoning.", Price = 650, ImageUrl = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?auto=format&fit=crop&w=900&q=80", IsAvailable = true },
            new Food { Name = "Chocolate Milkshake", Category = "Drinks", Description = "Creamy chocolate shake topped with whipped cream.", Price = 900, ImageUrl = "https://images.unsplash.com/photo-1572490122747-3968b75cc699?auto=format&fit=crop&w=900&q=80", IsAvailable = true }
        );
    }

    await db.SaveChangesAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
