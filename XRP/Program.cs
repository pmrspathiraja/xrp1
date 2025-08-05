using System.Configuration;
using XRP.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using XRP.Services.User;
using XRP.DataAccess.Repository.User;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<XRPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("XRPConnection")));// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
// 🔥 Add Authentication with Cookie Scheme
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Home/Login"; // redirect if not authenticated
            options.LogoutPath = "/Home/Logout";
            options.AccessDeniedPath = "/Home/AccessDenied"; // optional
            options.ExpireTimeSpan = TimeSpan.FromHours(1); // cookie expiration
        });

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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();



app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
