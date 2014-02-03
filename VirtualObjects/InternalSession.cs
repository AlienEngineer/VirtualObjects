using System.Linq;

namespace VirtualObjects
{
    public class InternalSession : ISession
    {
        private readonly SessionContext _context;

        public InternalSession(SessionContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return _context.QueryProvider.CreateQuery<TEntity>(null);
        }

        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExecuteOperation(_context.Map<TEntity>().Operations.GetOperation, entity);
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExecuteOperation(_context.Map<TEntity>().Operations.InsertOperation, entity);
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExecuteOperation(_context.Map<TEntity>().Operations.UpdateOperation, entity);
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExecuteOperation(_context.Map<TEntity>().Operations.DeleteOperation, entity) != null;
        }

        public ITransaction BeginTransaction()
        {
            return _context.Connection.BeginTranslation();
        }

        private TEntity ExecuteOperation<TEntity>(IOperation operation, TEntity entityModel)
        {
            return (TEntity)operation.PrepareOperation(entityModel).Execute(_context.Connection);
        }
    }
}