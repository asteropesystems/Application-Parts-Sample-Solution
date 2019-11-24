using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MainWebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void ConfigureApplicationParts(ApplicationPartManager apm)
        {
            string rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pluginsPath = Path.Combine(rootPath, "Plugins");

            // check for plugins path
            if (Directory.Exists(pluginsPath))
            {
                var assemblyFiles = Directory.GetFiles(pluginsPath, "*.*", SearchOption.AllDirectories);
                foreach (var assemblyFile in assemblyFiles)
                {
                    try
                    {
                        var assembly = Assembly.LoadFrom(assemblyFile);
                        if (assemblyFile.EndsWith(".Views.dll"))
                        {
                            apm.ApplicationParts.Add(new CompiledRazorAssemblyPart(assembly));
                        }
                        else
                        {
                            var manifestResourceNames = assembly.GetManifestResourceNames();

                            apm.ApplicationParts.Add(new AssemblyPart(assembly));
                        }
                    }
                    catch (Exception e) { }
                }
            }

            apm.FeatureProviders.Add(new ViewComponentFeatureProvider());
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false)
                .ConfigureApplicationPartManager(ConfigureApplicationParts)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
