using System;
using System.Linq;
using System.Linq.Expressions;

namespace VirtualObjects.Queries.Builder
{
    class ExpressionBasedBuilder : IBuiltedQuery, IQueryBuilder 
    {
        public Expression Projection { get; set; }

        public IQueryInfo BuildQuery()
        {
            return Build((IBuiltedQuery)this);
        }
  
        private IQueryInfo Build(IBuiltedQuery query)
        {
            return new QueryInfo
            {
                CommandText = ""
            };
        }

        public void Project(Expression projection)
        {
            Projection = projection;
        }

        public void Project<T>(Expression<Func<T, Object>> projection)
        {
            Project((Expression)projection);
        }
    }
}
