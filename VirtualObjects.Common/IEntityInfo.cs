using System;
using System.Collections.Generic;

namespace VirtualObjects
{
    public interface IEntityInfo
    {

        IEntityColumnInfo this[String propertyName] { get; }

        String EntityName { get; }

        IList<IEntityColumnInfo> Columns { get; }

        IList<IEntityColumnInfo> KeyColumns { get; }
        
        IEntityColumnInfo Identity { get; }

        Type EntityType { get; }
        
        IEntityColumnInfo GetFieldAssociatedWith(string name);

        int GetKeyHashCode(Object obj);

        IOperations Operations { get; }
    }
}