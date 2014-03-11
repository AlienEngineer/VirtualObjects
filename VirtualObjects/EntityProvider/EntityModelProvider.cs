using System;
using System.Collections;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{
    class EntityModelProvider : IEntityProvider
    {
        public virtual object CreateEntity(Type type)
        {
            // return type.CreateInstance();
            //
            // Activator is faster.
            // This can be verified in the Bin\Release\Performance.xlsx
            //
            return Activator.CreateInstance(type);
        }

        public virtual bool CanCreate(Type type)
        {
            return !type.IsDynamic() &&
                !type.InheritsOrImplements<IEnumerable>() &&
                !type.Properties().Any(e => e.GetGetMethod().IsVirtual);
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            return CanCreate(type) ? this : null;
        }

        public IEntityProvider MainProvider { get; set; }

        public virtual void PrepareProvider(Type outputType, SessionContext sessionContext)
        {
            // No prepare needed.
        }
    }
}
