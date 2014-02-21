using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace VirtualObjects.Tests.Scaffold
{
    using NUnit.Framework;
    using System.Data;
    using System.Data.SqlClient;
    using System.Data.Common;
    using VirtualObjects.Mappings;

    [TestFixture, Category("VirtualObjectsHelper")]
    public class VirtualObjectsHelperTests : UtilityBelt
    {

        [Table(TableName = "sys.tables")]
        public class Table
        {
            [Key(FieldName="Object_Id")]
            public int Id { get; set; }

            public String Name { get; set; }

            public String Type { get; set; }

            public virtual IQueryable<Column> Columns { get; set; }
        }

        [Table(TableName = "sys.columns")]
        public class Column
        {
            [Key]
            [Association(FieldName = "object_id", OtherKey = "Id")]
            public virtual Table Table { get; set; }

            [Key(FieldName="column_Id")]
            public long Id { get; set; }

            [Key]
            public String Name { get; set; }

            [Column(FieldName = "system_type_id")]
            public int Type { get; set; }
        }

        [Test]
        public void Helper_Can_Produce_TablesInformation()
        {
            using ( var session = new Session(
                configuration: new SessionConfiguration { Logger = Console.Out }, 
                connectionProvider: new Connections.DbConnectionProvider("System.Data.SqlClient", "Data Source=.\\Development;Initial Catalog=AimirimTeste;Integrated Security=True")) )
            {
            
                foreach ( var table in session.Query<Table>().Where(e => e.Type =="U"))
                {

                    Console.WriteLine("TableName: {0}", table.Name);

                    foreach ( var column in table.Columns )
                    {
                        Console.WriteLine("     ColumnName: {0}", column.Name);
                    }
                }

            } 
        }

        private static Boolean IsPrimaryKey(DataTable table)
        {
            foreach ( DataRow r in table.Rows )
            {
                foreach ( DataColumn c in table.Columns )
                {
                    if ( c.ColumnName == "KeyType" )
                    {
                        return Convert.ToInt32(r[c]) == 56;
                    }
                }
            }
            return false;
        }

        private static void DisplayData(System.Data.DataTable table)
        {
            foreach ( System.Data.DataRow row in table.Rows )
            {
                foreach ( System.Data.DataColumn col in table.Columns )
                {
                    Console.WriteLine("{0} = {1}", col.ColumnName, row[col]);
                }
                Console.WriteLine("============================ {0}", table.Columns.Count);
            }
        }

    }
}
