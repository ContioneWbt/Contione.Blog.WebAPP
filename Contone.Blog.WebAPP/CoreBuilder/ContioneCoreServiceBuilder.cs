using Autofac;
using Contione.Blog.Infrastructure.Cache;
using Contione.Blog.Infrastructure.Log;
using Contione.Blog.Infrastructure.StaticExtenions;
using Contione.Blog.Model.CommonModel.Enums;
using Contione.Blog.WebAPP.Attributes;
using Contione.Blog.WebAPP.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Contione.Blog.WebAPP.CoreBuilder
{
    public class ContioneCoreServiceBuilder : ICoreServiceBuilder
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _provider;
        public ContioneCoreServiceBuilder(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;
            _provider = services.BuildServiceProvider();//get an instance of IServiceProvider
        }

        public void AddAutoMapper()
        {
        }

        public void AddCache()
        {
            var cacheConfig =
               (CacheType)Enum.Parse(typeof(CacheType), _configuration.GetSection("AppSettings")["CacheType"]);
            switch (cacheConfig)
            {
                case CacheType.InMemory:
                    //net本地内存缓存
                    _services.AddScoped<ICache, Infrastructure.Cache.MemoryCache>();
                    _services.AddSingleton((Func<IServiceProvider, IMemoryCache>)(factory =>
                    {
                        var cache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new MemoryCacheOptions());
                        return cache;
                    }));
                    break;
                case CacheType.Redis:
                    //redis 分布式缓存
                    _services.AddScoped<ICache, RedisCache>();
                    //autofac单例模式不能重连对象，只能自定义个类实现单例
                    //services.AddSingleton<ConnectionMultiplexer>(provider =>
                    //{
                    //    var connection= ConnectionMultiplexer.Connect(Appsettings.app(new string[] { "AppSettings", "RedisCaching", "ConnectionString" }));
                    //    return connection;
                    //});
                    break;
            }
        }

        public void AddCors()
        {
        }

        public void AddDbcontext()
        {
        }

        public void AddHttpContext()
        {
        }

        public void AddJwtAuth()
        {
        }

        public void AddLog()
        {
            _services.AddSingleton<ILoggerHelper, LogHelper>();
        }

        public void AddMvcExtensions()
        {
            //注册认证服务
            _services.AddAuthentication(o => o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme
              )
           .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => {
               //这里填写一些配置信息，默认即可
           })    //添加Cookie认证
            .AddQQ(qqOptions =>
            {
                qqOptions.ClientId = StaticConstraint.QQAppId;         //QQ互联申请的AppId
                qqOptions.ClientSecret = StaticConstraint.QQAppKey;    //QQ互联申请的AppKey
                qqOptions.CallbackPath = StaticConstraint.QQCallback;  //QQ互联回调地址
                qqOptions.ClaimActions.MapJsonKey(MyClaimTypes.QQOpenId, "openid");
                qqOptions.ClaimActions.MapJsonKey(MyClaimTypes.QQName, "nickname");
                qqOptions.ClaimActions.MapJsonKey(MyClaimTypes.QQGender, "gender");
                qqOptions.ClaimActions.MapJsonKey(MyClaimTypes.QQFigure, "figureurl_qq_1");
            });
            _services.AddMvc();
            _services.AddControllersWithViews();
            //注册全局filter
            _services.AddMvc(cfg =>
            {
                cfg.Filters.Add(typeof(ExceptionHandleAttribute));
                cfg.Filters.Add(typeof(ActionFilterInvokeAttribute));
            });
            _services.AddRazorPages();
            _services.AddSession();
        }

        public void AddSwaggerGenerator()
        {
        }
    }
}
