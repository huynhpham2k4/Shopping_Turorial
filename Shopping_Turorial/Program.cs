using Shopping_Tutorial.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Shopping_Tutorial.Models;
using System.Xml;
using Shopping_Tutorial.Areas.Admin.Repository;
using Shopping_Tutorial.Areas.Admin.Repository;
using System.Runtime;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;


//builder la Add, con app la Use

var builder = WebApplication.CreateBuilder(args);

//Connection db
builder.Services.AddDbContext<Datacontext>(option =>
{
	option.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
});

//add email sender
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add services to the container. 
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

//dat session tren build
builder.Services.AddSession(option =>
{
	option.IdleTimeout = TimeSpan.FromSeconds(30);
	option.Cookie.IsEssential = true;
});// phai dat tren build


//username & pasaord
builder.Services.AddIdentity<AppUserModel, IdentityRole>()
	.AddEntityFrameworkStores<Datacontext>().AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
	// Password settings.
	options.Password.RequireDigit = true;
	options.Password.RequireLowercase = true;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 4;

	options.User.RequireUniqueEmail = true;
});

//Congiguration Login Google Account
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
	options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientID").Value;
	options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
});
//---Congiguration Login Google Account

var app = builder.Build();

app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

app.UseSession();


// Configure the HTTP request pipeline.s
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // danh nhap truoc

app.UseAuthorization();// kiem tra quyen

app.MapControllerRoute(
	name: "default",
	pattern: "/{controller=Home}/{action=Index}",
	defaults: new { controller = "Home", action = "Index" });

//app.MapControllerRoute(
//name: "default",
//pattern: "/{controller = Home}/{action = Index}/{id?}",
//defaults: new { controller = "Home", action = "Index" });

// tai sau cau hinh nhu thay nay thi no khong duoc

app.MapControllerRoute(
	name: "Areas",
	pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "category",
	pattern: "/category/{Slug?}",
	defaults: new { controller = "Category", action = "Index" });

app.MapControllerRoute(
	name: "brand",
	pattern: "/brand/{Slug?}",
	defaults: new { controller = "brand", action = "Index" });





//Seeding Data
var contxet = app.Services.CreateScope().ServiceProvider.GetRequiredService<Datacontext>();
SeedData.SeedingData(contxet);

app.Run();
