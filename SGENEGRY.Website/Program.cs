using SGENERGY.BusinessLayers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Missing connection string 'DefaultConnection'.");
                    
// Initialize Business layer configuration (used by existing Dapper repositories)
Configuration.Initialize(connectionString);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// SEO-friendly product routes — must be registered before the default route
app.MapControllerRoute(
    name: "product-list",
    pattern: "san-pham",
    defaults: new { controller = "Product", action = "Index" });

app.MapControllerRoute(
    name: "product-by-category",
    pattern: "san-pham/loai-hang/{categorySlug}",
    defaults: new { controller = "Product", action = "Index" });

app.MapControllerRoute(
    name: "product-by-supplier",
    pattern: "san-pham/hang/{supplierSlug}",
    defaults: new { controller = "Product", action = "Index" });

app.MapControllerRoute(
    name: "product-detail",
    pattern: "san-pham/{slug}",
    defaults: new { controller = "Product", action = "Detail" });

// SEO-friendly project routes
app.MapControllerRoute(
    name: "project-list",
    pattern: "du-an",
    defaults: new { controller = "Project", action = "Index" });

app.MapControllerRoute(
    name: "project-detail",
    pattern: "du-an/{slug}",
    defaults: new { controller = "Project", action = "Detail" });

// SEO-friendly contact route
app.MapControllerRoute(
    name: "contact",
    pattern: "lien-he",
    defaults: new { controller = "Contact", action = "Index" });

// Redirect legacy /Contact URL to SEO-friendly /lien-he
app.MapGet("/Contact", ctx =>
{
    ctx.Response.Redirect("/lien-he", permanent: true);
    return Task.CompletedTask;
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();