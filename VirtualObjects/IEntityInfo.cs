using System;
using System.Collections.Generic;
using VirtualObjects.Queries;

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
        IList<IEntityColumnInfo> ForeignKeys { get; }
        IEntityProvider EntityProvider { get; }
        IEntityMapper EntityMapper { get; }
    }
}