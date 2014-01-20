using System;
using System.Reflection;

namespace VirtualObjects.Config
{
    public interface IEntityColumnInfo
    {
        String ColumnName { get; }

        Boolean IsKey { get; }

        Boolean IsIdentity { get; }

        PropertyInfo Property { get; }
    }
}