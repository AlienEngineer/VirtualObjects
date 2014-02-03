using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fasterflect;

namespace VirtualObjects.Queries.Execution
{
    class CountQueryExecutor : IQueryExecutor
    {
        private readonly IQueryTranslator _translator;

        public CountQueryExecutor(IQueryTranslator translator)
        {
            _translator = translator;
        }

        public object ExecuteQuery(Expression expression, Context context)
        {
            try
            {
                var queryInfo = _translator.TranslateQuery(expression);

                return context.Connection.ExecuteScalar(queryInfo.CommandText, queryInfo.Parameters);
            }
            finally
            {
                context.Connection.Close();
            }
        }

        public TResult ExecuteQuery<TResult>(Expression query, Context context)
        {
            return (TResult) Convert.ChangeType( ExecuteQuery(query, context), typeof(TResult));
        }

        public bool CanExecute(MethodInfo method)
        {
            return method != null && (
                method.Name == "Count" ||
                method.Name == "LongCount" ||
                method.Name == "Any" ||
                method.Name == "Sum" ||
                method.Name == "Contains" ||
                method.Name == "Average" ||
                (method.Parameters().Count == 2 && (method.Name == "Min" || method.Name == "Max")));
        }
    }
}