using System;
using System.Linq.Expressions;

namespace VirtualObjects.NonQueries
{
    /// <summary>
    /// Represents an Update command.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IUpdate<TEntity> : INonQuery<TEntity>
    {

        /// <summary>
        /// Sets the specified field getter.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="fieldGetter">The field getter.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        IUpdate<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> fieldGetter, TValue value);

        /// <summary>
        /// Sets the specified field getter.
        /// </summary>
        /// <typeparam name="TJoinedEntity">The type of the joined entity.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="fieldGetter">The field getter.</param>
        /// <param name="joinedFieldGetter">The joined field getter.</param>
        /// <returns></returns>
        IUpdate<TEntity> Set<TJoinedEntity, TValue>(Expression<Func<TEntity, TValue>> fieldGetter, Expression<Func<TJoinedEntity, TValue>> joinedFieldGetter);

    }
}