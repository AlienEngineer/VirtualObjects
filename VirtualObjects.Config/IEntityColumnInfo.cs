using System;

namespace VirtualObjects.Config
{
    public interface IEntityColumnInfo
    {
        String ColumnName { get; }

        Boolean IsKey { get; }

        Boolean IsIdentity { get; }
    }
}