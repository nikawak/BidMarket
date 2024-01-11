using BidMarket.Models;
using BidMarket.Services;
using CourseProject.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;


public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        var connectionString = builder.Configuration.GetConnectionString("Local");

        services.AddDbContext<AppDbContext>(
        options =>
        {
            options.UseSqlServer(connectionString);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        },
        ServiceLifetime.Scoped);

        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization();


        builder.Services.AddTransient<ICloudService, MegaService>();
        builder.Services.AddTransient<MailSender>();
        builder.Services.AddTransient<TelegramBot>();
        builder.Services.AddTransient<Hasher>();

        builder.Services.AddSingleton<PaymentService>();
        builder.Services.Configure<PaymentSettings>(builder.Configuration.GetSection("BrainTree"));

        builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(opt =>
        {
            opt.Password.RequiredLength = 6;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Password.RequiredUniqueChars = 0;
            opt.Password.RequireDigit = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireUppercase = false;

        }).AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();

        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddControllersWithViews(mvcOtions =>
        {
            mvcOtions.EnableEndpointRouting = false;
        });

        builder.Services.AddCors();
        var app = builder.Build();

        app.UseCors(opt =>
        {
            opt.AllowAnyMethod();
            opt.AllowAnyHeader();
            opt.AllowAnyOrigin();
        });


        app.UseStatusCodePagesWithRedirects("Error/{0}");




        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();
        app.UseMvc(routes =>
        {
           
        });
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Lot}/{action=GetLots}/{id?}");
        #region Roles
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var roles = roleManager.Roles.ToList();
            string[] roleNames = { "Manager", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                }
            }
        }
        #endregion


        app.Run();

    }
}