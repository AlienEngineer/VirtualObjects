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
        /// <param name="expression">The expression.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        object ExecuteQuery(Expression expression, SessionContext context);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        TResult ExecuteQuery<TResult>(Expression expression, SessionContext context);

        /// <summary>
        /// Determines whether this instance can execute the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        bool CanExecute(MethodInfo method);
    }
}