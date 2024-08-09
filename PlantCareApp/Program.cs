using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PlantCareApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Get database connection string from the configuration file
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configure the application's DbContext to use SQL Server and automatically retry on failure
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Set up ASP.NET Identity for user management with default settings, including role support
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Adds role management
    .AddEntityFrameworkStores<ApplicationDbContext>(); // Stores user data in the database

// Add support for controllers and views (MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ensure the database is created and all migrations are applied
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Applies any pending migrations to keep the database schema up to date
}

// Create default roles and an admin user if they don't already exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Define the roles we need in the system
    string[] roleNames = { "Admin", "User" };

    // Loop through each role name to ensure it exists in the database
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            // Create the role if it doesn't exist
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Define a default admin user with specific credentials
    var adminUser = new IdentityUser
    {
        UserName = "dario@gc.ca",
        Email = "dario@gc.ca",
        EmailConfirmed = true
    };

    string adminPassword = "Test123$";
    var admin = await userManager.FindByEmailAsync(adminUser.Email);

    if (admin == null)
    {
        // If the admin user doesn't exist, create it with the specified password
        var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
        if (createAdmin.Succeeded)
        {
            // Assign the admin role to the new user
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
    else
    {
        // If the admin user exists but isn't in the Admin role, add them to it
        var roles = await userManager.GetRolesAsync(admin);
        if (!roles.Contains("Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

// Configure the request pipeline based on whether the app is in development or production
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint(); // In development, show detailed migration information
}
else
{
    app.UseExceptionHandler("/Home/Error"); // In production, handle errors with a user-friendly error page
    app.UseHsts(); // Enforce HTTPS for security
}

app.UseHttpsRedirection(); // Ensure HTTP requests are redirected to HTTPS
app.UseStaticFiles(); // Serve static files like images, CSS, and JavaScript

app.UseRouting(); // Enable routing for handling requests

app.UseAuthentication(); // Enable user authentication
app.UseAuthorization(); // Enforce authorization based on user roles

// Set up the default routing pattern for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Enable Razor Pages support

app.Run(); // Start the application
