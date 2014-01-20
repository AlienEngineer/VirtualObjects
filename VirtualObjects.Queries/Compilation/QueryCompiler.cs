using System;
using System.Linq.Expressions;
using VirtualObjects.Queries.Builder;
using Fasterflect;
using System.Linq;
using System.Text;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries.Compilation
{
    class QueryCompiler : IQueryCompiler
    {
        private readonly int index;
        private readonly IFormatter formatter;

        public QueryCompiler(IFormatter formatter)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Compiles the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IQueryInfo CompileQuery(IBuiltedQuery query)
        {
            var buffer = new CompilerBuffer();

            CompileProjection(query, buffer);
            CompileFrom(query, buffer);

            return new QueryInfo
            {
                CommandText = Merge(buffer)
            };
        }
  
        private string Merge(CompilerBuffer buffer)
        {
            return new StringBuilder()
                .Append("Select ").Append(buffer.Projection)
                .Append(" From ").Append(buffer.From)
                .ToString();
        }
  
        private void CompileProjection(IBuiltedQuery query, CompilerBuffer buffer)
        {
            var lambda = (LambdaExpression)query.Projection;

            var returnType = ExtractType(lambda.Body);

            buffer.Projection = String.Join(formatter.FieldSeparator, 
                returnType.Properties().Select(e=> formatter.FormatFieldWithTable(e.Name, index))
            );
        }

        private void CompileFrom(IBuiltedQuery query, CompilerBuffer buffer)
        {
            buffer.From = formatter.FormatField(query.SourceType.Name);
        }

        private Type ExtractType(Expression expression)
        {
            switch ( expression.NodeType )
            {
                case ExpressionType.New: 
                case ExpressionType.Parameter:
                    return expression.Type;
                default:
                    throw new UnsupportedException(Errors.UnableToGetType, expression);
            }
        }
    }
  
    public class CompilerBuffer
    {
        public String Projection { get; set; }
        public String From { get; set; }
    }
    
}