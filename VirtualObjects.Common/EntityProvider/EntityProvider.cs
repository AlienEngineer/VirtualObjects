using System;
using System.Collections;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{
    class EntityProvider : IEntityProvider
    {
        public virtual object CreateEntity(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public virtual bool CanCreate(Type type)
        {
            return !type.IsDynamic() && !type.InheritsOrImplements<IEnumerable>();
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            return CanCreate(type) ? this : null;
        }

        public IEntityProvider MainProvider { get; set; }

        public void PrepareProvider(Type outputType)
        {
            // No prepare needed.
        }
    }
}
