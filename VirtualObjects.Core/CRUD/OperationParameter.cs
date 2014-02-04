using System;

namespace VirtualObjects.Core.CRUD
{
    class OperationParameter : IOperationParameter
    {
        public Type Type { get; set; }
        public Object Value { get; set; }
        public String Name { get; set; }
        public IEntityColumnInfo Column { get; set; }
    }
}