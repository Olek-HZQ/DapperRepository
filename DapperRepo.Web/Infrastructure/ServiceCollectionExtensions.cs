using System;
using System.Reflection;
using Autofac;
using DapperRepo.Core;
using DapperRepo.Core.Cache;
using DapperRepo.Core.Constants;
using DapperRepo.Core.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DapperRepo.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            services.AddControllersWithViews().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
                .AddControllersAsServices();

            CommonHelper.DefaultFileProvider = new AppFileProvider(hostEnvironment.ContentRootPath);
        }

        public static void DependencyRegistrar(this ContainerBuilder builder, IConfiguration configuration)
        {
            if (bool.Parse(configuration.GetSection("AppConfig:RedisEnabled").Value))
            {
                builder.RegisterType<RedisConnectionWrapper>()
                    .As<ILocker>()
                    .As<IRedisConnectionWrapper>()
                    .SingleInstance();

                RegisterMore(builder, x =>
                {
                    x.RegisterType<RedisCacheManager>().As<IStaticCacheManager>().InstancePerLifetimeScope();
                });
            }

            switch (configuration.GetSection("AppConfig:CurrentDbTypeName").Value)
            {
                case ConnKeyConstants.Mssql:

                    // Repositories
                    RegisterRepository(builder, ConnKeyConstants.Mssql);

                    // Services
                    RegisterService(builder, ConnKeyConstants.Mssql);

                    break;

                case ConnKeyConstants.Mysql:

                    // Repositories
                    RegisterRepository(builder, ConnKeyConstants.Mysql);

                    // Services
                    RegisterService(builder, ConnKeyConstants.Mysql);

                    break;

                case ConnKeyConstants.Oracle:

                    // configure it by yourself if you wanna use oracle

                    break;
            }
        }

        #region for auofac dependency

        private static void RegisterRepository(ContainerBuilder builder, string registerDbTypeName)
        {
            string namespacePrefix = $"DapperRepo.Data.Repositories.{registerDbTypeName}";

            builder.RegisterAssemblyTypes(Assembly.Load("DapperRepo.Data"))
                .Where(x => x.Namespace != null && x.Namespace.StartsWith(namespacePrefix)
                                                && x.Name.EndsWith("Repository"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        private static void RegisterService(ContainerBuilder builder, string registerDbTypeName)
        {
            string namespacePrefix = $"DapperRepo.Services.{registerDbTypeName}";

            builder.RegisterAssemblyTypes(Assembly.Load("DapperRepo.Services"))
                .Where(x => x.Namespace != null && x.Namespace.StartsWith(namespacePrefix)
                                                && x.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// 注册更多（比如当前如果使用的MSSQL数据库，那有可能还需要MYSQL数据库的相关操作，这里就可以作为扩展注册）
        /// </summary>
        private static void RegisterMore(ContainerBuilder builder, Action<ContainerBuilder> register = null)
        {
            register?.Invoke(builder);
        }

        #endregion
    }
}
