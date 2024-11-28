using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Bulky.DataAccess.DbInitializer;
using BulkyBook.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add connection for EntityFrameworkcore
builder.Services.AddDbContext<ApplicationDbContext>(Options => 
Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//To fetch the values for stripe from appsettings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//builder.Services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


//To Handle access denied page.
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.LogoutPath = $"/Identity/Account/AccessDenied";
});

//Adding services for external login facebook
builder.Services.AddAuthentication().AddFacebook(option =>
{
    option.AppId = "1112148753602255";
    option.AppSecret = "f77230e01730a6f5c12323efd55b25c5";
});


//Adding session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(Options => {
    Options.IdleTimeout = TimeSpan.FromMinutes(100);
    Options.Cookie.HttpOnly = true;
    Options.Cookie.IsEssential=true;
});

builder.Services.AddScoped<IDbInitializer,DbInitializer>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
var app = builder.Build();
 
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

//for stripe
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDatabase();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

//For seeding admin role at first time
void SeedDatabase()
{
    using(var scope =app.Services.CreateScope())
    { 
        var dbInitializer=scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();

    }
}
