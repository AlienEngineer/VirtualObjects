using System.Linq;

namespace VirtualObjects
{
    public class Session : ISession
    {
        readonly ISession _session;

        public Session()
            : this(new NinjectContainer())
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
    }
}