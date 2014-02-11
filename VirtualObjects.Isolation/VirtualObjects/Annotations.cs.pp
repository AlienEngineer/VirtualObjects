using System;

namespace $rootnamespace$
{
    public class ColumnAttribute : Attribute
    {
        public String FieldName { get; set; }
    }

    public class KeyAttribute : ColumnAttribute { }

    public class IdentityAttribute : KeyAttribute { }

    public class TableAttribute : Attribute
    {
        public String TableName { get; set; }
    }

    public class AssociationAttribute : ColumnAttribute
    {
        public string OtherKey { get; set; }
    }

    public class VersionAttribute : ColumnAttribute { }
}
