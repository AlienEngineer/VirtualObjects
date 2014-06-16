
using System;
using System.Linq;
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
        /// Wheres the specified clause.
        /// </summary>
        /// <param name="clause">The clause.</param>
        INonQuery<TEntity> Where(Func<IQueryable<TEntity>, IQueryable<TEntity>> clause);


        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns>The number of entities afected by this operation.</returns>
        int Execute();
    }
}
