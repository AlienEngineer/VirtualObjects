using System;

namespace VirtualObjects.EntityProvider
{
    class DynamicEntityProvider : IEntityProvider
    {
        public IEntityProvider MainProvider { get; set; }

        public object CreateEntity(Type type)
        {
            throw new NotImplementedException();
        }

        public bool CanCreate(Type type)
        {
            throw new NotImplementedException();
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            throw new NotImplementedException();
        }

        
        public void PrepareProvider(Type outputType)
        {
            throw new NotImplementedException();
        }
    }
}