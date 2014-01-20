using System;
using System.Linq.Expressions;
using VirtualObjects.Config;
using VirtualObjects.Queries.Builder;
using Fasterflect;
using System.Linq;
using System.Text;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries.Compilation
{
    class QueryCompiler : IQueryCompiler
    {
        private readonly int _index;
        private readonly IFormatter _formatter;
        private readonly IMapper _mapper;

        public QueryCompiler(IFormatter formatter, IMapper mapper, int index = 0)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = index;
        }

        /// <summary>
        /// Compiles the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IQueryInfo CompileQuery(IBuiltedQuery query)
        {
            query.SourceType.RaiseIfNull(Errors.Query_SourceNotSet);

            var buffer = new CompilerBuffer()
            {
                EntityInfo = _mapper.Map(query.SourceType)
            };

            buffer.EntityInfo.RaiseIfNull(Errors.Query_EntityInfoNotFound, query);

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

            buffer.Projection = String.Join(_formatter.FieldSeparator, 
                returnType
                    .Properties()
                    .Select(e => buffer.EntityInfo[e.Name])
                    .Select(e=> _formatter.FormatFieldWithTable(e.ColumnName, _index))
            );
        }

        private void CompileFrom(IBuiltedQuery query, CompilerBuffer buffer)
        {
            buffer.From = _formatter.FormatTableName(buffer.EntityInfo.EntityName, _index);
        }

        private static Type ExtractType(Expression expression)
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
        public IEntityInfo EntityInfo { get; set; }
    }
    
}