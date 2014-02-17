using System;
using System.Linq;
using Ninject;
using VirtualObjects.Config;

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
        [Inject]
        public IQueryProvider QueryProvider { get; set; }
        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        /// <value>
        /// The mapper.
        /// </value>
        [Inject]
        public IMapper Mapper { get; set; }
        /// <summary>
        /// Gets or sets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        [Inject]
        public IConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        public ISession Session { get; set; }

        /// <summary>
        /// Maps the type of TEntity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IEntityInfo Map<TEntity>()
        {
            return Mapper.Map(typeof (TEntity));
        }

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
                    Mapper.Dispose();
                }

                Connection = null;
                Mapper = null;
                QueryProvider = null;

                _disposed = true;
            }
        }

        #endregion
    }
}
