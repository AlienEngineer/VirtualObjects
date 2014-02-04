using System;
using System.Linq;
using VirtualObjects.Core.Connection;

namespace VirtualObjects
{
    public class Session : ISession
    {
        ISession _session;

        public Session()
            : this(configuration: null, connectionName: null)
        {

        }

        public Session(SessionConfiguration configuration = null, IDbConnectionProvider connectionProvider = null)
            : this(new NinjectContainer(configuration, connectionProvider))
        {

        }

        public Session(SessionConfiguration configuration = null, String connectionName = null)
            : this(new NinjectContainer(configuration, connectionName))
        {

        }

        public Session(IOcContainer container)
        {
            _session = container.Get<ISession>();
        }

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return _session.GetAll<TEntity>();
        }

        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return _session.GetById(entity);
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return _session.Insert(entity);
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return _session.Update(entity);
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return _session.Delete(entity);
        }

        public ITransaction BeginTransaction()
        {
            return _session.BeginTransaction();
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
                    _session.Dispose();
                }

                _session = null;
                _disposed = true;
            }
        }

        #endregion

    }




}