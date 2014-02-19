using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Scaffold
{
    public static class VirtualObjectsHelper
    {
        public static IEnumerable<Table> GetTables()
        {
            const string serverName = @".\development";
            const string databaseName = @"northwind";
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
