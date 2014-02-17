using System;
using System.Linq;
using VirtualObjects.Queries.Annotations;
using ArgumentNullException = VirtualObjects.Exceptions.ArgumentNullException;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// </summary>
    public class InternalSession : ISession
    {
        internal SessionContext Context { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSession"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public InternalSession(SessionContext context)
        {
            Context = context;
            context.Session = this;
        }

        /// <summary>
        /// Gets all entities of TEntity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return Context.QueryProvider.CreateQuery<TEntity>(null);
        }

        /// <summary>
        /// Gets the entity by its ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return entity == null ? null 
                : ExecuteOperation(Context.Map<TEntity>().Operations.GetOperation, entity);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if ( entity == null ) throw new ArgumentNullException(Errors.Session_EntityNotSupplied);

            return ExecuteOperation(Context.Map<TEntity>().Operations.InsertOperation, entity);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public TEntity Update<TEntity>([NotNull] TEntity entity) where TEntity : class, new()
        {
            if (entity == null) throw new ArgumentNullException(Errors.Session_EntityNotSupplied);

            return ExecuteOperation(Context.Map<TEntity>().Operations.UpdateOperation, entity);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            if ( entity == null ) throw new ArgumentNullException(Errors.Session_EntityNotSupplied);

            return ExecuteOperation(Context.Map<TEntity>().Operations.DeleteOperation, entity) != null;
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            return Context.Connection.BeginTransaction();
        }

        private TEntity ExecuteOperation<TEntity>(IOperation operation, TEntity entityModel)
        {
            return (TEntity)operation.PrepareOperation(entityModel).Execute(Context);
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
                    Context.Dispose();
                }

                Context = null;

                _disposed = true;
            }
        }

        #endregion
    }
}