using System;
using System.Linq;
using VirtualObjects.Config;
using VirtualObjects.Programability;
using VirtualObjects.Queries;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// Context used by the ORM.
    /// 
    /// </summary>
    public class SessionContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the query provider.
        /// </summary>
        /// <value>
        /// The query provider.
        /// </value>
        public IQueryProvider QueryProvider { get; set; }

        /// <summary>
        /// Gets or sets the programability.
        /// </summary>
        /// <value>
        /// The programability.
        /// </value>
        public IProgramability Programability { get; set; }

        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public IConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ISession Session { get; set; }

        /// <summary>
        /// Gets or sets the map.
        /// </summary>
        /// <value>
        /// The map.
        /// </value>
        public Func<Type, IEntityInfo> Map { get; set; }

        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        public IMapper Mapper { get; set; }

        /// <summary>
        /// Gets or sets the translator.
        /// </summary>
        /// <value>
        /// The translator.
        /// </value>
        public IQueryTranslator Translator { get; set; }

        #region IDisposable Members
        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed )
            {
                if ( disposing )
                {
                    Connection.Dispose();
                }

                Connection = null;
                QueryProvider = null;

                _disposed = true;
            }
        }

        #endregion
    }
}
