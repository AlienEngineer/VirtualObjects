using System;
using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Config;

namespace VirtualObjects.CodeGenerators
{
    class EntityInfoCodeGeneratorFactory : IEntityInfoCodeGeneratorFactory
    {
        private readonly IEntityBag entityBag;

        public EntityInfoCodeGeneratorFactory(IEntityBag entityBag)
        {
            this.entityBag = entityBag;
        }

        public IEntityCodeGenerator Make(IEntityInfo info)
        {
            return new EntityInfoCodeGenerator(info, entityBag);
        }
    }
}
