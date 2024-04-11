using IdentityManagerUI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using WebApplication1.Data;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetSection("ConnectionStrings").GetValue<string>("Default");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

            services.AddDefaultIdentity<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                //.AddDefaultTokenProviders()
                //.AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
                //.AddPasswordValidator<CommonPasswordValidator<ApplicationUser>>()
                ;

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //Temporary middleware for testing, don't use in production!
            app.Use(async (context, next) =>
            {
                var userIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "Admin") }, "Demo");
                context.User = new ClaimsPrincipal(userIdentity);
                await next.Invoke();
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "areas", pattern: "{area}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
