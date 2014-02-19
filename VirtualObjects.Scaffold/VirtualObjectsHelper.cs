using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Scaffold
{
    public static class VirtualObjectsHelper
    {
        public static IEnumerable<Table> GetTables(string databaseName, string serverName)
        {
            Server server = new Server(serverName);
            Database database = new Database(server, databaseName);
            database.Refresh();

            

            foreach ( Table table in database.Tables )
            {
                yield return table;
                //yield return new MetaTable
                //{
                //    Name = table.Name,
                //    Columns = ParseColumns(table)
                //};
            }
        }

        private static IEnumerable<MetaColumn> ParseColumns(Table table)
        {
            foreach ( Column column in table.Columns )
            {
                
                yield return new MetaColumn
                {
                    Name = column.Name,
                    InPrimaryKey = column.InPrimaryKey,
                    DataType = column.DataType.ToString(),
                    Identity = column.Identity,
                    IsForeignKey = column.IsForeignKey
                };
            }
        }

    }

    [Serializable]
    public class MetaTable
    {
        public String Name { get; set; }
        public IEnumerable<MetaColumn> Columns { get; set; }
    }

    [Serializable]
    public class MetaColumn
    {
        public String Name { get; set; }
        public Boolean InPrimaryKey { get; set; }
        public bool Identity { get; set; }
        public bool IsForeignKey { get; set; }
        public String DataType { get; set; }
    }
}
