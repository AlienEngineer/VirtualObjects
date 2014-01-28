using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.EntityProvider
{
    class EntityProviderComposite : IEntityProvider
    {
        readonly IEnumerable<IEntityProvider> entityProviders;

        public IEntityProvider MainProvider { get; set; }

        public EntityProviderComposite(IEnumerable<IEntityProvider> entityProviders)
        {
            this.entityProviders = entityProviders
                .ForEach(e => e.MainProvider = this)
                .ToList();
            
            MainProvider = this;
        }

        public Boolean CanCreate(Type type)
        {
            return entityProviders.Any(e => e.CanCreate(type));
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            return entityProviders.FirstOrDefault(e => e.CanCreate(type));
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