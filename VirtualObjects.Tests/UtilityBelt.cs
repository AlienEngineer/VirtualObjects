using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using VirtualObjects.Config;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests
{
    public abstract class UtilityBelt
    {
        protected UtilityBelt()
        {
            InitBelt();
        }

        int _count = 0;

        [TearDown]
        public void FlushTime()
        {
            if (!TestContext.CurrentContext.Test.Properties.Contains("Repeat"))
            {
                return;
            }

            var times = (int)TestContext.CurrentContext.Test.Properties["Repeat"];

            _count++;

            if (_count%times != 0) return;

            Diagnostic.PrintAverageTime(TestContext.CurrentContext.Test.Name + " => Translation parsed in time :   {1} ms", "Translation");
            Diagnostic.PrintAverageTime(TestContext.CurrentContext.Test.Name + " Values parsed in time :    {1} ms", "Values");
        }

        private void InitBelt()
        {
            Connection = new SqlConnection(@"
                      Data Source=(LocalDB)\v11.0;
                      AttachDbFilename=" + Environment.CurrentDirectory + @"\Data\northwnd.mdf;
                      Integrated Security=True;
                      Connect Timeout=30");

            Mapper = CreateBuilder().Build();
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
    }
}
