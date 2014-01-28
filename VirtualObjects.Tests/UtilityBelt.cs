using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using VirtualObjects.Config;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Translation;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests
{
    public abstract class UtilityBelt
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
        }

        public IDataReader Execute(IQueryInfo query)
        {
            return CreateCommand(query).ExecuteReader();
        }

        public IDbCommand CreateCommand(IQueryInfo queryInfo)
        {
            
            var cmd = Connection.CreateCommand();
            cmd.CommandText = queryInfo.CommandText;

            Trace.WriteLine("Query: " + cmd.CommandText);

            queryInfo.Parameters
                .Select(e => new { e, Parameter = cmd.CreateParameter()})
                .Select(e =>
                            {
                                e.Parameter.ParameterName = e.e.Key;
                                e.Parameter.Value = e.e.Value;
                                cmd.Parameters.Add(e.Parameter);
                                return e.Parameter;
                            }).ToList();
            return cmd;

        }

        public IQueryInfo TranslateQuery(IQueryable query)
        {
            return new QueryTranslator(new SqlFormatter(), Mapper).TranslateQuery(query);
        }

        public IQueryable<T> Query<T>()
        {
            return new List<T>().AsQueryable();
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
