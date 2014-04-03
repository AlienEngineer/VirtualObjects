using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtualObjects.CodeGenerators;
using VirtualObjects.Config;
using VirtualObjects.Connections;
using VirtualObjects.Core;
using VirtualObjects.CRUD;
using VirtualObjects.EntityProvider;
using VirtualObjects.LightInject;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Execution;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects
{
    class LightInjectContainer : IOcContainer
    {
        private readonly IServiceContainer _container = new ServiceContainer();
        private readonly ILifetime _lifetime = new PerScopeLifetime();

        public LightInjectContainer(SessionConfiguration configuration, IDbConnectionProvider connectionProvider)
        {
            configuration = configuration ?? new SessionConfiguration();
            configuration.ConnectionProvider = configuration.ConnectionProvider ?? connectionProvider;

            RegisterServices(configuration);
        }

        public LightInjectContainer(SessionConfiguration configuration, string connectionName)
            :this(configuration, new NamedDbConnectionProvider(connectionName))
        {
            
        }

        public LightInjectContainer(SessionConfiguration configuration)
        {
            configuration = configuration ?? new SessionConfiguration();

            RegisterServices(configuration);
        }

        private void RegisterServices(SessionConfiguration configuration)
        {
            //
            // Defaults or configurable
            //
            configuration.ConnectionProvider = configuration.ConnectionProvider ?? new NamedDbConnectionProvider();
            configuration.Logger = configuration.Logger ?? new TextWriterStub();
            configuration.Formatter = configuration.Formatter ?? new SqlFormatter();

            _container.RegisterInstance(typeof (IDbConnectionProvider), configuration.ConnectionProvider);
            _container.RegisterInstance(typeof (TextWriter), configuration.Logger);
            _container.RegisterInstance(typeof (IFormatter), configuration.Formatter);

            _container.RegisterInstance(typeof (IEntityBag), new HybridEntityBag(new EntityBag()));
            _container.RegisterInstance(typeof (ITranslationConfiguration),
                configuration.TranslationConfigurationBuilder.Build());

            _container.Register<IConnection, Connection>(_lifetime);
            _container.Register<ISession, InternalSession>(_lifetime);
            _container.Register<IMapper, Mapper>(_lifetime);
            _container.Register<IEntityInfoCodeGeneratorFactory, EntityInfoCodeGeneratorFactory>(_lifetime);
            _container.Register<IQueryTranslator, CachingTranslator>(_lifetime);
            _container.Register<IEntitiesMapper, EntityModelEntitiesMapper>(_lifetime);
            _container.Register<IEntityMapper, EntityInfoModelMapper>(_lifetime);
            _container.Register<IQueryProvider, QueryProvider>(_lifetime);
            _container.Register<IOperationsProvider, OperationsProvider>(_lifetime);

            _container.Register<IEntityProvider, EntityModelProvider>(_lifetime);
            _container.Register<IEntityProvider, DynamicTypeProvider>(_lifetime);
            _container.Register<IEntityProvider, CollectionTypeEntityProvider>(_lifetime);

            _container.Register<IEntityProvider>(factory =>
                new EntityProviderComposite(new List<IEntityProvider>(
                    factory.GetAllInstances<IEntityProvider>().Where(e => !(e is EntityProviderComposite))
                    )
                    ));

            _container.Register<IQueryExecutor, CountQueryExecutor>(_lifetime);
            _container.Register<IQueryExecutor, QueryExecutor>(_lifetime);
            _container.Register<IQueryExecutor, SingleQueryExecutor>(_lifetime);

            _container.Register<IQueryExecutor>(factory =>
                new CompositeExecutor(new List<IQueryExecutor>(
                    factory.GetAllInstances<IQueryExecutor>().Where(e => !(e is CompositeExecutor))
                    )
                    ));
        }

        public object Get(Type type)
        {
            return _container.GetInstance(type);
        }

        public TResult Get<TResult>()
        {
            return _container.GetInstance<TResult>();
        }
    }
}