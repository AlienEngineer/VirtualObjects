using System;

namespace VirtualObjects.EntityProvider
{
    class ProxyEntityProvider : EntityModelProvider
    {
        public ProxyEntityProvider(ISession session)
        {
            
        }

        public override object CreateEntity(Type type)
        {
            throw new NotImplementedException();
        }

        public override bool CanCreate(Type type)
        {
            throw new NotImplementedException();
        }

    }
}
