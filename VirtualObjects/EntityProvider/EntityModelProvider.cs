using System;
using System.Collections;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{
    class EntityModelProvider : IEntityProvider
    {
        public virtual object CreateEntity(Type type)
        {
            return type.CreateInstance();
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

        public virtual void PrepareProvider(Type outputType)
        {
            // No prepare needed.
        }
    }
}
