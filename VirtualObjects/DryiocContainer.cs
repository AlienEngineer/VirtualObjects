using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DryIoc;
using VirtualObjects.CodeGenerators;
using VirtualObjects.Config;
using VirtualObjects.Connections;
using VirtualObjects.Core;
using VirtualObjects.CRUD;
using VirtualObjects.EntityProvider;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Execution;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects
{
    class DryiocContainer : IOcContainer
    {
        private const string _output = "OUT";
        private readonly IRegistry _container = new Container();

        public DryiocContainer(SessionConfiguration configuration, IDbConnectionProvider connectionProvider)
        {
            configuration = configuration ?? new SessionConfiguration();
            configuration.ConnectionProvider = configuration.ConnectionProvider ?? connectionProvider;

            RegisterServices(configuration);
        }

        public DryiocContainer(SessionConfiguration configuration, string connectionName)
            : this(configuration, new NamedDbConnectionProvider(connectionName))
        {

        }

        public DryiocContainer(SessionConfiguration configuration)
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

            _container.RegisterDelegate(r => configuration.ConnectionProvider, Reuse.Singleton, named: _output);
            _container.RegisterDelegate(r => configuration.Logger, Reuse.Singleton, named: _output);
            _container.RegisterDelegate(r => configuration.Formatter, Reuse.Singleton, named: _output);
            _container.RegisterDelegate(r => configuration.TranslationConfigurationBuilder.Build(), Reuse.Singleton, named: _output);

            _container.RegisterDelegate<IEntityBag>(r => new HybridEntityBag(new EntityBag()), Reuse.Singleton, named: _output);

            _container.Register<IConnection, Connection>(Reuse.Singleton, named: _output);
            _container.RegisterInstance(new SessionContext(), named: _output);
            _container.Register<ISession, InternalSession>(Reuse.Singleton, named: _output);
            _container.Register<IMapper, Mapper>(Reuse.Singleton, named: _output);
            _container.Register<IEntityInfoCodeGeneratorFactory, EntityInfoCodeGeneratorFactory>(Reuse.Singleton, named: _output);
            _container.Register<IQueryTranslator, CachingTranslator>(Reuse.Singleton, named: _output);
            _container.Register<IEntitiesMapper, EntityModelEntitiesMapper>(Reuse.Singleton, named: _output);
            _container.Register<IEntityMapper, EntityInfoModelMapper>(Reuse.Singleton, named: _output);
            _container.Register<IQueryProvider, QueryProvider>(Reuse.Singleton, named: _output);
            _container.Register<IOperationsProvider, OperationsProvider>(Reuse.Singleton, named: _output);

            _container.Register<IEntityProvider, EntityModelProvider>(Reuse.Singleton);
            _container.Register<IEntityProvider, DynamicTypeProvider>(Reuse.Singleton);
            _container.Register<IEntityProvider, CollectionTypeEntityProvider>(Reuse.Singleton);

            _container.Register<IEntityProvider, EntityProviderComposite>(Reuse.Singleton, named: _output);

            _container.Register<IQueryExecutor, CountQueryExecutor>(Reuse.Singleton);
            _container.Register<IQueryExecutor, QueryExecutor>(Reuse.Singleton);
            _container.Register<IQueryExecutor, SingleQueryExecutor>(Reuse.Singleton);

            _container.Register<IQueryExecutor, CompositeExecutor>(Reuse.Singleton, named: _output);
        }

        public object Get(Type type)
        {
            return _container.Resolve(type, _output);
        }

        public TResult Get<TResult>()
        {
            return _container.Resolve<TResult>(_output);
        }
    }
}