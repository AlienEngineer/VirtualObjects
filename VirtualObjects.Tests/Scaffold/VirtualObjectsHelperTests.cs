using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Tests.Scaffold
{
    using NUnit.Framework;
    using System.Data;
    using VirtualObjects.Scaffold;

    [TestFixture, Category("VirtualObjectsHelper")]
    public class VirtualObjectsHelperTests : UtilityBelt
    {


        //[Test]
        public void Helper_Can_Produce_TablesInformation()
        {

            foreach ( var table in VirtualObjectsHelper.GetTables("AimirimTeste", ".\\Development") )
            {

                Console.WriteLine("TableName: {0}", table.Name);

                foreach ( var column in table.Columns.Where(e => e.IsForeignKey) )
                {
                    var buff = new StringBuffer();

                    buff += "     ColumnName: {0, -20}";
                    buff += "     DataType:   {1, -20}";

                    if ( column.Identity )
                    {
                        buff += " IsIdentity ";
                    }
                    else if ( column.InPrimaryKey )
                    {
                        buff += " IsKey ";
                    }

                    foreach ( var foreignKey in column.ForeignKeys )
                    {
                        buff += " ";
                        buff += foreignKey.ReferencedTable.Name;
                        buff += ".";
                        buff += foreignKey.ReferencedColumn.Name;
                        buff += " = ";
                        buff += foreignKey.Table.Name;
                        buff += ".";
                        buff += foreignKey.Column.Name;
                        buff += ";";
                    }

                    Console.WriteLine(buff, column.Name, column.DataType);
                }
            }

        }
                                               
    }
}
