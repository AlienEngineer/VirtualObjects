using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Exceptions;

namespace VirtualObjects.EntityProvider
{
    class EntityProviderComposite : IEntityProvider
    {
        readonly IEnumerable<IEntityProvider> _entityProviders;

        public IEntityProvider MainProvider { get; set; }
        private IEntityProvider _tmpProvider ;
        private Type _type;
        
        public void PrepareProvider(Type outputType, SessionContext sessionContext)
        {
            _tmpProvider = GetProviderForType(outputType);
            
            _tmpProvider.PrepareProvider(outputType, sessionContext);
            _type = outputType;
        }

        public EntityProviderComposite(IEnumerable<IEntityProvider> entityProviders)
        {
            _entityProviders = entityProviders
                .ForEach(e => e.MainProvider = this)
                .ToList();
            
            MainProvider = this;
        }

        public Boolean CanCreate(Type type)
        {
            return _entityProviders.Any(e => e.CanCreate(type));
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            var provider = type == _type ? 
                _tmpProvider : 
                _entityProviders.FirstOrDefault(e => e.CanCreate(type));


            if ( provider == null )
            {
                throw new MappingException(Errors.Mapping_EntityTypeNotSupported, type);
            }

            return provider;
        }

        public object CreateEntity(Type type)
        {
            return GetProviderForType(type).CreateEntity(type);
        }
    }
}