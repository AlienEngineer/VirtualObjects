using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Mappings;

namespace VirtualObjects.Scaffold
{

#pragma warning disable 1591
    /// <summary>
    /// 
    /// </summary>
    public static class VirtualObjectsHelper
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

        [Table(TableName = "sys.foreign_key_columns")]
        public class ForeingKey
        {
            [Key]
            [Association(FieldName = "parent_object_id", OtherKey = "Id")]
            public virtual Table Table { get; set; }

            [Key]
            [Association(FieldName = "parent_column_id", OtherKey = "Id", DependesOn = "Table")]
            public virtual Column Column { get; set; }

            [Association(FieldName = "Referenced_column_id", OtherKey = "Id", DependesOn = "ReferencedTable")]
            public virtual Column ReferencedColumn { get; set; }

            [Association(FieldName = "Referenced_object_id", OtherKey = "Id")]
            public virtual Table ReferencedTable { get; set; }
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

        [Serializable]
        public class MetaTable
        {
            public String Name { get; set; }
            public ICollection<MetaField> Columns { get; set; }
        }

        [Serializable]
        public class MetaField
        {
            public bool Identity { get; set; }
            public bool InPrimaryKey { get; set; }
            public bool IsForeignKey { get; set; }
            public String Name { get; set; }
            public int DataType { get; set; }

            public MetaTable Table { get; set; }
            public ICollection<MetaForeignKey> ForeignKeys { get; set; }
        }

        [Serializable]
        public class MetaForeignKey
        {
            public MetaField Column { get; set; }
            public MetaTable Table { get; set; }
            public MetaField ReferencedColumn { get; set; }
            public MetaTable ReferencedTable { get; set; }
        }


#pragma warning restore 1591

        /// <summary>
        /// Gets the tables.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public static IEnumerable<MetaTable> GetTables(string databaseName, string serverName, String tableName = null)
        {
            return GetTablesLazy(databaseName, serverName, tableName).ToList();         
        }

        private static IEnumerable<MetaTable> GetTablesLazy(string databaseName, string serverName, String tableName = null)
        {
            using ( var session = new Session(
                    new SessionConfiguration{ Logger = Console.Out },
                    new Connections.DbConnectionProvider("System.Data.SqlClient", String.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", serverName, databaseName))) )
            {

                foreach ( var table in session.Query<Table>()
                    .Where(e => e.Type == "U" && (e.Name == tableName || tableName == null)) )
                {
                    var metaTable = new MetaTable
                    {
                        Name = table.Name
                    };

                    metaTable.Columns = table.Columns.ToList().Select(column =>
                    {
                        var metaColumn = new MetaField
                        {
                            Name = column.Name,
                            Identity = column.IsIdentity,
                            InPrimaryKey = IsPrimaryKey(table, column, session),
                            IsForeignKey = IsForeignKey(table, column, session),
                            DataType = column.Type,
                            Table = metaTable,
                            ForeignKeys = GetForeignKeys(table, column, session)
                        };

                        return metaColumn;
                    }).ToList();

                    yield return metaTable;
                }

            }
        }

        private static bool IsPrimaryKey(Table table, Column column, Session session)
        {
            var indexInfo = session.Query<IndexColumn>().FirstOrDefault(e => e.Column == column && e.Table == table);

            return (indexInfo != null && indexInfo.Index != null && indexInfo.Index.IsPrimaryKey);
        }

        private static bool IsForeignKey(Table table, Column column, Session session)
        {
            return session.Query<ForeingKey>().Any(e => e.Column == column && e.Table == table);
        }

        private static ICollection<MetaForeignKey> GetForeignKeys(Table table, Column column, Session session)
        {
            return GetForeignKeysLazy(table, column, session).ToList();
        }

        private static IEnumerable<MetaForeignKey> GetForeignKeysLazy(Table table, Column column, Session session)
        {
            foreach ( var foreignKey in session.Query<ForeingKey>()
                .Where(e => e.Column == column && e.Table == table && e.ReferencedColumn != null) )
            {
                string foreignKeyReferencedColumnName = foreignKey.ReferencedColumn.Name;

                if ( foreignKey.ReferencedTable.Name != foreignKey.ReferencedColumn.Table.Name )
                {
                    throw new Exception(String.Format("{0} table doesn't exist in {1}", foreignKey.ReferencedColumn.Name, foreignKey.ReferencedTable.Name));
                }

                yield return new MetaForeignKey
                {
                    Column = new MetaField { Name = foreignKey.Column.Name },
                    Table = new MetaTable { Name = foreignKey.Table.Name },
                    ReferencedColumn = new MetaField { Name = foreignKeyReferencedColumnName },
                    ReferencedTable = new MetaTable { Name = foreignKey.ReferencedTable.Name }
                };
            }
        }
    }

}
