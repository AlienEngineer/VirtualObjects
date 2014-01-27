using System;

namespace VirtualObjects.EntityProvider
{
    public class EntityProvider : IEntityProvider
    {
        public object CreateEntity(Type type)
        {
            return Activator.CreateInstance(type);
        }
    }
}
