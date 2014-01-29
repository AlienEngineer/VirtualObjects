﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using VirtualObjects.Exceptions;

namespace VirtualObjects.Queries.Execution
{
    class CompositeExecutor : IQueryExecutor
    {
        private readonly IEnumerable<IQueryExecutor> _executors;

        public CompositeExecutor(IEnumerable<IQueryExecutor> executors)
        {
            _executors = executors;
        }


        public object ExecuteQuery(Expression expression, Context context)
        {
            MethodInfo method = ExtractFistMethod(expression);

            var executor = _executors.FirstOrDefault(e => e.CanExecute(method));

            if ( executor == null )
            {
                if (method == null)
                {
                    throw new ExecutionException("Unable to find the proper executor for the given query.");
                }
                else
                {
                    throw new ExecutionException("Unable to find the proper executor for {Name} method.", method);
                }
            }

            return executor.ExecuteQuery(expression, context);
        }

        private MethodInfo ExtractFistMethod(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;

            return callExpression != null ? callExpression.Method : null;
        }

        public TResult ExecuteQuery<TResult>(Expression expression, Context context)
        {
            return (TResult)Convert.ChangeType(ExecuteQuery(expression, context), typeof(TResult));
        }

        public bool CanExecute(MethodInfo method)
        {
            return _executors.Any(e => e.CanExecute(method));
        }
    }
}