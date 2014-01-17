using System;
using System.Collections.Generic;

namespace VirtualObjects.Config
{
    public interface IEntityInfo
    {

        IEntityColumnInfo this[String columnName] { get; }

        String EntityName { get; }

        IEnumerable<IEntityColumnInfo> Columns { get; }

        IEnumerable<IEntityColumnInfo> KeyColumns { get; }
    }
}