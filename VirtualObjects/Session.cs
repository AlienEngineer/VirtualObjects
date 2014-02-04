using System;
using System.Linq;
using VirtualObjects.Core.Connection;

namespace VirtualObjects
{
    public class Session : ISession
    {

        internal ISession InternalSession { get; private set; }

        public Session()
            : this(configuration: null, connectionName: null) { }

        public Session(SessionConfiguration configuration = null, IDbConnectionProvider connectionProvider = null)
            : this(new NinjectContainer(configuration, connectionProvider)) { }

        public Session(SessionConfiguration configuration = null, String connectionName = null)
            : this(new NinjectContainer(configuration, connectionName)) { }

        public Session(IOcContainer container)
        {
            InternalSession = container.Get<ISession>();
        }

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return InternalSession.GetAll<TEntity>();
        }

        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.GetById(entity);
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Insert(entity);
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Update(entity);
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Delete(entity);
        }

        public ITransaction BeginTransaction()
        {
            return InternalSession.BeginTransaction();
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
                    InternalSession.Dispose();
                }

                InternalSession = null;
                _disposed = true;
            }
        }

        #endregion

    }




}