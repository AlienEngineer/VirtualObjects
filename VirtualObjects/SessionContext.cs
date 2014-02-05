using System;
using System.Linq;
using Ninject;
using VirtualObjects.Config;

namespace VirtualObjects
{
    public class SessionContext : IDisposable
    {
        [Inject]
        public IQueryProvider QueryProvider { get; set; }
        [Inject]
        public IMapper Mapper { get; set; }
        [Inject]
        public IConnection Connection { get; set; }

        public ISession Session { get; set; }

        public IEntityInfo Map<TEntity>()
        {
            return Mapper.Map(typeof (TEntity));
        }

        #region IDisposable Members
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

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
