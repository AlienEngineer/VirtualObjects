using System;
using System.Collections.Generic;

namespace VirtualObjects.Config
{
    public interface IEntityInfo
    {

        IEntityColumnInfo this[String propertyName] { get; }

        String EntityName { get; }

        IEnumerable<IEntityColumnInfo> Columns { get; }

        IEnumerable<IEntityColumnInfo> KeyColumns { get; }
        
        Type EntityType { get; }
        
        IEntityColumnInfo GetFieldAssociatedWith(string name);
    }
}