using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualObjects.Config
{
    public class AttributeMappingBuilder : IMappingBuilder
    {
        readonly MappingBuilder _innerBuilder = new MappingBuilder();

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
