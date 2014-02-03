using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Core.Connection
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

    class NamedDbConnectionProvider : DbConnectionProvider
    {
        private readonly string _connectionName;

        public NamedDbConnectionProvider(string connectionName) 
            : base(null, null)
        {
            _connectionName = connectionName;
        }

        public override IDbConnection CreateConnection()
        {
            var settings = ConfigurationManager.ConnectionStrings;

            var connectionString = settings[_connectionName];

            if (connectionString == null)
            {
                throw  new ConnectionProviderException(Errors.ConnectionProvider_UnableToFindConnectionName, new { ConnectionName = _connectionName });
            }

            ConnectionString = connectionString.ConnectionString;
            ProviderName = connectionString.ProviderName;

            return base.CreateConnection();
        }
    }
}