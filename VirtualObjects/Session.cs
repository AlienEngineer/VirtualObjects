using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using VirtualObjects.CodeGenerators;
using VirtualObjects.Config;
using VirtualObjects.Connections;
using VirtualObjects.Core;
using VirtualObjects.CRUD;
using VirtualObjects.EntityProvider;
using VirtualObjects.Exceptions;
using VirtualObjects.Programability;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Execution;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects
{

    class ModulesConfiguration : IModulesConfiguration
    {
        private SessionConfiguration _configuration;

        public ModulesConfiguration(SessionConfiguration configuration, IDbConnectionProvider connectionProvider)
        {
            InternalInitialize(configuration);

            _configuration.ConnectionProvider = connectionProvider;

            Initialize(configuration);
        }

        public ModulesConfiguration(SessionConfiguration configuration, string connectionProvider)
        {
            InternalInitialize(configuration);

            _configuration.ConnectionProvider = new NamedDbConnectionProvider(connectionProvider);

            Initialize(_configuration);
        }

        public ModulesConfiguration(SessionConfiguration configuration)
        {
            InternalInitialize(configuration);

            Initialize(configuration ?? new SessionConfiguration());
        }

        private void InternalInitialize(SessionConfiguration configuration)
        {
            _configuration = configuration ?? new SessionConfiguration();

            _configuration.Initialize();
            _configuration.ConfigureMappingBuilder(_configuration.TranslationConfigurationBuilder);
        }


        private void Initialize(SessionConfiguration configuration)
        {
            ConnectionProvider = configuration.ConnectionProvider ?? new NamedDbConnectionProvider();
            Logger = configuration.Logger ?? new TextWriterStub();
            Formmater = configuration.Formatter ?? new SqlFormatter(configuration.FunctionTranslation);

            ConnectionManager = new Connection(ConnectionProvider, Logger, new SqlProgramability());

            EntityBag = new HybridEntityBag(new EntityBag());

            EntityProvider = new EntityProviderComposite(
                new IEntityProvider[]
                {
                    new EntityModelProvider(),
                    new DynamicTypeProvider(), 
                    new CollectionTypeEntityProvider()
                });

            EntityMapper = new EntityInfoModelMapper();
            EntitiesMapper = new EntityModelEntitiesMapper();
            
            
            OperationsProvider = new OperationsProvider(Formmater, EntityMapper, EntityProvider);
            
            TranslationConfiguration = configuration.TranslationConfigurationBuilder.Build();

            EntityInfoCodeGeneratorFactory = new EntityInfoCodeGeneratorFactory(EntityBag, TranslationConfiguration, configuration);

            Mapper = new Mapper(EntityBag, TranslationConfiguration, OperationsProvider, EntityInfoCodeGeneratorFactory);


            SessionContext = new SessionContext
            {
                Connection = ConnectionManager,
                Map = Mapper.Map,
                Mapper = Mapper
            };


            Translator = new CachingTranslator(Formmater, Mapper, EntityBag, configuration);

            QueryExecutor = new CompositeExecutor(
                new IQueryExecutor[]
                {
                    new CountQueryExecutor(Translator),
                    new SingleQueryExecutor(EntitiesMapper, Translator), 
                    new QueryExecutor(EntitiesMapper, Translator)
                });

            QueryProvider = new QueryProvider(QueryExecutor, SessionContext, Translator);
            
            SessionContext.QueryProvider = QueryProvider;
            
            Session = new InternalSession(SessionContext);
        }

        public IQueryExecutor QueryExecutor { get; set; }
        public EntityInfoCodeGeneratorFactory EntityInfoCodeGeneratorFactory { get; set; }
        public ITranslationConfiguration TranslationConfiguration { get; set; }
        public IEntityProvider EntityProvider { get; set; }
        public EntityInfoModelMapper EntityMapper { get; set; }
        public IFormatter Formmater { get; set; }
        public OperationsProvider OperationsProvider { get; set; }
        public HybridEntityBag EntityBag { get; set; }
        public Mapper Mapper { get; set; }
        public TextWriter Logger { get; set; }
        public IDbConnectionProvider ConnectionProvider { get; set; }
        public ISession Session { get; private set; }
        public IConnection ConnectionManager { get; private set; }
        public IQueryTranslator Translator { get; private set; }
        public IQueryProvider QueryProvider { get; private set; }
        public SessionContext SessionContext { get; private set; }
        public IEntitiesMapper EntitiesMapper { get; private set; }
    }

    interface IModulesConfiguration
    {
        ISession Session { get; }
        IConnection ConnectionManager { get; }
        IQueryTranslator Translator { get; }
        IQueryProvider QueryProvider { get; }
        SessionContext SessionContext { get; }
        IEntitiesMapper EntitiesMapper { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Session : ISession
    {

        internal ISession InternalSession { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        public Session()
            : this(configuration: null, connectionName: null) { }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString => ((InternalSession) InternalSession).Context.Connection.ConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connectionProvider">The connection provider.</param>
        public Session(SessionConfiguration configuration = null, IDbConnectionProvider connectionProvider = null)
            : this(new ModulesConfiguration(configuration, connectionProvider)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connectionName">Name of the connection.</param>
        public Session(SessionConfiguration configuration = null, string connectionName = null)
            : this(new ModulesConfiguration(configuration, connectionName)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Session(SessionConfiguration configuration)
            : this(new ModulesConfiguration(configuration)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        internal Session(IModulesConfiguration container)
        {
            InternalSession = container.Session;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public IDbConnection Connection => InternalSession.Connection;

        /// <summary>
        /// Gets all entities of TEntity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return InternalSession.GetAll<TEntity>();
        }

        /// <summary>
        /// Gets the raw data.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IDataReader GetRawData(string query)
        {
            return InternalSession.GetRawData(query);
        }

        /// <summary>
        /// Executes the speficied command with the speficied parameters.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public IEnumerable<TEntity> Query<TEntity>(string command, IEnumerable<IQueryParameter> parameters)
        {
            return InternalSession.Query<TEntity>(command, parameters);
        }

        /// <summary>
        /// Gets the entity by its ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.GetById(entity);
        }

        /// <summary>
        /// Gets how many entities existe of the given TEntity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public int Count<TEntity>()
        {
            return InternalSession.Count<TEntity>(); 
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Insert(entity);
        }

        
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Update(entity);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Delete(entity);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <param name="isolation"></param>
        /// <returns></returns>
        public ITransaction BeginTransaction(IsolationLevel isolation = IsolationLevel.Unspecified)
        {
            return InternalSession.BeginTransaction(isolation);
        }

        /// <summary>
        /// Executes the store procedure.
        /// </summary>
        /// <param name="storeProcedure">The store procedure.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public int ExecuteStoreProcedure(string storeProcedure, IEnumerable<KeyValuePair<string, object>> args)
        {
            return InternalSession.ExecuteStoreProcedure(storeProcedure, args);
        }

        #region IDisposable Members
        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed )
            {
                if ( disposing )
                {
                    InternalSession.Dispose();
                }

                InternalSession = null;
                _disposed = true;
            }
        }

        #endregion

    }


    /// <summary>
    /// A session that will connect to an excel file.
    /// Default Provider: System.Data.OleDb
    /// ConnectionString : Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';
    /// </summary>
    public class ExcelSession : Session
    {
        /// <summary>
        /// 
        /// </summary>
        public enum Extension
        {
            /// <summary>
            /// The XLS
            /// </summary>
            Xls,
            /// <summary>
            /// The XLSX
            /// </summary>
            Xlsx,
        }

        static readonly IDictionary<string, string> Masks = new Dictionary<string, string>();

        static ExcelSession()
        {
            Masks.Add(Extension.Xls.ToString().ToLower(), "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties='Excel 8.0;HDR=YES;'");
            Masks.Add(Extension.Xlsx.ToString().ToLower(), "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 12.0;HDR=YES;'");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSession" /> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="configuration"></param>
        public ExcelSession(string filename, SessionConfiguration configuration = null)
            : base(PrepareConfiguration(configuration), new DbConnectionProvider("System.Data.OleDb", BuildConnectionString(filename.ToLower())))
        { }

        private static SessionConfiguration PrepareConfiguration(SessionConfiguration configuration)
        {
            if (configuration == null)
            {
                configuration = new SessionConfiguration();
            }

            configuration.Formatter = new ExcelFormatter(configuration.FunctionTranslation);

            return configuration;
        }

        private static string BuildConnectionString(string filename)
        {
            try
            {
                return string.Format(Masks[ParseExtension(filename)], filename);
            }
            catch ( Exception ex )
            {
                throw new VirtualObjectsException(Errors.Excel_UnableToCreateConnectionString, new { FileName = filename }, ex);
            }
        }

        private static string ParseExtension(string filename)
        {
            return filename.Substring(filename.LastIndexOf('.') + 1);
        }

    }
}