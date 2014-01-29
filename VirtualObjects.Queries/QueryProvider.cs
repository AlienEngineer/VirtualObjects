﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VirtualObjects.Queries.Execution;

namespace VirtualObjects.Queries
{
    class QueryProvider : IQueryProvider
    {
        private readonly IQueryExecutor _executor;
        private readonly Context _context;

        public QueryProvider(IQueryExecutor executor, Context context)
        {
            _executor = executor;
            _context = context;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return expression != null
                       ? new Query<TElement>(this, expression)
                       : new Query<TElement>(this);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var query = ExtractQuery(expression);

            var elementType = query == null ? 
                (Type)((ConstantExpression)expression).Value : 
                query.ElementType;

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>)
                    .MakeGenericType(elementType), new object[] { this, expression });
            }
            catch ( TargetInvocationException tie )
            {
                throw tie.InnerException;
            }
        }

        public object Execute(Expression expression)
        {
            return _executor.ExecuteQuery(expression, _context);
            //var m = typeof(QueryProvider).GetMethod("InnerExecute").MakeGenericMethod(query.ElementType);
            //return m.Invoke(this, new[] { expression });
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _executor.ExecuteQuery<TResult>(expression, _context);
        }
        
        private static IQueryable ExtractQuery(Expression expression)
        {
            while (true)
            {
                var callExpression = expression as MethodCallExpression;
                if (callExpression != null)
                {
                    expression = callExpression.Arguments[0];
                    continue;
                }
                return ((ConstantExpression) expression).Value as IQueryable;
            }
        }
    }
}