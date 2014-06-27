using System;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.NonQueries
{
    class Update<TEntity> : IUpdate<TEntity>
    {
        public IQueryable<TEntity> Query { get; private set; }
        public SessionContext Context { get; private set; }
        private IFormatter _formatter;

        public Update(SessionContext context, IQueryable<TEntity> query)
        {
            Query = query;
            Context = context;
            _formatter = Context.Formatter;
        }

        public IUpdate<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> fieldGetter, TValue value)
        {
            return this;
        }

        public IUpdate<TEntity> Set<TJoinedEntity, TValue>(Expression<Func<TEntity, TValue>> fieldGetter, Expression<Func<TJoinedEntity, TValue>> joinedFieldGetter)
        {
            return this;
        }

        public IQueryInfo Translate()
        {
            //
            // Create a select statement to use as a stub.
            var query = Query.Select(e => new { NOFIELD = 1 });

            //
            // Replace stub projection for update statements.
            var queryInfo = Context.Translator.TranslateQuery(query);

            queryInfo.CommandText = queryInfo.CommandText
                .Replace("Select 1 ", "Update [Employee] Set [T0].[LastName] = @p1 ");

            return queryInfo;
        }

        public String BuildUpdate(QueryInfo queryInfo)
        {
            StringBuffer sb = _formatter.Update;

            sb += _formatter.FormatTableName(queryInfo.EntityInfo.EntityName);

            return sb;
        }

        public INonQuery<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            Query = Query.Where(expression);
            return this;
        }

        public int Execute()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Translate().CommandText;
        }
    }
}