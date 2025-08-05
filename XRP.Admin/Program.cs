using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using XRP.DataAccess.Context;
using XRP.DataAccess.Repository.User;
using XRP.Services.User;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<XRPContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("XRPConnection")));// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Home/LoginAdmin"; // redirect if not authenticated
            options.LogoutPath = "/Home/Logout";
            options.AccessDeniedPath = "/Home/AccessDenied"; // optional
            options.ExpireTimeSpan = TimeSpan.FromHours(1); // cookie expiration
        });


// Add services to the container.
builder.Services.AddControllersWithViews();

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
    pattern: "{controller=Home}/{action=LoginAdmin}/{id?}");

app.Run();
