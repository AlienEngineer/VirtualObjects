using System;
namespace VirtualObjects.Tests.Config
{
    public static class Db
    {
        public class ColumnAttribute : Attribute
        {
            public ColumnAttribute(String fieldName)
            {
                FieldName = fieldName;
            }

            public String FieldName { get; private set; }
        }

        public class KeyAttribute : ColumnAttribute
        {
            public KeyAttribute(String fieldName = null) 
                : base(fieldName)
            {
            }
        }

        public class IdentityAttribute : KeyAttribute
        {
            public IdentityAttribute(String fieldName = null)
                : base(fieldName)
            {
            }
        }

        public class TableAttribute : Attribute
        {
            public TableAttribute(String tableName)
            {
                TableName = tableName;
            }

            public String TableName { get; private set; }
        }

        public class AssociationAttribute : ColumnAttribute
        {
            public string OtherKey { get; private set; }

            public AssociationAttribute(String fieldName, String otherKey)
                :base(fieldName)
            {
                OtherKey = otherKey;
            }
        }
    }
    
}