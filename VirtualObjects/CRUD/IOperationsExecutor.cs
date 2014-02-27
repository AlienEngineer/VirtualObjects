using System;

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
        Object Insert(Object entityModel, SessionContext sessionContext);
        /// <summary>
        /// Updates the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        Object Update(Object entityModel, SessionContext sessionContext);
        /// <summary>
        /// Deletes the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        Object Delete(Object entityModel, SessionContext sessionContext);
        /// <summary>
        /// Gets the specified entity model.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        Object Get(Object entityModel, SessionContext sessionContext);

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <param name="entityModel">The entity model.</param>
        /// <param name="sessionContext">The session context.</param>
        /// <returns></returns>
        byte[] GetVersion(Object entityModel, SessionContext sessionContext);
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
        Byte[] GetVersion(Object entityModel, SessionContext sessionContext);
    }
}
