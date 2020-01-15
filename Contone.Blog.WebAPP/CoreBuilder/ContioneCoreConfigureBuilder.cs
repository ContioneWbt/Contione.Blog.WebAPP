using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contione.Blog.WebAPP.CoreBuilder
{
    public class ContioneCoreConfigureBuilder : ICoreConfigurationBuilder
    {
        private readonly IApplicationBuilder _app;
        private readonly IWebHostEnvironment _env;
        private readonly IServiceProvider _svp;
        private readonly IConfiguration _configuration;
        public ContioneCoreConfigureBuilder(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp, IConfiguration configuration)
        {
            _app = app;
            _env = env;
            _svp = svp;
            _configuration = configuration;
        }
        public void UseAuth()
        {
          
        }

        public void UseCors()
        {
          
        }

        public void UseErrorHanle()
        {
            if (_env.IsDevelopment())
            {
                _app.UseDeveloperExceptionPage();
            }
            else
            {
                _app.UseExceptionHandler("/Home/Error");
            }
        }

        public void UseOther()
        {
            _app.UseStaticFiles();

            _app.UseRouting();

            _app.UseCookiePolicy();

            _app.UseSession();

            _app.UseAuthentication();

            _app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

            _app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                       name: "areas",
                       areaName: "System",
                       pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });//终结点，可能是mvc 也可能是别的项目类型  signal
        }

        public void UseRazorEngine()
        {
        }

        public void UseSwagger()
        {
        }
    }
}
