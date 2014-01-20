using System;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Queries.Compilation;

namespace VirtualObjects.Queries.Builder
{
    class ExpressionBasedBuilder : IBuiltedQuery, IQueryBuilder 
    {
        private readonly IQueryCompiler queryCompiler;

        public ExpressionBasedBuilder(IQueryCompiler queryCompiler)
        {
            this.queryCompiler = queryCompiler;
        }

        public Expression Projection { get; set; }
        public Type SourceType { get; set; }
        
        public IQueryInfo BuildQuery()
        {
            return queryCompiler.CompileQuery(this);
        }
  
        public void Project(Expression projection)
        {
            Projection = projection;
        }

        public void Project<T>(Expression<Func<T, Object>> projection)
        {
            Project((Expression)projection);
        }

        public void From(Type src)
        {
            SourceType = src;
        }

        public void From<T>()
        {
            From(typeof(T));
        }

    }
}
