using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualObjects.Config
{
    public class AttributeMappingBuilder : IMappingBuilder
    {
        private readonly MappingBuilder _innerBuilder;

        public AttributeMappingBuilder(IOperationsProvider operationsProvider)
        {
            _innerBuilder = new MappingBuilder(operationsProvider);
        }

        public IMapper Build()
        {
            _innerBuilder.ColumnNameFromProperty(e => e.Name);
            _innerBuilder.EntityNameFromType(e => e.Name);

            Configure(_innerBuilder);

            return _innerBuilder.Build();
        }

        protected virtual void Configure(MappingBuilder builder)
        {
            
        }
    }

}
