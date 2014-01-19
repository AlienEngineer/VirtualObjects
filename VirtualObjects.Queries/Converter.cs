using System;
using System.Data;
using System.Data.Linq;
using System.Linq;
using VirtualObjects.Queries.LinqToSql;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// Converts the ExpressionTree instance to a QueryInfo.
    /// </summary>
    public interface IConverter
    {
        IQueryInfo ConvertToQuery(IQueryable queryable);
    }

    class Converter : DataContext, IConverter
    {
        public Converter(IDbConnection connection, Type providerType)
            : base(connection, new LinqToSqlMapper(providerType))
        {
        }

        #region Implementation of IConverter

        public IQueryInfo ConvertToQuery(IQueryable queryable)
        {
            return new QueryInfo
            {
                Command = GetCommand(queryable)
            };
        }

        #endregion
    }
}
