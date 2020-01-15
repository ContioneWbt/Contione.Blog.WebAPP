using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Contione.Blog.Infrastructure.StaticExtenions;
using Contione.Blog.WebAPP.CoreBuilder;
using Contione.Blog.WebAPP.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Contone.Blog.WebAPP
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            StaticConstraint.Init(s => configuration[s]);
        }

        #region .Net Core ≈‰÷√∑˛ŒÒ◊¢»Î
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddContioneServiceProvider(_configuration);
        }
        #endregion

        #region .Net Core ≈‰÷√Autofac 
        public void ConfigureContainer(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<ContioneAutofacExt>();
        }
        #endregion

        #region .Net Core ≈‰÷√
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider svp)
        {
            app.AddContioneConfigureProvider(env, svp, _configuration);
        } 
        #endregion
    }
}
