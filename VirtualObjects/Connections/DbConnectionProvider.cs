using System;
using System.Data;
using System.Data.Common;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Connections
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        protected string ConnectionString { get; set; }
        protected string ProviderName { get; set; }

        public DbConnectionProvider(string providerName, string connectionString)
        {
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        public virtual IDbConnection CreateConnection()
        {
            try
            {
                var factory = DbProviderFactories.GetFactory(ProviderName);
                var connection = factory.CreateConnection();

                connection.ConnectionString = ConnectionString;
                return connection;
            }
            catch ( Exception ex )
            {
                throw new ConnectionProviderException(Errors.ConnectionProvider_UnableToCreate, new
                {
                    ex.Message,
                    ProviderName,
                    ConnectionString
                });
            }
        }
    }
}