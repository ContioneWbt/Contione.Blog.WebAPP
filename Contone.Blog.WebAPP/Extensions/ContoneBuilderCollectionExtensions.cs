using Autofac;
using Contione.Blog.WebAPP.CoreBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contione.Blog.WebAPP.Extensions
{
    public static class ContioneBuilderCollectionExtensions
    {
        /// <summary>
        /// 添加中间件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static void AddContioneServiceProvider(this IServiceCollection services, IConfiguration configuration)
        {
            ICoreServiceBuilder servicebuilder = new ContioneCoreServiceBuilder(services, configuration);
            servicebuilder.AddMvcExtensions();
            servicebuilder.AddCache();
            servicebuilder.AddLog();
            servicebuilder.AddAutoMapper();
            servicebuilder.AddCors();
            servicebuilder.AddSwaggerGenerator();
            servicebuilder.AddJwtAuth();
            servicebuilder.AddHttpContext();
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="svp"></param>
        /// <param name="configuration"></param>
        public static void AddContioneConfigureProvider(this IApplicationBuilder app, IWebHostEnvironment env,
            IServiceProvider svp, IConfiguration configuration)
        {
            ICoreConfigurationBuilder configurationBuilder = new ContioneCoreConfigureBuilder(app, env, svp, configuration);
            configurationBuilder.UseRazorEngine();
            configurationBuilder.UseErrorHanle();
            configurationBuilder.UseSwagger();
            configurationBuilder.UseAuth();
            configurationBuilder.UseCors();
            configurationBuilder.UseOther();
        }
    }
}
