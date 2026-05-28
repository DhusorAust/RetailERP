using DevExpress.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using RetailERP.Core.Common;
using System;
using System.IO;
using RetailERP.Web.Models;

namespace RetailERP.Web
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", builder =>
                {
                    builder.WithOrigins("https://localhost", "http://localhost")
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                });
            });

            services.AddMvc();
            services.AddControllersWithViews();
            services.AddDevExpressControls();

            
            //services.ConfigureReportingServices(configurator => {
            //    configurator.DisableCheckForCustomControllers();
            //});

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(300);
                options.Cookie.IsEssential = true;
            });
            services.AddMvc(setupAction =>
            {
                setupAction.EnableEndpointRouting = false;
            }).AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
            });//.SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddScoped(typeof(DapperContext<>), typeof(DapperContext<>));

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvc().AddRazorRuntimeCompilation();
            services.Configure<FormOptions>(options =>
            {
                options.ValueCountLimit = int.MaxValue;
            });

            services.AddMvc(options =>
            options.MaxModelBindingCollectionSize = 900000000
            );

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 524288000;
            });

            ServiceList.RegisterServiceList(services);
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

           
            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseDevExpressControls();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
                RequestPath = "/node_modules",
            });
            app.UseRouting();
            app.UseCors("AllowSpecificOrigin");
            app.UseSession();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Security}/{action=Index}/{id?}");
            });
        }
    }
}
