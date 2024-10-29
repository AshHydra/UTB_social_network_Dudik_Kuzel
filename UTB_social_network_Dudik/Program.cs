using Microsoft.EntityFrameworkCore;
using Utb_sc_Infrastructure.Database;
using Utb_sc_Domain.Entities; // Adjust to your actual namespace for entities
using Microsoft.AspNetCore.Identity;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// P�id�n� slu�eb pro MVC
builder.Services.AddControllersWithViews();

// Nastaven� p�ipojen� k datab�zi MySQL (v p��pad�, �e pou��v�te MySQL)
string connectionString = builder.Configuration.GetConnectionString("MySQL");
ServerVersion serverVersion = new MySqlServerVersion("8.0.38");
builder.Services.AddDbContext<SocialNetworkDbContext>(optionsBuilder => optionsBuilder.UseMySql(connectionString, serverVersion));

// Konfigurace Identity
builder.Services.AddIdentity<User, Role>()
     .AddEntityFrameworkStores<Social_network_DbContext>()
     .AddDefaultTokenProviders();

// Konfigurace mo�nost� Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 1;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.User.RequireUniqueEmail = true;
});

// Konfigurace pro cookie ov��ov�n�
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.LoginPath = "/Security/Account/Login";
    options.LogoutPath = "/Security/Account/Logout";
    options.SlidingExpiration = true;
});

// Registrace slu�eb aplikace (pokud m�te n�jak� slu�by ve sv� aplikaci)
//builder.Services.AddScoped<IFileUploadService, FileUploadService>(); // Replace or add other services as needed

builder.Services.AddDbContext<Social_network_DbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySQL"),
        new MySqlServerVersion("8.0.38"),
        mySqlOptions => mySqlOptions.MigrationsAssembly("Utb_sc_Infrastructure") // P�id�n� MigrationsAssembly
    )
);


var app = builder.Build();

// Konfigurace HTTP request pipeline
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

// Definice oblast� (areas)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// V�choz� route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
