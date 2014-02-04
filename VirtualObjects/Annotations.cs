using System;

namespace VirtualObjects
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName">The name of this field.</param>
        /// <param name="otherKey">The property name, not the fieldName.</param>
        public AssociationAttribute(String fieldName, String otherKey)
            : base(fieldName)
        {
            OtherKey = otherKey;
        }
    }

    public class VersionAttribute : ColumnAttribute
    {
        public VersionAttribute(string fieldName = null)
            : base(fieldName)
        {
        }
    }


}