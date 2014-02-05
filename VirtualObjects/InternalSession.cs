using System;
using System.Linq;
using VirtualObjects.Queries.Annotations;
using ArgumentNullException = VirtualObjects.Exceptions.ArgumentNullException;

namespace VirtualObjects
{
    public class InternalSession : ISession
    {
        internal SessionContext Context { get; private set; }

        public InternalSession(SessionContext context)
        {
            Context = context;
            context.Session = this;
        }

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return Context.QueryProvider.CreateQuery<TEntity>(null);
        }

        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return entity == null ? null 
                : ExecuteOperation(Context.Map<TEntity>().Operations.GetOperation, entity);
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if ( entity == null ) throw new ArgumentNullException(Errors.Session_EntityNotSupplied);

            return ExecuteOperation(Context.Map<TEntity>().Operations.InsertOperation, entity);
        }

        public TEntity Update<TEntity>([NotNull] TEntity entity) where TEntity : class, new()
        {
            if (entity == null) throw new ArgumentNullException(Errors.Session_EntityNotSupplied);

            return ExecuteOperation(Context.Map<TEntity>().Operations.UpdateOperation, entity);
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if ( entity == null ) throw new ArgumentNullException(Errors.Session_EntityNotSupplied);

            return ExecuteOperation(Context.Map<TEntity>().Operations.DeleteOperation, entity) != null;
        }

        public ITransaction BeginTransaction()
        {
            return Context.Connection.BeginTranslation();
        }

        private TEntity ExecuteOperation<TEntity>(IOperation operation, TEntity entityModel)
        {
            return (TEntity)operation.PrepareOperation(entityModel).Execute(Context);
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
                    Context.Dispose();
                }

                Context = null;

                _disposed = true;
            }
        }

        #endregion
    }
}