using System;
using System.Linq;

namespace VirtualObjects.Tests.Repositories 
{
    public static class RepositoryExtensions
    {

        #region IRepository Helpers

        public static TResult WithinTransaction<TResult>(this IRepository repository, Func<TResult> execute)
        {
            var repo = repository as Repository;

            if (repo == null)
            {
                throw new VirtualObjectsException("WithinTransaction only supports Repository instances.");
            }

            return repo.Session.WithinTransaction(trans => execute());
        }

        public static void WithinTransaction(this IRepository repository, Action execute)
        {
            repository.WithinTransaction<object>(() =>
            {
                execute();
                return null;
            });
        }
		

        public static TResult WithinTransaction<TResult>(this IRepository repository, Func<ITransaction, TResult> execute)
        {
            var repo = repository as Repository;

            if (repo == null)
            {
                throw new VirtualObjectsException("WithinTransaction only supports Repository instances.");
            }

            return repo.Session.WithinTransaction(execute);
        }


        /// <summary>
        /// Withins the transaction.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="execute">The execute.</param>
        public static void WithinTransaction(this IRepository repository, Action<ITransaction> execute)
        {
            repository.WithinTransaction<object>(trans =>
            {
                execute(trans);
                return null;
            });
        }

        /// <summary>
        /// Keeps the session alive. Doesn't close the connection to the database after each operation is complete.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="execute">The execute.</param>
        /// <returns></returns>
        public static TResult KeepAlive<TResult>(this IRepository repository, Func<TResult> execute)
        {
            var repo = repository as Repository;

            if (repo == null)
            {
                throw new VirtualObjectsException("KeepAlive only supports Repository instances.");
            }

            return repo.Session.KeepAlive(execute);
        }

        /// <summary>
        /// Keeps the session alive. Doesn't close the database connection after each operation is complete.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="execute">The execute.</param>
        public static void KeepAlive(this IRepository repository, Action execute)
        {
            repository.KeepAlive<object>(() =>
            {
                execute();
                return null;
            });
        }

        public static IQueryable<TEntity> Query<TEntity>(this IRepository repository) where TEntity : class, new()
        {
            return repository.GetAll<TEntity>();
        }

        public static int Count<TEntity>(this IRepository repository) where TEntity : class, new()
        {
            return repository.GetAll<TEntity>().Count();
        }

        public static bool Exists<TEntity>(this IRepository repository, TEntity entity) where TEntity : class, new()
        {
            return repository.GetById(entity) != null;
        }

        public static TResult WithRollback<TResult>(this IRepository repository, Func<TResult> execute)
        {
            var transaction = repository.BeginTransaction();
            try
            {
                return execute();
            }
            catch ( Exception )
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Rollback();
            }
        }

        public static void WithRollback(this IRepository repository, Action execute)
        {
            repository.WithRollback<object>(() =>
            {
                execute(); return null;
            });
        }
        #endregion
    }
}