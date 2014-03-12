using System.IO;
using System.Linq;
using Castle.DynamicProxy;
using Fasterflect;
using Ninject.Modules;
using Ninject.Syntax;
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

    /// <summary>
    /// 
    /// This is the configuration of a NinjectModule.
    /// 
    /// </summary>
    public class VirtualObjectsModule : NinjectModule
    {
        private readonly SessionConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualObjectsModule"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public VirtualObjectsModule(SessionConfiguration configuration)
        {
            _configuration = configuration ?? new SessionConfiguration();
        }

        /// <summary>
        /// Loads the module into the kernel.
        /// </summary>
        public override void Load()
        {
            _configuration.Init(new NinjectContainer(Kernel));

            //
            // Connection
            //
            if ( _configuration.ConnectionProvider == null )
            {
                Bind<IDbConnectionProvider>().To<NamedDbConnectionProvider>().InSingletonScope();
            }
            else
            {
                Bind<IDbConnectionProvider>().ToConstant(_configuration.ConnectionProvider).InSingletonScope();
            }

            Bind<IConnection>().To<Connection>().InThreadScope();

            if ( _configuration.Logger == null )
            {
                Bind<TextWriter>().To<TextWriterStub>().InSingletonScope();
            }
            else
            {
                Bind<TextWriter>().ToConstant(_configuration.Logger).InSingletonScope();
            }

            Bind<ISession>().To<InternalSession>().InThreadScope();
            Bind<SessionContext>().ToSelf().InThreadScope();

            //
            // Entity info Mapper
            //
            Bind<IMapper>().ToMethod(context => _configuration.MappingBuilder.Build()).InThreadScope();

            Bind<IMappingBuilder>().To<MappingBuilder>().InThreadScope();

            //
            // QueryTranslation
            //
            if (_configuration.Formatter == null)
            {
                Bind<IFormatter>().To<SqlFormatter>().InSingletonScope();
            }
            else
            {
                Bind<IFormatter>().ToConstant(_configuration.Formatter).InSingletonScope();
            }
            Bind<IQueryTranslator>().To<CachingTranslator>().InThreadScope();

            //
            // Entities Provider
            //
            Bind<IEntityProvider>().To<EntityProviderComposite>().ExcludeSelf().InThreadScope();
            Bind<IEntityProvider>().To<EntityModelProvider>().WhenInjectedInto<EntityProviderComposite>();
            Bind<IEntityProvider>().To<ProxyEntityProvider>().WhenInjectedInto<EntityProviderComposite>();
            Bind<IEntityProvider>().To<DynamicTypeProvider>().WhenInjectedInto<EntityProviderComposite>();
            Bind<IEntityProvider>().To<CollectionTypeEntityProvider>().WhenInjectedInto<EntityProviderComposite>();

            Bind<ProxyGenerator>().ToSelf().InThreadScope();
            

            //
            // Entities Mappers
            //
            Bind<IEntitiesMapper>().To<CollectionEntitiesMapper>().InSingletonScope();
            Bind<IEntityMapper>().To<OrderedEntityMapper>().InSingletonScope();
            Bind<IEntityMapper>().To<DynamicTypeEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<DynamicEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<DynamicWithMemberEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            Bind<IEntityMapper>().To<GroupedDynamicEntityMapper>().WhenInjectedInto<CollectionEntitiesMapper>();
            //Bind<IEntitiesMapper>().To<ParallelEntitiesMapper>().InSingletonScope();
            //Bind<IEntityMapper>().To<OrderedEntityMapper>().InSingletonScope();
            //Bind<IEntityMapper>().To<DynamicTypeEntityMapper>().WhenInjectedInto<ParallelEntitiesMapper>();
            //Bind<IEntityMapper>().To<DynamicEntityMapper>().WhenInjectedInto<ParallelEntitiesMapper>();
            //Bind<IEntityMapper>().To<DynamicWithMemberEntityMapper>().WhenInjectedInto<ParallelEntitiesMapper>();
            //Bind<IEntityMapper>().To<GroupedDynamicEntityMapper>().WhenInjectedInto<ParallelEntitiesMapper>();


            //
            // Query Executors
            //
            Bind<IQueryExecutor>().To<CompositeExecutor>().ExcludeSelf().InThreadScope();
            Bind<IQueryExecutor>().To<CountQueryExecutor>().WhenInjectedInto<CompositeExecutor>();
            Bind<IQueryExecutor>().To<QueryExecutor>().WhenInjectedInto<CompositeExecutor>();
            Bind<IQueryExecutor>().To<SingleQueryExecutor>().WhenInjectedInto<CompositeExecutor>();


            //
            // Query Provider
            //
            Bind<IQueryProvider>().To<QueryProvider>();

            //
            // CRUD Operations
            //
            Bind<IOperationsProvider>().To<OperationsProvider>();

            
        }

    }

    internal static class NinjectExtensions
    {
        public static IBindingInNamedWithOrOnSyntax<TImplementation> WhenInjectedInto<TImplementation, TInclude1, TInclude2>(this IBindingWhenInNamedWithOrOnSyntax<TImplementation> bind)
        {
            return bind.When(e => e.Target != null &&
                                  (e.Target.Type.InheritsOrImplements<TInclude1>() ||
                                   e.Target.Type == typeof(TInclude1) ||
                                   e.Target.Type.InheritsOrImplements<TInclude2>() ||
                                   e.Target.Type == typeof(TInclude2)));
        }

        public static IBindingInNamedWithOrOnSyntax<T> ExcludeSelf<T>(this IBindingWhenInNamedWithOrOnSyntax<T> bind)
        {
            return bind.When(e => e.Target == null ||
                                  e.Target.Member.ReflectedType != typeof(T));
        }

        ///// <summary>
        ///// Valid for composite type class. Annotated with [Composite].
        ///// </summary>
        ///// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        ///// <param name="bind">The bind.</param>
        ///// <returns></returns>
        //public static IBindingInNamedWithOrOnSyntax<TImplementation> WhenInjectedIntoComposite<TImplementation>(this IBindingWhenInNamedWithOrOnSyntax<TImplementation> bind)
        //{
        //    return bind.WhenClassHas<CompositeAttribute>();
        //}

    }
}
