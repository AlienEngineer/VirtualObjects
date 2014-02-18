using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualObjects.Mappings;

namespace VirtualObjects.Scaffold
{
    public static class VirtualObjectsHelper
    {

        [Table(TableName = "sys.objects")]
        public class MetaTable
        {
            [Key(FieldName = "Object_Id")]
            public int Id { get; set; }

            public String Name { get; set; }

            public String Type { get; set; }

            public IEnumerable<MetaColumn> Columns { get; set; }
        }

        public class MetaColumn
        {
            [Association(FieldName = "Object_Id", OtherKey = "Id")]
            public virtual MetaTable Table { get; set; }

            [Key(FieldName = "Column_Id")]
            public int Id { get; set; }

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
