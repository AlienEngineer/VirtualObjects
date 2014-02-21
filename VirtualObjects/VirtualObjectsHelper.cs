using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Scaffold
{
    /// <summary>
    /// 
    /// </summary>
    public static class VirtualObjectsHelper
    {
        /// <summary>
        /// Gets the tables.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="serverName">Name of the server.</param>
        /// <returns></returns>
        public static IEnumerable<Table> GetTables(string databaseName, string serverName)
        {
            Server server = new Server(serverName);
            Database database = new Database(server, databaseName);
            database.Refresh();



            foreach ( Table table in database.Tables )
            {
                yield return table;

            }

        }

    }

}
