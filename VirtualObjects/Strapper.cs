using System;
using System.Linq;
using Ninject.Modules;
using VirtualObjects.Config;
using VirtualObjects.Core.Connection;
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
            Bind<IDbConnectionProvider>().To<NamedDbConnectionProvider>();

            Bind<IConnection>();

            //
            // Entity info Mapper
            //
            Bind<IMapper>().ToConstant(CreateBuilder().Build());
            
            //
            // QueryTranslation
            //
            Bind<IFormatter>().To<SqlFormatter>();
            Bind<IQueryTranslator>().To<CachingTranslator>();

            //
            // Entities Provider
            //
            Bind<IEntityProvider>().To<EntityProviderComposite>();
            Bind<IEntityProvider>().To<EntityModelProvider>().WhenInjectedInto<EntityProviderComposite>();
            Bind<IEntityProvider>().To<DynamicTypeProvider>().WhenInjectedInto<EntityProviderComposite>();
            Bind<IEntityProvider>().To<CollectionTypeEntityProvider>().WhenInjectedInto<EntityProviderComposite>();

            //
            // Entities Mappers
            //
            Bind<IEntitiesMapper>().To<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<OrderedEntityMapper>();
            Bind<IEntityMapper>().To<DynamicTypeEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<DynamicEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<DynamicWithMemberEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<GroupedDynamicEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();

            //
            // Query Executors
            //
            Bind<IQueryExecutor>().To<CompositeExecutor>();
            Bind<IQueryExecutor>().To<CountQueryExecutor>().WhenInjectedInto<CompositeExecutor>();
            Bind<IQueryExecutor>().To<QueryExecutor>().WhenInjectedInto<CompositeExecutor>();
            Bind<IQueryExecutor>().To<SingleQueryExecutor>().WhenInjectedInto<CompositeExecutor>();

            Bind<Context>().ToSelf();

            //
            // Query Provider
            //
            Bind<IQueryProvider>().To<QueryProvider>();

        }

        private MappingBuilder CreateBuilder()
        {
            var builder = new MappingBuilder();

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
