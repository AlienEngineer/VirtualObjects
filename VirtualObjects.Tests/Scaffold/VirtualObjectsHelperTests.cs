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

    [TestFixture, Category("VirtualObjectsHelper")]
    public class VirtualObjectsHelperTests : UtilityBelt
    {

        [Test]
        public void Helper_Can_Produce_TablesInformation()
        {

            var connection = Connection as DbConnection;

            connection.Open();

            var table = connection.GetSchema(SqlClientMetaDataCollectionNames.Tables, new String[] { null, null, null, "BASE TABLE" });


            // Display the contents of the table.
            foreach ( System.Data.DataRow row in table.Rows )
            {
                Console.WriteLine("{0}", row["Table_Name"]);

                var cmd = connection.CreateCommand();
                cmd.CommandText = String.Format("Select * From [{0}]", row["Table_Name"]);
                var reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly);

                var columnsInfo = reader.GetSchemaTable();
                
                foreach ( DataRow columnInfo in columnsInfo.Rows )
                {
                    Console.WriteLine(" Name: {0}", columnInfo["ColumnName"]);
                    Console.WriteLine(" IsIdentity: {0}", columnInfo["IsIdentity"]);
                    Console.WriteLine(" IsKey: {0}", columnInfo["IsKey"]);
                    Console.WriteLine(" DataTypeName: {0}", columnInfo["DataTypeName"]);
                }
                
                reader.Close();

                foreach ( DataRelation relation in columnsInfo.ChildRelations )
                {
                    var fk = relation.ChildKeyConstraint;

                    Console.WriteLine(" RelatedTable {0}; RelatedColums {1}", fk.RelatedTable, fk.RelatedColumns);
                }

                Console.WriteLine("============================");
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
