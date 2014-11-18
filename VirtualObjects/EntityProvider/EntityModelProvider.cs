using System;

namespace VirtualObjects.EntityProvider
{
    class EntityModelProvider : IEntityProvider
    {
        private Func<ISession, Type, Object> Make;
        private Type outputType;
        private SessionContext sessionContext;
        
        public virtual object CreateEntity(Type type)
        {
            // return type.CreateInstance();
            //
            // Activator is faster.
            // This can be verified in the Bin\Release\Performance.xlsx
            //
            //return Activator.CreateInstance(type);

            return Make(sessionContext.Session, type);
        }

        public virtual bool CanCreate(Type type)
        {
            var canCreate = !type.IsDynamic() && !type.IsCollection();

            return canCreate;
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            return CanCreate(type) ? this : null;
        }

        public IEntityProvider MainProvider { get; set; }

        public virtual void PrepareProvider(Type outputType, SessionContext sessionContext)
        {
            if ( this.outputType == outputType )
            {
                return;
            }

            this.outputType = outputType;
            this.sessionContext = sessionContext;
            var entityInfo = sessionContext.Map(outputType);

            if ( entityInfo != null )
            {
                Make = ((session, type) => entityInfo.EntityProxyFactory(session));
            }
            else
            {
                Make = ((session, type) => Activator.CreateInstance(type));
            }
        }
    }
}
