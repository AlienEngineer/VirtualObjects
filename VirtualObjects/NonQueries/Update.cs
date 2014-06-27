using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.CRUD;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.NonQueries
{
    class Update<TEntity> : IUpdate<TEntity>
    {

        class SetEntry
        {
            public Expression Expression { get; set; }
            public Object Value { get; set; }
        }

        public IQueryable<TEntity> Query { get; private set; }
        public SessionContext Context { get; private set; }
        private readonly IFormatter _formatter;

        IList<SetEntry> sets = new List<SetEntry>();

        public Update(SessionContext context, IQueryable<TEntity> query)
        {
            Query = query;
            Context = context;
            _formatter = Context.Formatter;
        }

        public IUpdate<TEntity> Set<TValue>(Expression<Func<TEntity, TValue>> fieldGetter, TValue value)
        {
            sets.Add(new SetEntry
            {
                Expression = fieldGetter,
                Value = value
            });

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

            queryInfo.EntityInfo = Context.Map(typeof (TEntity));

            queryInfo.CommandText = queryInfo.CommandText
                .Replace("Select 1 ", BuildUpdate(queryInfo));

            return queryInfo;
        }

        public String BuildUpdate(IQueryInfo queryInfo)
        {
            StringBuffer sb = _formatter.Update;
            sb += " ";
            sb += _formatter.FormatTableName(queryInfo.EntityInfo.EntityName);

            if (sets.Count == 0)
            {
                throw  new TranslationException("An update cannot be executed without any Set clause.");
            }
            sb += " ";
            sb += _formatter.Set;
            sb += " ";

            var parameters = queryInfo.Parameters;

            foreach (var entry in sets)
            {
                var lambdaExp = entry.Expression as LambdaExpression;

                var memberExp = lambdaExp.Body as MemberExpression;

                var column = queryInfo.EntityInfo.Columns.First(e => e.Property.Name == memberExp.Member.Name);

                sb += _formatter.FormatField(column.ColumnName);

                sb += _formatter.FormatNode(ExpressionType.Equal);

                parameters["@p" + parameters.Count] = new OperationParameter
                {
                    Value = entry.Value
                };

                sb += parameters.Last().Key;
                sb += _formatter.FieldSeparator;
            }

            sb.RemoveLast(_formatter.FieldSeparator);
            sb += " ";

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