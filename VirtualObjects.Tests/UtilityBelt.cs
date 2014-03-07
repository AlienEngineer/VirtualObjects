using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;
using VirtualObjects.Config;
using VirtualObjects.Queries;

namespace VirtualObjects.Tests
{
    public class TestsBase
    {

        public TestsBase()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(TestContext.CurrentContext.TestDirectory, "Data"));
        }
 
    }

    public class TimedTests : TestsBase
    {
        public const int Repeat = 10;

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
    }

    public abstract class UtilityBelt : TimedTests
    {
        protected UtilityBelt()
        {
            InitBelt();
        }

        private void InitBelt()
        {
            var ioc = new NinjectContainer(new SessionConfiguration
            {
                // Logger = Console.Out
            }, "northwind");

            ConnectionManager = ioc.Get<IConnection>();
            Mapper = ioc.Get<IMapper>();
            Translator = ioc.Get<IQueryTranslator>();
            QueryProvider = ioc.Get<IQueryProvider>();
            SessionContext = ioc.Get<SessionContext>();

            Session = new Session(ioc);

            Connection = ConnectionManager.DbConnection;
        }

        ITransaction _dbTransaction;

        [SetUp]
        public void SetUpConnection()
        {
        }

        [TearDown]
        public void CleanUpConnection()
        {
            if (_dbTransaction != null)
            {
                _dbTransaction.Rollback();
                _dbTransaction = null;
            }
            ConnectionManager.Close();
        }

        /// <summary>
        /// This helper will begin a transaction and will rollback any changes at every unit-test TearDown
        /// </summary>
        public void RollBackOnTearDown()
        {
            if (_dbTransaction == null)
            {
                _dbTransaction = ConnectionManager.BeginTransaction();    
            }
        }

        public IDataReader ExecuteReader(IQueryInfo query)
        {
            return ExecuteReader(query.CommandText, query.Parameters);
        }

        public Object ExecuteScalar(IQueryInfo query)
        {
            return ExecuteScalar(query.CommandText, query.Parameters);
        }
        
        public IQueryInfo TranslateQuery(IQueryable query)
        {
            return Translator.TranslateQuery(query);
        }

        public IQueryable<T> Query<T>() where T : class, new()
        {
            return Session.Query<T>();
        }

        public Session Session { get; private set; }
        public IDbConnection Connection { get; private set; }
        public IMapper Mapper { get; private set; }
        public IQueryProvider QueryProvider { get; private set; }
        public IQueryTranslator Translator { get; private set; }
        public IConnection ConnectionManager { get; private set; }
        public SessionContext SessionContext { get; private set; }

        public object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return ConnectionManager.ExecuteScalar(commandText, parameters);
        }

        public IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            return ConnectionManager.ExecuteReader(commandText, parameters);
        }

        public void ExecuteNonQuery(string commandText, IDictionary<string, IOperationParameter> parameters)
        {
            ConnectionManager.ExecuteNonQuery(commandText, parameters);
        }

    }
}
