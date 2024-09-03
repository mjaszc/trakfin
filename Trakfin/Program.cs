using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Trakfin.Data;
using Trakfin.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Trakfin;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TrakfinContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TrakfinContext") ?? throw new InvalidOperationException("Connection string 'TrakfinContext' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TrakfinContext>();

// Add services to the container.
// builder.Services.AddControllersWithViews();


var app = builder.Build();

var mySecret = builder.Configuration["API_URL"];

/*
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}
*/

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
