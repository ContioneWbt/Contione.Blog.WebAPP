using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.IO;
using Autofac.Extras.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Contione.Blog.Model.DbContext;
using Contione.Blog.IAppService;
using Contione.Blog.AppService;
using Contone.Blog.IRepository;
using Contione.Blog.Repository;
using Contione.Blog.Infrastructure.StaticExtenions;
using Contione.Blog.WebAPP.AOP;

namespace Contione.Blog.WebAPP.CoreBuilder
{
    public class ContioneAutofacExt : Autofac.Module
    {
        /// <summary>
        /// 注册容器
        /// </summary>
        protected override void Load(ContainerBuilder builder)
        {
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

            builder.RegisterType<BlogCacheAOP>();

            var assemblysModel = Assembly.Load(StaticConstraint.BlogModel);

            builder.RegisterAssemblyTypes(assemblysModel).Where(x => x.Name == typeof(ModelBaseContext).Name)
                .InstancePerLifetimeScope();

            //builder.RegisterAssemblyTypes(assemblysModel).Where(x => x.Attributes.Equals(typeof(BlogCacheAOP)))
            //.InstancePerDependency();//指定它的生命周期

            #region IOC 服务解耦和EF仓储解耦
            var repositoryDllFile = Path.Combine(basePath, StaticConstraint.RepositoryDll);
            var assemblysRepository = Assembly.LoadFile(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepository).Where(x => x.Name.Contains(StaticConstraint.Repository)).AsImplementedInterfaces();


            var servicesDllFile = Path.Combine(basePath, StaticConstraint.AppService);//获取项目绝对路径
            var assemblysServices = Assembly.LoadFile(servicesDllFile);
            builder.RegisterAssemblyTypes(assemblysServices)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BlogCacheAOP));//AOP切面缓存;


            //注册泛型
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>));
            builder.RegisterGeneric(typeof(GenericServices<>)).As(typeof(IGenericServices<>));

            #endregion
        }
    }
 
}
