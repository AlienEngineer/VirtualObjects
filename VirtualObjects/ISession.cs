using System;
using System.Linq;

namespace VirtualObjects
{
    /// <summary>
    /// Represents a connection session.
    /// </summary>
    public interface ISession : IDisposable
    {

        /// <summary>
        /// Gets all entities of TEntity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new();

        /// <summary>
        /// Gets the entity by its ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new();

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new();

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        TEntity Update<TEntity>(TEntity entity) where TEntity : class, new();

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Boolean Delete<TEntity>(TEntity entity) where TEntity : class, new();

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        ITransaction BeginTransaction();
    }

    public static class SessionExtensions
    {

        public static TResult WithinTransaction<TResult>(this ISession session, Func<TResult> execute)
        {
            var transaction = session.BeginTransaction();
            try
            {
                return execute();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                transaction.Commit();
            }
        }

        public static void WithinTransaction(this ISession session, Action execute)
        {
            session.WithinTransaction<Object>(() => { execute(); return null; });
        }

        /// <summary>
        /// Keeps the session alive. Doesn't close the connection to the database after each operation is complete.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="execute">The execute.</param>
        /// <returns></returns>
        public static TResult KeepAlive<TResult>(this ISession session, Func<TResult> execute)
        {
            var internalSession = (InternalSession)((Session) session).InternalSession;
            try
            {
                internalSession.Context.Connection.KeepAlive = true;
                return execute();
            }
            finally
            {
                internalSession.Context.Connection.KeepAlive = false;
            }
        }

        /// <summary>
        /// Keeps the session alive. Doesn't close the database connection after each operation is complete.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="execute">The execute.</param>
        public static void KeepAlive(this ISession session, Action execute)
        {
            session.KeepAlive<Object>(() => { execute(); return null; });
        }

    }

}
