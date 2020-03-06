using System;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using DapperRepository.Core.Cache;
using DapperRepository.Core.Configuration;
using DapperRepository.Core.Constants;

namespace DapperRepository.Web.Infrastructure
{
    public class Bootstrapper
    {
        public static void Run()
        {
            SetAutofacContainer();
        }

        private static void SetAutofacContainer()
        {
            DapperRepositoryConfig config = ConfigurationManager.GetSection("DapperRepositoryConfig") as DapperRepositoryConfig;

            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterInstance(config).As<DapperRepositoryConfig>().SingleInstance();

            Assembly assembly = Assembly.GetExecutingAssembly();

            builder.RegisterControllers(assembly);

            if (config != null && config.RedisEnabled)
            {
                RegisterMore(builder, x =>
                {
                    x.RegisterType<RedisConnectionWrapper>().As<IRedisConnectionWrapper>().SingleInstance();
                    x.RegisterType<RedisCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
                });
            }

            if (config != null)
            {
                switch (config.CurrentDbTypeName)
                {
                    case ConnKeyConstants.Mssql:

                        // Repositories
                        RegisterRepository(builder, ConnKeyConstants.Mssql);

                        // Services
                        RegisterService(builder, ConnKeyConstants.Mssql);

                        RegisterMore(builder, b =>
                        {

                        });

                        break;

                    case ConnKeyConstants.Mysql:

                        // Repositories
                        RegisterRepository(builder, ConnKeyConstants.Mysql);

                        // Services
                        RegisterService(builder, ConnKeyConstants.Mysql);

                        break;

                    case ConnKeyConstants.Oracle:

                        // configure it by yourself if you use oracle

                        break;
                }
            }

            IContainer container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterRepository(ContainerBuilder builder, string registerDbTypeName)
        {
            string namespacePrefix = $"DapperRepository.Data.Repositories.{registerDbTypeName}";

            builder.RegisterAssemblyTypes(Assembly.Load("DapperRepository.Data"))
                            .Where(x => x.Namespace != null && x.Namespace.StartsWith(namespacePrefix)
                                && x.Name.EndsWith("Repository"))
                            .AsImplementedInterfaces()
                            .InstancePerLifetimeScope();
        }

        private static void RegisterService(ContainerBuilder builder, string registerDbTypeName)
        {
            string namespacePrefix = $"DapperRepository.Services.{registerDbTypeName}";

            builder.RegisterAssemblyTypes(Assembly.Load("DapperRepository.Services"))
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
    }
}