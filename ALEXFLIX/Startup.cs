using ALEXFLIX.Data;
using ALEXFLIX.Helpers;
using ALEXFLIX.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALEXFLIX
{
    public class Startup
    {
        IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //AUTENTICACION DE USUARIOS
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie();

            services.AddResponseCaching();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5);
            });
            services.AddTransient<UploadService>();
            services.AddTransient<PathProvider>();
            services.AddTransient<MailService>();
            services.AddTransient<ToolkitService>();
            services.AddTransient<CyperService>();
            String cadenasql = this.Configuration.GetConnectionString("cadenasql");
            //String cadenaazure = this.Configuration.GetConnectionString("cadenasqlazure");

            services.AddDbContext<AlexflixContext>(options => options.UseSqlServer(cadenasql));
            //services.AddDbContext<AlexflixContext>(options => options.UseSqlServer(cadenaazure));

            //API
            String urlapiAlexflix = this.Configuration["urlapiAlexflix"];
            services.AddTransient(x => new ServiceAlexflixApi(urlapiAlexflix));

            //STORAGE BLOB
            String storagekeys = this.Configuration["StorageKeyAccount"];
            services.AddTransient(x => new ServiceStorageBlobs(storagekeys));

            //INYECTAMOS TEMPDATA, con CookieTempData
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();

            services.AddControllersWithViews(option => option.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseStaticFiles();
            app.UseResponseCaching();
            app.UseSession();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
               routes.MapRoute(
                   name: "default",
                   template: "{controller=Peliculas}/{action=Index}/{id?}"
               );
            });

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}"
            //        );
            //});
        }

    }
}
