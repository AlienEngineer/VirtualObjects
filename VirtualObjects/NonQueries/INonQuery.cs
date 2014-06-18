
using System;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Queries;

namespace VirtualObjects.NonQueries
{
    /// <summary>
    /// Represents a non query operation.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface INonQuery<TEntity>
    {

        /// <summary>
        /// Translates this instance.
        /// </summary>
        /// <returns></returns>
        IQueryInfo Translate();

        /// <summary>
        /// Wheres the specified expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        INonQuery<TEntity> Where(Expression<Func<TEntity, Boolean>> expression);

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>The number of entities afected by this operation.</returns>
        int Execute();
    }
}
