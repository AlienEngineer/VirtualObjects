using System;

namespace VirtualObjects
{
    public interface IOperationParameter
    {
        Type Type { get; }
        Object Value { get; }
        String Name { get; }
        IEntityColumnInfo Column { get; }
    }
}
