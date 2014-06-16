using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.NonQueries;

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
        /// Gets how many entities existe of the given TEntity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        int Count<TEntity>();

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
        /// Starts the building of an Update operation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        IUpdate<TEntity> Update<TEntity>();

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

        /// <summary>
        /// Executes the store procedure.
        /// </summary>
        /// <param name="storeProcedure">The store procedure.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        int ExecuteStoreProcedure(String storeProcedure, IEnumerable<KeyValuePair<String, Object>> args);
    }
}
