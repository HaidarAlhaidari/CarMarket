using CarMarket.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// DATABASE
// ----------------------------------------------------

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ----------------------------------------------------
// IDENTITY OCH SÄKERHET
// ----------------------------------------------------

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        // E-postadressen måste vara unik
        options.User.RequireUniqueEmail = true;

        // Kan ändras till true senare när e-postbekräftelse fungerar
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;

        // Krav på lösenord
        options.Password.RequiredLength = 10;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredUniqueChars = 1;

        // Lås kontot efter flera felaktiga inloggningsförsök
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Inställningar för inloggnings-cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "CarMarket.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;
    options.Cookie.SameSite =
        SameSiteMode.Lax;

    options.ExpireTimeSpan =
        TimeSpan.FromMinutes(60);

    options.SlidingExpiration = true;

    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath =
        "/Identity/Account/AccessDenied";
});

// MVC Controllers och Razor Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----------------------------------------------------
// SKAPA ROLLER OCH ADMIN
// ----------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<IdentityUser>>();

    // Roller som ska finnas i databasen
    string[] roles =
    {
        "Admin",
        "User"
    };

    foreach (string roleName in roles)
    {
        bool roleExists =
            await roleManager.RoleExistsAsync(roleName);

        if (!roleExists)
        {
            await roleManager.CreateAsync(
                new IdentityRole(roleName));
        }
    }

    // Uppgifterna läses från User Secrets.
    // De ska inte skrivas direkt i Program.cs.
    string? adminEmail =
        builder.Configuration["AdminUser:Email"];

    string? adminPassword =
        builder.Configuration["AdminUser:Password"];

    if (!string.IsNullOrWhiteSpace(adminEmail) &&
        !string.IsNullOrWhiteSpace(adminPassword))
    {
        var adminUser =
            await userManager.FindByEmailAsync(adminEmail);

        // Skapa administratören om kontot inte finns
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult =
                await userManager.CreateAsync(
                    adminUser,
                    adminPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Admin user could not be created: {errors}");
            }
        }

        // Kontrollera att användaren har Admin-rollen
        if (!await userManager.IsInRoleAsync(
                adminUser,
                "Admin"))
        {
            var roleResult =
                await userManager.AddToRoleAsync(
                    adminUser,
                    "Admin");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Admin role could not be added: {errors}");
            }
        }
    }
}

// ----------------------------------------------------
// HTTP PIPELINE
// ----------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    // Skyddar webbplatsen genom att kräva HTTPS
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Viktig ordning:
// 1. Kontrollera vem användaren är
// 2. Kontrollera vad användaren får göra
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Standard-route för MVC
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Identity använder Razor Pages
app.MapRazorPages()
    .WithStaticAssets();

app.Run();