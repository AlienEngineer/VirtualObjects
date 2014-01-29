﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace VirtualObjects.Queries.Execution
{
    class QueryExecutor : IQueryExecutor
    {
        private readonly IEntitiesMapper _mapper;
        private readonly IQueryTranslator _translator;

        private static readonly MethodInfo ProxyGenericIteratorMethod =
            typeof(QueryExecutor)
                .GetMethod(
                    "ProxyGenericIterator",
                    BindingFlags.NonPublic | BindingFlags.Static);

        public QueryExecutor(IEntitiesMapper mapper, IQueryTranslator translator)
        {
            _mapper = mapper;
            _translator = translator;
        }


        public virtual object ExecuteQuery(Expression expression, Context context)
        {
            return MapEntities(expression, context);
        }

        private object MapEntities(Expression expression, Context context)
        {
            var queryInfo = _translator.TranslateQuery(expression);

            var reader = context.Connection.ExecuteReader(queryInfo.CommandText, queryInfo.Parameters);

            return _mapper.MapEntities(reader, queryInfo, queryInfo.OutputType);
        }

        public virtual TResult ExecuteQuery<TResult>(Expression expression, Context context)
        {
            var methodIterator = ProxyGenericIteratorMethod.MakeGenericMethod(typeof(TResult));
            var result = MapEntities(expression, context);

            return (TResult)methodIterator.Invoke(null, new[] { result });
        }

        public virtual bool CanExecute(MethodInfo method)
        {
            return method == null || method.ReturnType.IsAssignableFrom(typeof(IEnumerable));
        }

        private static IEnumerable<T> ProxyGenericIterator<T>(IEnumerable enumerable)
        {
            return ProxyNonGenericIterator(enumerable).Cast<T>();
        }

        private static IEnumerable ProxyNonGenericIterator(IEnumerable enumerable)
        {
            return enumerable.Cast<object>();
        }
    }
}