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

            RegisterMore(builder, x =>
            {
                x.RegisterType<RedisConnectionWrapper>().As<IRedisConnectionWrapper>().SingleInstance();
                x.RegisterType<RedisCacheManager>().As<ICacheManager>().InstancePerLifetimeScope();
            });

            if (config != null)
            {
                switch (config.ActivedDbTypeName)
                {
                    case ConnKeyConstants.Mssql:

                        // Repositories
                        RegisterRepository(builder, ConnKeyConstants.Mssql);

                        // Services
                        RegisterSevice(builder, ConnKeyConstants.Mssql);

                        RegisterMore(builder, b =>
                        {

                        });

                        break;

                    case ConnKeyConstants.Mysql:

                        // Repositories
                        RegisterRepository(builder, ConnKeyConstants.Mysql);

                        // Services
                        RegisterSevice(builder, ConnKeyConstants.Mysql);

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
            string namespacePrefix = string.Format("DapperRepository.Data.Repositories.{0}", registerDbTypeName);

            builder.RegisterAssemblyTypes(Assembly.Load("DapperRepository.Data"))
                            .Where(x => x.Namespace != null && x.Namespace.StartsWith(namespacePrefix)
                                && x.Name.EndsWith("Repository"))
                            .AsImplementedInterfaces()
                            .InstancePerLifetimeScope();
        }

        private static void RegisterSevice(ContainerBuilder builder, string registerDbTypeName)
        {
            string namespacePrefix = string.Format("DapperRepository.Services.{0}", registerDbTypeName);

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
            if (register != null)
                register(builder);
        }
    }
}