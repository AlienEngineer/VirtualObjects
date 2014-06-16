using System;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries;

namespace VirtualObjects.NonQueries
{
    class Update<TEntity> : Query<TEntity>, IUpdate<TEntity>
    {
        
        public IQueryable<TEntity> Query { get; private set; }

        public Update(IQueryProvider provider, Expression expression, SessionContext context)
            : base(provider, expression, context) { }

        public Update(IQueryProvider provider, SessionContext context)
            : base(provider, context) { }

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
            var query = this.Select(e => new { NOFIELD = 1 });

            //
            // Replace stub projection for update statements.

            var queryInfo = Context.Translator.TranslateQuery(query);

            return queryInfo;
        }

        public INonQuery<TEntity> Where(Func<IQueryable<TEntity>, IQueryable<TEntity>> clause)
        {
            if (Query != null)
            {
                throw new NonQueryOperationException("Unable to add more than one query.");
            }

            Query = clause(Provider.CreateQuery<TEntity>(null));

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