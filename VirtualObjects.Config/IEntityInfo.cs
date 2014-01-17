using System;
using System.Collections.Generic;

namespace VirtualObjects.Config
{
    public interface IEntityInfo
    {
        String EntityName { get; }

        IEnumerable<IEntityColumnInfo> Columns { get; }
    }
}