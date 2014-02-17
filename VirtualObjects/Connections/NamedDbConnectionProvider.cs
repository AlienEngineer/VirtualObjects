using System;
using System.Configuration;
using System.Data;
using System.Linq;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class NamedDbConnectionProvider : DbConnectionProvider
    {
        private readonly string _connectionName;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDbConnectionProvider"/> class.
        /// </summary>
        public NamedDbConnectionProvider()
            : this(Environment.MachineName)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedDbConnectionProvider"/> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        public NamedDbConnectionProvider(string connectionName)
            : base(null, null)
        {
            _connectionName = connectionName ?? Environment.MachineName;
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="VirtualObjects.Exceptions.ConnectionProviderException"></exception>
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