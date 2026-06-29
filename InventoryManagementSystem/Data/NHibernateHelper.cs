using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using InventoryManagementSystem.Mapping;

namespace InventoryManagementSystem.Data
{
    public static class NHibernateHelper
    {
        public static ISessionFactory CreateSessionFactory(string connectionString)
        {
            try
            {
                return Fluently.Configure()
                    .Database(MsSqlConfiguration.MsSql2012
                        .ConnectionString(connectionString)
                        .Driver<NHibernate.Driver.MicrosoftDataSqlClientDriver>())
                    .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ProductMap>())
                    .ExposeConfiguration(cfg =>
                    {
                        new SchemaUpdate(cfg).Execute(false, true);
                    })
                    .BuildSessionFactory();
            }
            catch (Exception ex)
            {
               
                var realError = ex;
                while (realError.InnerException != null)
                {
                    realError = realError.InnerException;
                }

              
                System.Diagnostics.Debug.WriteLine("=== NHIBERNATE CRASH REASON ===");
                System.Diagnostics.Debug.WriteLine(realError.Message);
                System.Diagnostics.Debug.WriteLine("===============================");

                throw new Exception($"NHibernate Mapping Error: {realError.Message}", ex);
            }
        }
    }
}