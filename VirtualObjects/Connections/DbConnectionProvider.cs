using System;
using System.Data;
using System.Data.Common;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class DbConnectionProvider : IDbConnectionProvider
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        protected string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>
        /// The name of the provider.
        /// </value>
        protected string ProviderName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionProvider"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public DbConnectionProvider(string providerName, string connectionString)
        {
            ProviderName = providerName;
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="VirtualObjects.Exceptions.ConnectionProviderException"></exception>
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