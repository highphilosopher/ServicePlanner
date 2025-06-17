using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicePlanner.Components;
using ServicePlanner.Data;
using ServicePlanner.Models;
using ServicePlanner.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add authentication services for interactive components
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
    });
builder.Services.AddAuthorization();

// Add Entity Framework with configurable database path
var dbPath = Environment.GetEnvironmentVariable("SERVICEPLANNER_DB_PATH") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=./data/serviceplanner.db";

// Ensure the directory exists for the database file
var dbDirectory = Path.GetDirectoryName(dbPath.Replace("Data Source=", ""));
if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
{
    Directory.CreateDirectory(dbDirectory);
}

builder.Services.AddDbContext<ServicePlannerContext>(options =>
    options.UseSqlite(dbPath.StartsWith("Data Source=") ? dbPath : $"Data Source={dbPath}"));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ServicePlannerContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

// Register application services
builder.Services.AddScoped<ServiceTemplateService>();
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<SongService>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

// Initialize database with sample data and admin user
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ServicePlannerContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    
    // Ensure database is created and migrations are applied
    await context.Database.MigrateAsync();
    
    await SeedDatabase(context);
    await SeedRolesAndAdminUser(roleManager, userService);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Add API endpoint for login
app.MapPost("/api/login", async (LoginRequest request, UserService userService, HttpContext context) =>
{
    try
    {
        var result = await userService.SignInAsync(request.Email, request.Password, request.RememberMe);
        
        if (result.Succeeded)
        {
            var user = await userService.GetUserByEmailAsync(request.Email);
            return Results.Ok(new { success = true, isFirstLogin = user?.IsFirstLogin ?? false });
        }
        else if (result.IsLockedOut)
        {
            return Results.Ok(new { success = false, error = "Account is locked out. Please try again later." });
        }
        else if (result.RequiresTwoFactor)
        {
            return Results.Ok(new { success = false, error = "Two-factor authentication is required." });
        }
        else
        {
            return Results.Ok(new { success = false, error = "Invalid email or password." });
        }
    }
    catch (Exception)
    {
        return Results.Ok(new { success = false, error = "An error occurred during login. Please try again." });
    }
});

app.MapPost("/api/logout", async (UserService userService) =>
{
    await userService.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapGet("/api/logout", async (UserService userService) =>
{
    await userService.SignOutAsync();
    return Results.Redirect("/login");
});

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

static async Task SeedDatabase(ServicePlannerContext context)
{
    // Check if data already exists
    if (await context.Songs.AnyAsync() || await context.ServiceTemplates.AnyAsync())
        return;

    // Add sample songs
    var songs = new List<ServicePlanner.Models.Song>
    {
        new() { Name = "Amazing Grace", SongName = "Amazing Grace", Artist = "Traditional", Category = "Hymn", Key = "G", Speed = "Slow", Publisher = "Traditional", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "How Great Thou Art", SongName = "How Great Thou Art", Artist = "Traditional", Category = "Hymn", Key = "C", Speed = "Medium", Publisher = "Traditional", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "Blessed Be Your Name", SongName = "Blessed Be Your Name", Artist = "Matt Redman", Category = "Contemporary", Key = "A", Speed = "Medium", Publisher = "Thankyou Music", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "10,000 Reasons", SongName = "10,000 Reasons", Artist = "Matt Redman", Category = "Contemporary", Key = "C", Speed = "Medium", Publisher = "Thankyou Music", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "Great Is Thy Faithfulness", SongName = "Great Is Thy Faithfulness", Artist = "Traditional", Category = "Hymn", Key = "Bb", Speed = "Slow", Publisher = "Traditional", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "Way Maker", SongName = "Way Maker", Artist = "Sinach", Category = "Contemporary", Key = "E", Speed = "Medium", Publisher = "Integrity Music", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "Holy, Holy, Holy", SongName = "Holy, Holy, Holy", Artist = "Traditional", Category = "Hymn", Key = "Eb", Speed = "Slow", Publisher = "Traditional", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "Cornerstone", SongName = "Cornerstone", Artist = "Hillsong", Category = "Contemporary", Key = "C", Speed = "Medium", Publisher = "Hillsong Music", Seasonal = false, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "Silent Night", SongName = "Silent Night", Artist = "Traditional", Category = "Christmas", Key = "C", Speed = "Slow", Publisher = "Traditional", Seasonal = true, Disabled = false, CreatedDate = DateTime.Now },
        new() { Name = "Joy to the World", SongName = "Joy to the World", Artist = "Traditional", Category = "Christmas", Key = "D", Speed = "Fast", Publisher = "Traditional", Seasonal = true, Disabled = false, CreatedDate = DateTime.Now }
    };

    context.Songs.AddRange(songs);

    // Add sample template
    var template = new ServicePlanner.Models.ServiceTemplate
    {
        Name = "Sunday Morning Service",
        Description = "Standard Sunday morning worship service",
        CreatedDate = DateTime.Now,
        Events = new List<ServicePlanner.Models.ServiceEvent>
        {
            new() { Type = ServicePlanner.Models.EventType.Prayer, Title = "Opening Prayer", Order = 1 },
            new() { Type = ServicePlanner.Models.EventType.Song, Title = "Opening Hymn", Order = 2 },
            new() { Type = ServicePlanner.Models.EventType.Speaker, Title = "Welcome & Announcements", Order = 3 },
            new() { Type = ServicePlanner.Models.EventType.Song, Title = "Worship Songs", Order = 4 },
            new() { Type = ServicePlanner.Models.EventType.Communion, Title = "Communion", Order = 5 },
            new() { Type = ServicePlanner.Models.EventType.Speaker, Title = "Main Message", Order = 6 },
            new() { Type = ServicePlanner.Models.EventType.Song, Title = "Closing Hymn", Order = 7 },
            new() { Type = ServicePlanner.Models.EventType.Prayer, Title = "Closing Prayer", Order = 8 }
        }
    };

    context.ServiceTemplates.Add(template);
    await context.SaveChangesAsync();
}

static async Task SeedRolesAndAdminUser(RoleManager<IdentityRole> roleManager, UserService userService)
{
    // Create roles if they don't exist
    string[] roles = { "Admin", "User" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
    
    // Create default admin user
    await userService.SeedDefaultAdminAsync();
}
