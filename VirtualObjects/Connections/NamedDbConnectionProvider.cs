using System;
using System.Configuration;
using System.Data;
using System.Linq;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Connections
{
    public class NamedDbConnectionProvider : DbConnectionProvider
    {
        private readonly string _connectionName;

        public NamedDbConnectionProvider()
            : this(Environment.MachineName)
        {

        }

        public NamedDbConnectionProvider(string connectionName)
            : base(null, null)
        {
            _connectionName = connectionName ?? Environment.MachineName;
        }

        public override IDbConnection CreateConnection()
        {
            var settings = ConfigurationManager.ConnectionStrings;

            var connectionString = settings[_connectionName];

            if ( connectionString == null )
            {
                throw new ConnectionProviderException(Errors.ConnectionProvider_UnableToFindConnectionName, new { ConnectionName = _connectionName });
            }

            ConnectionString = connectionString.ConnectionString;
            ProviderName = connectionString.ProviderName;

            return base.CreateConnection();
        }
    }
}