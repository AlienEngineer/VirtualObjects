using VirtualObjects.Config;

namespace VirtualObjects.CodeGenerators
{
    class EntityInfoCodeGeneratorFactory : IEntityInfoCodeGeneratorFactory
    {
        private readonly ITranslationConfiguration configuration;
        private readonly SessionConfiguration _sessionConfiguration;
        private readonly IEntityBag entityBag;

        public EntityInfoCodeGeneratorFactory(IEntityBag entityBag, ITranslationConfiguration configuration, SessionConfiguration sessionConfiguration)
        {
            this.configuration = configuration;
            _sessionConfiguration = sessionConfiguration;
            this.entityBag = entityBag;
        }

        public IEntityCodeGenerator Make(IEntityInfo info)
        {
            return new EntityInfoCodeGenerator(info, entityBag, configuration, _sessionConfiguration);
        }
    }
}
