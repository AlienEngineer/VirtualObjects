using System;

namespace VirtualObjects.EntityProvider
{
    public class EntityProvider : IEntityProvider
    {
        public object CreateEntity(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public bool CanCreate(Type type)
        {
            return !type.IsDynamic();
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            return CanCreate(type) ? this : null;
        }
    }
}
