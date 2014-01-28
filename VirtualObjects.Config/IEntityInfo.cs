using System;
using System.Collections.Generic;

namespace VirtualObjects.Config
{
    public interface IEntityInfo
    {

        IEntityColumnInfo this[String propertyName] { get; }

        String EntityName { get; }

        IList<IEntityColumnInfo> Columns { get; }

        IList<IEntityColumnInfo> KeyColumns { get; }
        
        Type EntityType { get; }
        
        IEntityColumnInfo GetFieldAssociatedWith(string name);
        int GetKeyHashCode(Object obj);
    }
}