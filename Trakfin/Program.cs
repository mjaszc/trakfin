using Microsoft.EntityFrameworkCore;
using Trakfin.Data;
using Microsoft.AspNetCore.Identity;
using Trakfin.Middlewares;
using System.Web.Http;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        HttpConfiguration config = new HttpConfiguration();

        builder.Services.AddDbContext<TrakfinContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("TrakfinContext"), sqlServerOptionsAction: sql_opt =>
            {
                sql_opt.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null);
            }));

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<TrakfinContext>();

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

        app.UseAuthorization();

        app.UseMiddleware<CancelledTaskBugWorkaroundMessageHandler>();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();
        app.Run();

    }
}


