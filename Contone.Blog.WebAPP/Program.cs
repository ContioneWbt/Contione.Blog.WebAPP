using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Contone.Blog.WebAPP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddCommandLine(args)//支持命令行
             .Build();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)
              .ConfigureLogging((context, loggingBuilder) =>
              {
                  loggingBuilder.AddFilter("System", LogLevel.Warning);
                  loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);//过滤掉系统默认的一些日志
                  loggingBuilder.AddLog4Net();
              })
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
             }).UseServiceProviderFactory(new AutofacServiceProviderFactory());
    }
}
