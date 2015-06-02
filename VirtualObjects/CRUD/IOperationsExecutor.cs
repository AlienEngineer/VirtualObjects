namespace VirtualObjects.CRUD
{
    /// <summary>
    /// 
    /// </summary>
    public interface IOperationsExecutor
    {
        /// <summary>
        /// Inserts the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        object Insert(object entityModel, SessionContext sessionContext);
        /// <summary>
        /// Updates the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        object Update(object entityModel, SessionContext sessionContext);
        /// <summary>
        /// Deletes the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        object Delete(object entityModel, SessionContext sessionContext);
        /// <summary>
        /// Gets the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        object Get(object entityModel, SessionContext sessionContext);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        byte[] GetVersion(object entityModel, SessionContext sessionContext);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOperationsExecutor<T>
    {
        /// <summary>
        /// Inserts the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        T Insert(T entityModel, SessionContext sessionContext);
        /// <summary>
        /// Updates the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        T Update(T entityModel, SessionContext sessionContext);
        /// <summary>
        /// Deletes the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        T Delete(T entityModel, SessionContext sessionContext);
        /// <summary>
        /// Gets the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        T Get(T entityModel, SessionContext sessionContext);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        byte[] GetVersion(object entityModel, SessionContext sessionContext);
    }
}
