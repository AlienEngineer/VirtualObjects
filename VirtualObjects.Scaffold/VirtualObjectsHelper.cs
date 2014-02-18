using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualObjects.Mappings;

namespace VirtualObjects.Scaffold
{
    public static class VirtualObjectsHelper
    {

        [Table(TableName = "INFORMATION_SCHEMA.TABLES")]
        public class MetaTable
        {
            [Key(FieldName = "TABLE_NAME")]
            public String Name { get; set; }

            [Column(FieldName = "TABLE_TYPE")]
            public String Type { get; set; }

            public IEnumerable<MetaColumn> Columns { get; set; }
        }

        [Table(TableName = "INFORMATION_SCHEMA.COLUMNS")]
        public class MetaColumn
        {
            [Key(FieldName = "TABLE_NAME")]
            [Association(FieldName = "TABLE_NAME", OtherKey = "Name")]
            public virtual MetaTable Table { get; set; }

            [Key(FieldName = "COLUMN_NAME")]
            public String Name { get; set; }

            public Boolean IsKey { get; set; }
            
            [Column(FieldName = "Is_Identity")]
            public Boolean IsIdentity { get; set; }
        }


        public static IEnumerable<MetaTable> GeTables(String connectionName)
        {
            
        }

    }
}
