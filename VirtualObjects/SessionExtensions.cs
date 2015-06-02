using System;
using System.Data;
using System.Linq;

namespace VirtualObjects
{
    /// <summary>
    /// Extension methods for the ISession interface.
    /// </summary>
    public static class SessionExtensions
    {

        /// <summary>
        /// Executes an operation Within a transaction.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="execute">The execute.</param>
        /// <returns></returns>
        public static TResult WithinTransaction<TResult>(this ISession session, Func<ITransaction, TResult> execute)
        {
            var transaction = session.BeginTransaction();
            try
            {
                return execute(transaction);
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

        /// <summary>
        /// Executes an operation Within a transaction.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="isolation">The isolation.</param>
        /// <param name="execute">The execute.</param>
        /// <returns></returns>
        public static TResult WithinTransaction<TResult>(this ISession session, IsolationLevel isolation, Func<ITransaction, TResult> execute)
        {
            var transaction = session.BeginTransaction(isolation);
            try
            {
                return execute(transaction);
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

        /// <summary>
        /// Executes an operation Within a transaction.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="execute">The execute.</param>
        public static void WithinTransaction(this ISession session, Action<ITransaction> execute)
        {
            session.WithinTransaction<object>(transaction => { execute(transaction); return null; });
        }

        /// <summary>
        /// Executes an operation Within a transaction.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="isolation">The isolation.</param>
        /// <param name="execute">The execute.</param>
        public static void WithinTransaction(this ISession session, IsolationLevel isolation, Action<ITransaction> execute)
        {
            session.WithinTransaction<object>(isolation, transaction => { execute(transaction); return null; });
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
                internalSession.Context.Connection.Close();
            }
        }

        /// <summary>
        /// Keeps the session alive. Doesn't close the database connection after each operation is complete.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="execute">The execute.</param>
        public static void KeepAlive(this ISession session, Action execute)
        {
            session.KeepAlive<object>(() => { execute(); return null; });
        }

        /// <summary>
        /// Queries the session returning a query of TEntity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static IQueryable<TEntity> Query<TEntity>(this ISession session) where TEntity : class, new()
        {
            return session.GetAll<TEntity>();
        }

        /// <summary>
        /// Counts how many ocorrencies of the type TEntity exists in the specified session.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="session">The session.</param>
        /// <returns></returns>
        public static int Count<TEntity>(this ISession session) where TEntity : class, new()
        {
            return session.GetAll<TEntity>().Count();
        }

        /// <summary>
        /// Verifies if the given entity exists in the data source.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static bool Exists<TEntity>(this ISession session, TEntity entity) where TEntity : class, new()
        {
            return session.GetById(entity) != null;
        }

        /// <summary>
        /// Executes an operation with a rollback operation at the end. Use this for unit-testing.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="execute">The execute.</param>
        /// <returns></returns>
        public static TResult WithRollback<TResult>(this ISession session, Func<TResult> execute)
        {
            var transaction = session.BeginTransaction();
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

        /// <summary>
        /// Executes an operation with a rollback operation at the end. Use this for unit-testing.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="execute">The execute.</param>
        public static void WithRollback(this ISession session, Action execute)
        {
            session.WithRollback<object>(() =>
            {
                execute(); return null;
            });
        }
    }
}