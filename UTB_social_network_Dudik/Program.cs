using Microsoft.EntityFrameworkCore;
using Utb_sc_Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Utb_sc_Infrastructure.Identity;
using UTB_social_network_Dudik.Hubs;

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
    options.LoginPath = "/Account/Login"; // Adjust the path to your Login action
    options.LogoutPath = "/Account/Logout"; // Adjust the path to your Logout action
    options.SlidingExpiration = true;
});

// Register RoleManager service explicitly (to resolve dependency)
builder.Services.AddScoped<RoleManager<IdentityRole<int>>>();

// Register SignalR services
builder.Services.AddSignalR();

// Add session services
builder.Services.AddDistributedMemoryCache(); // Use in-memory cache for session storage
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Protect the session cookie
    options.Cookie.IsEssential = true; // Ensure it's not blocked by cookie policies
});
builder.Services.AddControllersWithViews();

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

// Use session middleware
app.UseSession();

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

// Map SignalR hub
app.MapHub<ChatHub>("/chathub");

// Run the application
app.Run();
