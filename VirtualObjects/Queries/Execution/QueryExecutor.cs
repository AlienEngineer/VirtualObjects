using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VirtualObjects.Reflection;

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


        public virtual object ExecuteQuery(Expression expression, SessionContext context)
        {
            var queryInfo = _translator.TranslateQuery(expression, context);

            return MapEntities(queryInfo, context);
        }

        private object MapEntities(IQueryInfo queryInfo, SessionContext context)
        {
            var reader = context.Connection.ExecuteReader(queryInfo.CommandText, queryInfo.Parameters);

            var mapper = queryInfo.EntitiesMapper ?? _mapper;

            return mapper.MapEntities(reader, queryInfo, queryInfo.OutputType, context);
        }

        public virtual TResult ExecuteQuery<TResult>(Expression expression, SessionContext context)
        {
            var queryInfo = _translator.TranslateQuery(expression, context);

            var methodIterator = ProxyGenericIteratorMethod.MakeGenericMethod(queryInfo.OutputType);
            var result = MapEntities(queryInfo, context);

            return (TResult)methodIterator.Invoke(null, new[] { result });
        }

        public virtual bool CanExecute(MethodInfo method)
        {
            return method == null ||
                method.ReturnType.InheritsOrImplements<IEnumerable>() ||
                method.Name == "Select" ||
                method.Name == "Union" ||
                method.Name == "Distinct";
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
