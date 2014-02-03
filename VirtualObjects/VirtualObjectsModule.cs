using System;
using System.Linq;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using VirtualObjects.Config;
using VirtualObjects.Core.Connection;
using VirtualObjects.Core.CRUD;
using VirtualObjects.EntityProvider;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Execution;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects
{
    public class VirtualObjectsModule : NinjectModule
    {
        public override void Load()
        {
            //
            // Connection
            //
            Bind<IDbConnectionProvider>().To<NamedDbConnectionProvider>().InSingletonScope();

            Bind<IConnection>().To<Connection>().InThreadScope();

            //
            // Entity info Mapper
            //
            Bind<IMapper>().ToMethod(context => CreateBuilder(context.Kernel.Get<IOperationsProvider>()).Build()).InThreadScope();
            
            //
            // QueryTranslation
            //
            Bind<IFormatter>().To<SqlFormatter>().InSingletonScope();
            Bind<IQueryTranslator>().To<CachingTranslator>().InThreadScope();

            //
            // Entities Provider
            //
            Bind<IEntityProvider>().To<EntityProviderComposite>().InSingletonScope();
            Bind<IEntityProvider>().To<EntityModelProvider>().WhenInjectedInto<EntityProviderComposite>();
            Bind<IEntityProvider>().To<DynamicTypeProvider>().WhenInjectedInto<EntityProviderComposite>();
            Bind<IEntityProvider>().To<CollectionTypeEntityProvider>().WhenInjectedInto<EntityProviderComposite>();

            //
            // Entities Mappers
            //
            Bind<IEntitiesMapper>().To<CollectionEntitiesMapper>().InSingletonScope();
            Bind<IEntityMapper>().To<OrderedEntityMapper>().InSingletonScope();
            Bind<IEntityMapper>().To<DynamicTypeEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<DynamicEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<DynamicWithMemberEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<GroupedDynamicEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();

            //
            // Query Executors
            //
            Bind<IQueryExecutor>().To<CompositeExecutor>().InThreadScope();
            Bind<IQueryExecutor>().To<CountQueryExecutor>().WhenInjectedInto<CompositeExecutor>();
            Bind<IQueryExecutor>().To<QueryExecutor>().WhenInjectedInto<CompositeExecutor>();
            Bind<IQueryExecutor>().To<SingleQueryExecutor>().WhenInjectedInto<CompositeExecutor>();

            Bind<Context>().ToMethod(context => 
                new Context
                {
                   Connection = context.Kernel.Get<IConnection>()    
                });

            //
            // Query Provider
            //
            Bind<IQueryProvider>().To<QueryProvider>();


            //
            // CRUD Operations
            //
            Bind<IOperationsProvider>().To<OperationsProvider>();

        }

        private MappingBuilder CreateBuilder(IOperationsProvider operationsProvider)
        {
            var builder = new MappingBuilder(operationsProvider);

            //
            // TableName getters
            //
            builder.EntityNameFromType(e => e.Name);
            builder.EntityNameFromAttribute<TableAttribute>(e => e.TableName);

            //
            // ColumnName getters
            //
            builder.ColumnNameFromProperty(e => e.Name);
            builder.ColumnNameFromAttribute<ColumnAttribute>(e => e.FieldName);

            builder.ColumnKeyFromAttribute<KeyAttribute>();
            builder.ColumnKeyFromAttribute<IdentityAttribute>();

            builder.ColumnIdentityFromAttribute<IdentityAttribute>();

            builder.ForeignKeyFromAttribute<AssociationAttribute>(e => e.OtherKey);

            builder.ColumnVersionFromProperty(e => e.Name == "Version");
            builder.ColumnVersionFromAttribute<VersionAttribute>();

            return builder;
        }
    }
}
