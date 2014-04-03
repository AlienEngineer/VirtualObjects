using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Fasterflect;

namespace VirtualObjects.Queries.Execution
{
    class SingleQueryExecutor : QueryExecutor
    {
        public SingleQueryExecutor(IEntitiesMapper mapper, IQueryTranslator translator) 
            : base(mapper, translator)
        {
        }

        public override object ExecuteQuery(Expression expression, SessionContext context)
        {
            return base.ExecuteQuery<IEnumerable<object>>(expression, context).FirstOrDefault();
        }

        public override TResult ExecuteQuery<TResult>(Expression expression, SessionContext context)
        {
            var keepAlive = context.Connection.KeepAlive;
            context.Connection.KeepAlive = true;
            try
            {
                return base.ExecuteQuery<IEnumerable<TResult>>(expression, context).FirstOrDefault();
            }
            finally
            {
                context.Connection.KeepAlive = keepAlive;    
                context.Connection.Close();
            }
        }
        
        public override bool CanExecute(MethodInfo method)
        {
            return method != null && 
                    !method.ReturnType.IsAssignableFrom(typeof(IEnumerable)) && (
                    method.Name.StartsWith("First") ||
                    method.Name.StartsWith("Last") ||
                    method.Name.StartsWith("Single") ||
                    method.Name == "Min" && method.Parameters().Count == 1 ||
                    method.Name == "Max" && method.Parameters().Count == 1);
        }
    }
}