using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using DapperRepo.Core;
using DapperRepo.Core.Cache;
using DapperRepo.Core.Configuration;
using DapperRepo.Core.Constants;
using DapperRepo.Core.Infrastructure;
using DapperRepo.Services.Customers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DapperRepo.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            services.AddControllersWithViews().AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
                .AddControllersAsServices();

            CommonHelper.DefaultFileProvider = new AppFileProvider(hostEnvironment.ContentRootPath);

            //add AppConfig configuration parameters
            var appConfig = services.ConfigureStartupConfig<AppConfig>(configuration.GetSection("AppConfig"));

            var builder = new ContainerBuilder();
            builder.Populate(services);

            if (appConfig.RedisEnabled)
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

            switch (appConfig.CurrentDbTypeName)
            {
                case ConnKeyConstants.Mssql:

                    // Repositories
                    RegisterRepository(builder, ConnKeyConstants.Mssql);

                    // Services
                    RegisterService(builder);

                    break;

                case ConnKeyConstants.Mysql:

                    // Repositories
                    RegisterRepository(builder, ConnKeyConstants.Mysql);

                    // Services
                    RegisterService(builder);

                    break;

                case ConnKeyConstants.Oracle:

                    // configure it by yourself if you wanna use oracle

                    break;
            }

            return new AutofacServiceProvider(builder.Build());
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

        private static void RegisterService(ContainerBuilder builder)
        {
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
        }

        /// <summary>
        /// 注册更多（比如当前如果使用的MSSQL数据库，那有可能还需要MYSQL数据库的相关操作，这里就可以作为扩展注册）
        /// </summary>
        private static void RegisterMore(ContainerBuilder builder, Action<ContainerBuilder> register = null)
        {
            register?.Invoke(builder);
        }

        #endregion

        /// <summary>
        /// Create, bind and register as service the specified configuration parameters 
        /// </summary>
        /// <typeparam name="TConfig">Configuration parameters</typeparam>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Set of key/value application configuration properties</param>
        /// <returns>Instance of configuration parameters</returns>
        public static TConfig ConfigureStartupConfig<TConfig>(this IServiceCollection services, IConfiguration configuration) where TConfig : class, new()
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            //create instance of config
            var config = new TConfig();

            //bind it to the appropriate section of configuration
            configuration.Bind(config);

            //and register it as a service
            services.AddSingleton(config);

            return config;
        }
    }
}
