using System.Data;

namespace VirtualObjects.Connections
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
