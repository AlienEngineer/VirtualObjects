using System;
using System.Collections;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{
    class EntityModelProvider : IEntityProvider
    {
        private Func<Type, Object> Make;              

        public virtual object CreateEntity(Type type)
        {
            // return type.CreateInstance();
            //
            // Activator is faster.
            // This can be verified in the Bin\Release\Performance.xlsx
            //
            //return Activator.CreateInstance(type);

            return Make(type);
        }

        public virtual bool CanCreate(Type type)
        {
            var canCreate = !type.IsDynamic() &&
                !type.InheritsOrImplements<IEnumerable>() &&
                !type.Properties().Any(e => e.GetGetMethod().IsVirtual);

            return canCreate;
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            return CanCreate(type) ? this : null;
        }

        public IEntityProvider MainProvider { get; set; }

        public virtual void PrepareProvider(Type outputType, SessionContext sessionContext)
        {
            var entityInfo = sessionContext.Mapper.Map(outputType);
            if ( entityInfo != null )
            {
                Make = ((type) => entityInfo.EntityFactory());
            }
            else
            {
                Make = ((type) => Activator.CreateInstance(type));
            }
        }
    }
}
