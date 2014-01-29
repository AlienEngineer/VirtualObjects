using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VirtualObjects.Queries.Execution
{
    /// <summary>
    /// Responsable for the query execution.
    /// </summary>
    public interface IQueryExecutor
    {
        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        object ExecuteQuery(Expression query, Context context);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="context"></param>
        /// <returns></returns>
        TResult ExecuteQuery<TResult>(Expression query, Context context);

        /// <summary>
        /// Determines whether this instance can execute the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        bool CanExecute(MethodInfo method);
    }
}