using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace VirtualObjects.Tests.Scaffold
{
    using NUnit.Framework;
    using Microsoft.SqlServer.Management.Smo;
    using System.Data;

    [TestFixture, Category("VirtualObjectsHelper")]
    public class VirtualObjectsHelperTests : UtilityBelt
    {

        [Test]
        public void Helper_Can_Produce_TablesInformation()
        {
            
            Server server = new Server(@".\development");
            Database database = new Database(server, "northwind");
            database.Refresh();

            foreach ( Table table in database.Tables )
            {
                foreach ( Column column in table.Columns )
                {
                    column.Name.Should().NotBeBlank();
                    if ( column.IsForeignKey )
                    {
                        foreach ( ForeignKey foreignKeys in table.ForeignKeys )
                        {
                            foreach ( ForeignKeyColumn foreignColumn in foreignKeys.Columns )
                            {
                                if ( foreignColumn.Parent.Parent.Name == table.Name && foreignColumn.Name == column.Name )
                                {
                                    Console.Write(String.Format("[Association(FieldName=\"{0}\", OtherKey = \"{1}\")]", column.Name, foreignColumn.ReferencedColumn));
                                    Console.WriteLine("");

                                    Console.Write("public ");
                                    Console.Write(" virtual ");
                                    Console.Write(foreignColumn.Parent.ReferencedTable.Replace(" ", ""));
                                    Console.Write(" ");
                                    Console.Write(foreignColumn.Parent.ReferencedTable.Replace(" ", ""));
                                }
                            }
                        }

                    }
                }

            }

        }

    }
}
