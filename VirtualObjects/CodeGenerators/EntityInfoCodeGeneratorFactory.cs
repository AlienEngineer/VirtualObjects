using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Config;

namespace VirtualObjects.CodeGenerators
{
    class EntityInfoCodeGeneratorFactory : IEntityInfoCodeGeneratorFactory
    {
        private readonly ITranslationConfiguration configuration;
        private readonly IEntityBag entityBag;

        public EntityInfoCodeGeneratorFactory(IEntityBag entityBag, ITranslationConfiguration configuration)
        {
            this.configuration = configuration;
            this.entityBag = entityBag;
        }

        public IEntityCodeGenerator Make(IEntityInfo info)
        {
            return new EntityInfoCodeGenerator(info, entityBag, configuration);
        }
    }
}
