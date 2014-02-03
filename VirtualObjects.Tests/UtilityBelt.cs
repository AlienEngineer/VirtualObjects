using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
        public const int Repeat = 10;

        protected UtilityBelt()
        {
            InitBelt();
        }

        int _count;

        [TearDown]
        public void FlushTime()
        {
            if ( !TestContext.CurrentContext.Test.Properties.Contains("Repeat") )
            {
                return;
            }

            var times = (int)TestContext.CurrentContext.Test.Properties["Repeat"];

            _count++;

            if ( _count % times != 0 ) return;

            Diagnostic.PrintTime(TestContext.CurrentContext.Test.Name + " => executed in time :   {1} ms");

        }

        private void InitBelt()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data"));

            Connection = new SqlConnection(@"
                      Data Source=(LocalDB)\v11.0;
                      AttachDbFilename=|DataDirectory|\northwnd.mdf;
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



        IDbTransaction _dbTransaction;

        [SetUp]
        public void SetUpConnection()
        {
            Connection.Open();
        }

        [TearDown]
        public void CleanUpConnection()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
                _dbTransaction = null;
            }
            Connection.Close();
        }

        /// <summary>
        /// This helper will begin a transaction and will rollback any changes at every unit-test TearDown
        /// </summary>
        public void RollBackOnTearDown()
        {
            if (_dbTransaction == null)
            {
                _dbTransaction = Connection.BeginTransaction();    
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

        private IDbCommand CreateCommand(String commandText, IEnumerable<KeyValuePair<string, IOperationParameter>> parameters)
        {
            var cmd = Connection.CreateCommand();

            cmd.Transaction = _dbTransaction;
            cmd.CommandText = commandText;

            Trace.WriteLine("Command: " + cmd.CommandText);

            parameters
                .Select(e => new { OperParameter = e, Parameter = cmd.CreateParameter() })
                .ForEach(e =>
                {
                    e.Parameter.ParameterName = e.OperParameter.Key;
                    e.Parameter.Value = e.OperParameter.Value.Value ?? DBNull.Value;

                    if ( e.OperParameter.Value.Type == typeof(Byte[]) )
                    {
                        e.Parameter.DbType = DbType.Binary;
                    }

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
// ReSharper disable once AssignNullToNotNullAttribute
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
            builder.ColumnKeyFromAttribute<Db.KeyAttribute>();
            builder.ColumnKeyFromAttribute<Db.IdentityAttribute>();

            builder.ColumnIdentityFromAttribute<Db.IdentityAttribute>();

            builder.ForeignKeyFromAttribute<Db.AssociationAttribute>(e => e.OtherKey);

            builder.ColumnVersionFromProperty(e => e.Name == "Version");
            builder.ColumnVersionFromAttribute<Db.VersionAttribute>();

            return builder;
        }

        public IDbConnection Connection { get; private set; }
        public IMapper Mapper { get; private set; }
        public IQueryProvider QueryProvider { get; private set; }
        public IQueryTranslator Translator { get; private set; }
        public IConnection ConnectionManager { get; private set; }

        public object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteScalar();
        }

        public IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            var cmd = CreateCommand(commandText, parameters);
            return cmd.ExecuteReader();
        }

        public void ExecuteNonQuery(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            var cmd = CreateCommand(commandText, parameters);
            cmd.ExecuteNonQuery();
        }
    }
}
