using System;
using BookM.Models;
using BookM.Services;
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

// Neo4j Configuration
var neo4jConfig = builder.Configuration.GetSection("Neo4j");
var uri = neo4jConfig["Uri"];
var user = neo4jConfig["User"];
var password = neo4jConfig["Password"];

if (!string.IsNullOrEmpty(uri) && !string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(password))
{
    builder.Services.AddSingleton<Neo4j.Driver.IDriver>(provider => 
        Neo4j.Driver.GraphDatabase.Driver(uri, Neo4j.Driver.AuthTokens.Basic(user, password)));
    builder.Services.AddScoped<BookM.Services.Neo4jService>();
}

builder.Services.AddScoped<IEmailSender,EmailSender>();

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
