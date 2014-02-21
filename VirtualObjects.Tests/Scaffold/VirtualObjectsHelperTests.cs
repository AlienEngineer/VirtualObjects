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
            [Key(FieldName = "Object_Id")]
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

            [Key(FieldName = "column_Id")]
            public int Id { get; set; }

            public String Name { get; set; }

            [Column(FieldName = "is_identity")]
            public Boolean IsIdentity { get; set; }

            [Column(FieldName = "system_type_id")]
            public int Type { get; set; }

        }

        [Table(TableName = "sys.Index_Columns")]
        public class IndexColumn
        {
            [Key]
            [Association(FieldName = "object_id", OtherKey = "Id")]
            public virtual Table Table { get; set; }

            [Key]
            [Association(FieldName = "column_id", OtherKey = "Id")]
            public virtual Column Column { get; set; }

            [Association(FieldName = "index_Id", OtherKey = "Id")]
            public virtual Index Index { get; set; }
        }

        [Table(TableName = "sys.Indexes")]
        public class Index
        {
            [Key(FieldName = "index_Id")]
            public int Id { get; set; }

            [Key]
            [Association(FieldName = "object_id", OtherKey = "Id")]
            public virtual Table Table { get; set; }

            [Column(FieldName = "is_primary_key")]
            public Boolean IsPrimaryKey { get; set; }
        }

        [Test]
        public void Helper_Can_Produce_TablesInformation()
        {
            using ( var session = new Session(
                configuration: new SessionConfiguration
                {
                    //    Logger = Console.Out 
                },
                connectionProvider: new Connections.DbConnectionProvider("System.Data.SqlClient", "Data Source=.\\Development;Initial Catalog=AimirimTeste;Integrated Security=True")) )
            {

                foreach ( var table in session.Query<Table>().Where(e => e.Type == "U") )
                {

                    Console.WriteLine("TableName: {0}", table.Name);

                    foreach ( var column in table.Columns )
                    {
                        var buff = new StringBuffer();

                        buff += "     ColumnName: {0, -20}";

                        if ( column.IsIdentity )
                        {
                            buff += " IsIdentity ";
                        }
                        else
                        {
                            var indexInfo = session.Query<IndexColumn>().FirstOrDefault(e => e.Column == column && e.Table == table);

                            if ( indexInfo != null && indexInfo.Index.IsPrimaryKey )
                            {
                                buff += " IsKey ";
                            }
                        }

                        Console.WriteLine(buff, column.Name);
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
