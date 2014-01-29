﻿using System;
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
            var queryInfo = _translator.TranslateQuery(expression);

            return context.Connection.ExecuteScalar(queryInfo.CommandText, queryInfo.Parameters);
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
                method.Name == "Sum" ||
                method.Name == "Min" && method.Parameters().Count == 2 ||
                method.Name == "Max" && method.Parameters().Count == 2 || 
                method.Name == "Average");
        }
    }
}