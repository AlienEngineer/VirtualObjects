using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using VirtualObjects.Config;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Execution;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Queries.Translation;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests
{
    public abstract class UtilityBelt : IConnection
    {
        public const int REPEAT = 10;

        protected UtilityBelt()
        {
            InitBelt();
        }

        private void InitBelt()
        {

            Connection = new SqlConnection(@"
                      Data Source=(LocalDB)\v11.0;
                      AttachDbFilename=" + Environment.CurrentDirectory + @"\Data\northwnd.mdf;
                      Integrated Security=True;
                      Connect Timeout=30");

            Mapper = CreateBuilder().Build();

            Translator = new CachingTranslator(new SqlFormatter(), Mapper);

            ConnectionManager = this;

            QueryProvider = MakeQueryProvider();
        }

        private IQueryProvider MakeQueryProvider()
        {
            var entityProvider = new EntityProvider.EntityProviderComposite(
                new List<IEntityProvider>
                {
                    new EntityProvider.EntityProvider(),
                    new EntityProvider.DynamicTypeProvider(),
                    new EntityProvider.CollectionTypeEntityProvider()
                });

            var entitiesMapper = new CollectionEntitiesMapper(Mapper,
                entityProvider,
                new List<IEntityMapper>
                {
                    new OrderedEntityMapper(),
                    new DynamicTypeEntityMapper(),
                    new DynamicEntityMapper(),
                    new DynamicWithMemberEntityMapper(),
                    new GroupedDynamicEntityMapper()
                });

            return new QueryProvider(
                new CompositeExecutor(
                    new List<IQueryExecutor>
                    {
                        new CountQueryExecutor(Translator),
                        new QueryExecutor(entitiesMapper, Translator),
                        new SingleQueryExecutor(entitiesMapper, Translator)
                    }), new Context { Connection = ConnectionManager });
        }



        IDbTransaction dbTransaction;

        [SetUp]
        public void SetUpConnection()
        {
            Connection.Open();
        }

        [TearDown]
        public void CleanUpConnection()
        {
            if (dbTransaction != null)
            {
                dbTransaction.Rollback();
            }
            Connection.Close();
        }

        /// <summary>
        /// This helper will begin a transaction and will rollback any changes at every unit-test TearDown
        /// </summary>
        public void RollBackOnTearDown()
        {
            if (dbTransaction == null)
            {
                dbTransaction = Connection.BeginTransaction();    
            }
        }

        public IDataReader ExecuteReader(IQueryInfo query)
        {
            return CreateCommand(query).ExecuteReader();
        }

        public Object ExecuteScalar(IQueryInfo query)
        {
            return CreateCommand(query).ExecuteScalar();
        }

        public IDbCommand CreateCommand(IQueryInfo queryInfo)
        {
            return CreateCommand(queryInfo.CommandText, queryInfo.Parameters);
        }

        private IDbCommand CreateCommand(String commandText, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            var cmd = Connection.CreateCommand();
            cmd.CommandText = commandText;

            Trace.WriteLine("Query: " + cmd.CommandText);

            parameters
                .Select(e => new { e, Parameter = cmd.CreateParameter() })
                .ForEach(e =>
                {
                    e.Parameter.ParameterName = e.e.Key;
                    e.Parameter.Value = e.e.Value;
                    cmd.Parameters.Add(e.Parameter);
                });

            return cmd;
        }

        public IQueryInfo TranslateQuery(IQueryable query)
        {
            return Translator.TranslateQuery(query);
        }

        public IQueryable<T> Query<T>()
        {
            return QueryProvider.CreateQuery<T>(null);
        }

        private MappingBuilder CreateBuilder()
        {
            var builder = new MappingBuilder();

            //
            // TableName getters
            //
            builder.EntityNameFromType(e => e.Name);
            builder.EntityNameFromAttribute<Db.TableAttribute>(e => e.TableName);

            //
            // ColumnName getters
            //
            builder.ColumnNameFromProperty(e => e.Name);
            builder.ColumnNameFromAttribute<Db.ColumnAttribute>(e => e.FieldName);

            builder.ColumnKeyFromProperty(e => e.Name == "Id");
            builder.ColumnKeyFromAttribute<Db.KeyAttribute>(e => e != null);
            builder.ColumnKeyFromAttribute<Db.IdentityAttribute>(e => e != null);

            builder.ColumnIdentityFromAttribute<Db.IdentityAttribute>(e => e != null);

            builder.ForeignKeyFromAttribute<Db.AssociationAttribute>(e => e.OtherKey);

            return builder;
        }

        public IDbConnection Connection { get; private set; }
        public IMapper Mapper { get; private set; }
        public IQueryProvider QueryProvider { get; private set; }
        public IQueryTranslator Translator { get; private set; }
        public IConnection ConnectionManager { get; private set; }

        public object ExecuteScalar(string commandText, IDictionary<string, object> parameters)
        {
            var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteScalar();
        }

        public IDataReader ExecuteReader(string commandText, IDictionary<string, object> parameters)
        {
            var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteReader();
        }

        public void ExecuteNonQuery(string commandText, IDictionary<string, object> parameters)
        {
            var cmd = CreateCommand(commandText, parameters);
            cmd.ExecuteNonQuery();
        }
    }
}
