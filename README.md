Dapper and Repository Pattern in MVC 
=

    First add a new database named DapperRepositoryDb and execute the sql script which you need(mssql or mysql). 
    If you are using oracle,sorry for you have to write those codes to support it. 
    Thanks.

## Define the database connection string 
the file path:~/App_Data/DbConnSettings.json

      {

         "MssqlMasterKey": "Data Source=.;Initial Catalog=DapperRepositoryDb;Integrated Security=True;",

         "MysqlMasterKey": "your mysql connection string" 

      }
 you can add more for your project.
 
 ## Switch the database
     you only update the Web.config file to switch the database for your current project.
     such as update the "ActivedDbTypeName" value from mysql to mssql,then your project using the mssql now.
     
 ## Support multiple type of databases at the same time
     for some reasons you would need two of more type of databases in your project at the same time.
     like you are using the mssql now,and you still wanna use the mysql in it.
     so you can using the RegisterMore method to do this.
 the method:
 ``` c#
        /// <summary>
        /// 注册更多（比如当前如果使用的MSSQL数据库，那有可能还需要MYSQL数据库的相关操作，这里就可以作为扩展注册）
        /// </summary>
        private static void RegisterMore(ContainerBuilder builder, Action<ContainerBuilder> register = null)
        {
            if (register != null)
                register(builder);
        }
 ```
 example:
 ``` c#
        case ConnKeyConstants.Mssql:

        // Repositories
        RegisterRepository(builder, ConnKeyConstants.Mssql);

        // Services
        RegisterSevice(builder, ConnKeyConstants.Mssql);
        
        RegisterMore(builder, b =>
                        {
                           // register what you need
                           b.RegisterType<youmysqlclass>().As<youmysqlinterface>().InstancePerLifetimeScope();
                        });

        break;
 ```
 In this way your class should be inherit from MysqlRepositoryBase<T>,and you can ovvrride the "ConnStrKey" for which of the mysql database you need.
 

Demo Address: http://dapperrepository.coolwecool.com

All contents of this package are licensed under the [MIT license](https://opensource.org/licenses/MIT).
