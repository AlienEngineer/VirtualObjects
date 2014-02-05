using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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


        public object ExecuteQuery(Expression expression, SessionContext context)
        {
            var executor = GetQueryExecutor(expression);

            return executor.ExecuteQuery(expression, context);
        }

        private IQueryExecutor GetQueryExecutor(Expression expression)
        {
            MethodInfo method = ExtractFistMethod(expression);

            var executor = _executors.FirstOrDefault(e => e.CanExecute(method));

            if (executor == null)
            {
                if (method == null)
                {
                    throw new ExecutionException(Errors.Execution_UnableToFindExecutor);
                }
                
                throw new ExecutionException(Errors.Execution_UnableToFindSpecificExecutor, method);
            }
            return executor;
        }

        private MethodInfo ExtractFistMethod(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;

            return callExpression != null ? callExpression.Method : null;
        }

        public TResult ExecuteQuery<TResult>(Expression expression, SessionContext context)
        {
            var executor = GetQueryExecutor(expression);

            return executor.ExecuteQuery<TResult>(expression, context);
        }

        public bool CanExecute(MethodInfo method)
        {
            return _executors.Any(e => e.CanExecute(method));
        }
    }
}