using ASC.Web.Data;
using Microsoft.EntityFrameworkCore;
using ASC.DataAccess.Repository;
using Microsoft.AspNetCore.Identity;
using ASC.Web.Navigation;
using ASC.Business.Interfaces;
using ASC.Business.Operations;
using ASC.Web.Areas.Configuration.Models;
using ASC.Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

 
builder.Services.Configure<ASC.Web.Configuration.ApplicationSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.Configure<ASC.Web.Configuration.AdminUserSettings>(
    builder.Configuration.GetSection("AdminUser"));

builder.Services.Configure<ASC.Web.Configuration.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

 
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        var googleAuthSection = builder.Configuration.GetSection("Authentication:Google");

        options.ClientId = googleAuthSection["ClientId"] ?? string.Empty;
        options.ClientSecret = googleAuthSection["ClientSecret"] ?? string.Empty;
    });

 
builder.Services.AddScoped<IUnitOfWork>(serviceProvider =>
{
    var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    return new UnitOfWork(context);
});

 
builder.Services.AddScoped<IMasterDataOperations, MasterDataOperations>();
builder.Services.AddScoped<IMasterDataCacheOperations, MasterDataCacheOperations>();
builder.Services.AddScoped<IServiceRequestOperations, ServiceRequestOperations>();

builder.Services.AddScoped<IServiceNotificationOperations, ServiceNotificationOperations>();
builder.Services.AddScoped<IPromotionOperations, PromotionOperations>();
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IIdentitySeed, IdentitySeed>();

builder.Services.AddTransient<ASC.Web.Services.IEmailSender, ASC.Web.Services.AuthMessageSender>();
builder.Services.AddTransient<ASC.Web.Services.ISmsSender, ASC.Web.Services.AuthMessageSender>();

builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, ASC.Web.Services.AuthMessageSender>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["CacheSettings:CacheConnectionString"];
    options.InstanceName = builder.Configuration["CacheSettings:CacheInstance"];
});
builder.Services.AddMemoryCache();

builder.Services.AddScoped<INavigationCacheOperations, NavigationCacheOperations>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
 
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

var app = builder.Build();

 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();


app.MapAreaControllerRoute(
    name: "ServiceRequestsArea",
    areaName: "ServiceRequests",
    pattern: "ServiceRequests/{controller=Dashboard}/{action=Dashboard}/{id?}");

app.MapAreaControllerRoute(
    name: "AccountsArea",
    areaName: "Accounts",
    pattern: "Accounts/{controller=Account}/{action=Profile}/{id?}");

app.MapAreaControllerRoute(
    name: "ConfigurationArea",
    areaName: "Configuration",
    pattern: "Configuration/{controller=MasterData}/{action=MasterKeys}/{id?}");

app.MapControllerRoute(
    name: "areaRoute",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    await context.Database.MigrateAsync();

    var identitySeed = scope.ServiceProvider.GetRequiredService<IIdentitySeed>();
    await identitySeed.SeedAsync();

    var masterDataCacheOperations = scope.ServiceProvider.GetRequiredService<IMasterDataCacheOperations>();
    await masterDataCacheOperations.CreateMasterDataCacheAsync();
}

app.Run();