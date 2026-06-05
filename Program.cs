using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ThaiThanhNhan_2287700059_CuoiKi.Data;
using ThaiThanhNhan_2287700059_CuoiKi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
var hasGoogleCredentials = !string.IsNullOrWhiteSpace(googleClientId)
    && !string.IsNullOrWhiteSpace(googleClientSecret)
    && !googleClientId.Contains("google-client-id", StringComparison.OrdinalIgnoreCase)
    && !googleClientSecret.Contains("google-client-secret", StringComparison.OrdinalIgnoreCase);

if (hasGoogleCredentials)
{
    builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            options.ClientId = googleClientId!;
            options.ClientSecret = googleClientSecret!;
        });
}

builder.Services.AddScoped<CourseCatalogService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

await DbInitializer.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
