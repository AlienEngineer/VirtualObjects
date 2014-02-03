using System.Data;

namespace VirtualObjects.Core.Connection
{
    /// <summary>
    /// Connection Provider.
    /// </summary>
    public interface IDbConnectionProvider
    {
        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();
    }
}
