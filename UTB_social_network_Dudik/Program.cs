using Microsoft.EntityFrameworkCore;
using Utb_sc_Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Utb_sc_Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services for MVC
builder.Services.AddControllersWithViews();

// Configure MySQL database connection
string connectionString = builder.Configuration.GetConnectionString("MySQL");
ServerVersion serverVersion = new MySqlServerVersion("8.0.38");
builder.Services.AddDbContext<SocialNetworkDbContext>(options =>
    options.UseMySql(connectionString, serverVersion));

// Configure Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Password options
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 1;

    // Lockout options
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

    // User options
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<SocialNetworkDbContext>()
.AddDefaultTokenProviders();

// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Security/Account/Login";
    options.LogoutPath = "/Security/Account/Logout";
    options.SlidingExpiration = true;
});

// Register RoleManager service explicitly (to resolve dependency)
builder.Services.AddScoped<RoleManager<IdentityRole<int>>>();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
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

// Configure areas routing
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// Configure default routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
