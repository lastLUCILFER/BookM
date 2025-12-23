using System;
using BookM.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------------------
//  Add services
// ---------------------
 builder.Services.AddDbContext<BookMContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Where to send user if not logged in
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });
builder.Services.AddHttpClient<BookM.Services.TicketmasterService>();
builder.Services.AddScoped<BookM.Services.TicketmasterService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ---------------------
//  Middleware
// ---------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ---------------------
//  Routes
// ---------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
