using System;

namespace VirtualObjects.CRUD
{
    class OperationParameter : IOperationParameter
    {
        public Type Type { get; set; }
        public object Value { get; set; }
        public string Name { get; set; }
        public IEntityColumnInfo Column { get; set; }
    }
}