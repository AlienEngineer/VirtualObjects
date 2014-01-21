using System;
using System.Collections.Generic;
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
        private readonly IDictionary<String, Object> _parameters;
        private int _depth;
        private QueryCompiler _rootCompiler;

        public QueryCompiler(IFormatter formatter, IMapper mapper)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = _depth = 0;
            _parameters = new Dictionary<String, Object>();
            _rootCompiler = this;
        }

        public QueryCompiler(IFormatter formatter, IMapper mapper, int index)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = index;
        }

        private IDictionary<string, object> Parameters
        {
            get { return _rootCompiler._parameters; }
        }

        private IQueryCompiler CreateQueryCompiler()
        {
            return new QueryCompiler(_formatter, _mapper, ++_rootCompiler._depth)
            {
                //
                // Bind the new compile to the root.
                // This binding is used to share parameters and such.
                _rootCompiler = _rootCompiler
            };
        }


        /// <summary>
        /// Compiles the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IQueryInfo CompileQuery(IBuiltedQuery query)
        {
            query.SourceType.RaiseIfNull(Errors.Query_SourceNotSet);

            var buffer = new CompilerBuffer
            {
                EntityInfo = _mapper.Map(query.SourceType)
            };

            buffer.EntityInfo.RaiseIfNull(Errors.Query_EntityInfoNotFound, query);

            CompileProjection(query, buffer);
            CompileFrom(query, buffer);
            CompilePredicates(query, buffer);

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
                .Append(buffer.Predicates)
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
                    .Select(e => _formatter.FormatFieldWithTable(e.ColumnName, _index))
            );
        }

        private void CompileFrom(IBuiltedQuery query, CompilerBuffer buffer)
        {
            buffer.From = _formatter.FormatTableName(buffer.EntityInfo.EntityName, _index);
        }

        private void CompilePredicates(IBuiltedQuery query, CompilerBuffer buffer)
        {
            if ( buffer.Predicates == null && query.Predicates.Any() )
            {
                buffer.Predicates = " Where ";
            }

            if ( !query.Predicates.Any() )
            {
                return;
            }

            var predicate = query.Predicates.First();
            CompilePredicateExpression(predicate, buffer);

            foreach ( var expression in query.Predicates.Skip(1) )
            {
                buffer.Predicates += _formatter.FormatNode(ExpressionType.AndAlso);
                CompilePredicateExpression(expression, buffer);
            }
        }

        private void CompilePredicateExpression(Expression expression, CompilerBuffer buffer)
        {
            switch ( expression.NodeType )
            {
                case ExpressionType.Lambda:
                    CompilePredicateExpression(((LambdaExpression)expression).Body, buffer);
                    break;

                case ExpressionType.MemberAccess:
                    CompileMemberAccess(expression, buffer);
                    break;

                case ExpressionType.Constant:
                    CompileConstant(expression, buffer);
                    break;

                case ExpressionType.Invoke:
                    CompileInvoke(expression, buffer);
                    break;

                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                case ExpressionType.OrElse:
                case ExpressionType.AndAlso:
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Divide:
                case ExpressionType.Multiply:
                    CompileBinaryExpression(expression, buffer);
                    break;

                default:
                    throw new UnsupportedException(Errors.SQL_ExpressionTypeNotSupported, expression);
            }

        }

        private void CompileInvoke(Expression expression, CompilerBuffer buffer)
        {
            var invoke = expression as InvocationExpression;

            if ( invoke == null )
            {
                throw new UnsupportedException(Errors.Internal_WrongMethodCall, expression);
            }

            CompileConstant(Expression.Constant(Expression.Lambda(expression).Compile().DynamicInvoke()), buffer);
        }

        private void CompileConstant(Expression expression, CompilerBuffer buffer)
        {
            var constant = expression as ConstantExpression;

            if ( constant == null )
            {
                throw new UnsupportedException(Errors.Internal_WrongMethodCall, expression);
            }

            var formatted = _formatter.FormatConstant(constant.Value, Parameters.Count);

            Parameters[formatted] = constant.Value;

            buffer.Predicates += formatted;
        }

        private void CompileMemberAccess(Expression expression, CompilerBuffer buffer)
        {
            var member = expression as MemberExpression;

            if ( member == null )
            {
                throw new UnsupportedException(Errors.Internal_WrongMethodCall, expression);
            }

            var constant = ExtractConstant(expression);

            if ( constant != null )
            {
                CompileConstant(constant, buffer);
                return;
            }

            buffer.Predicates += _formatter.FormatFieldWithTable(buffer.EntityInfo[member.Member.Name].ColumnName, _index);
        }

        private void CompileBinaryExpression(Expression expression, CompilerBuffer buffer)
        {
            var binary = expression as BinaryExpression;

            if ( binary == null )
            {
                throw new UnsupportedException(Errors.Internal_WrongMethodCall, expression);
            }

            buffer.Predicates += "(";

            CompilePredicateExpression(binary.Left, buffer);
            CompileNodeType(binary.NodeType, buffer);
            CompilePredicateExpression(binary.Right, buffer);

            buffer.Predicates += ")";
        }

        private void CompileNodeType(ExpressionType nodeType, CompilerBuffer buffer)
        {
            buffer.Predicates += _formatter.FormatNode(nodeType);
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

        private static Expression ExtractConstant(Expression expression)
        {
            if ( expression == null )
            {
                return null;
            }

            switch ( expression.NodeType )
            {
                case ExpressionType.MemberAccess:
                    return ExtractConstant(((MemberExpression)expression).Expression);
                case ExpressionType.Constant:
                    return expression;
                default: return null;
            }
        }
    }

    public class CompilerBuffer
    {
        public String Projection { get; set; }
        public String From { get; set; }
        public IEntityInfo EntityInfo { get; set; }
        public String Predicates { get; set; }
    }

}