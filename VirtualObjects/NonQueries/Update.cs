using System;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries;

namespace VirtualObjects.NonQueries
{
    class Update<TEntity> : IUpdate<TEntity>
    {
        public IQueryable<TEntity> Query { get; private set; }
        public SessionContext Context { get; private set; }

        public Update(SessionContext context, IQueryable<TEntity> query)
        {
            Query = query;
            Context = context;
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

            return queryInfo;
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