using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.EntityProvider
{
    class EntityProviderComposite : IEntityProvider
    {
        readonly IEnumerable<IEntityProvider> _entityProviders;

        public IEntityProvider MainProvider { get; set; }
        private IEntityProvider _tmpProvider ;
        private Type _type;
        
        public void PrepareProvider(Type outputType)
        {
            _tmpProvider = GetProviderForType(outputType);
            
            if ( _tmpProvider == null )
            {
                throw new MappingException(Errors.Mapping_EntityTypeNotSupported, outputType);
            }

            _tmpProvider.PrepareProvider(outputType);
            _type = outputType;
        }

        public EntityProviderComposite(IEnumerable<IEntityProvider> entityProviders)
        {
            this._entityProviders = entityProviders
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
            return type == _type ? 
                _tmpProvider : 
                _entityProviders.FirstOrDefault(e => e.CanCreate(type));
        }

        public object CreateEntity(Type type)
        {
            var provider = GetProviderForType(type);

            if (provider == null)
            {
                throw new MappingException(Errors.Mapping_EntityTypeNotSupported, type);
            }

            return provider.CreateEntity(type);
        }
    }
}