using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private QueryCompiler CreateQueryCompiler()
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
            CompileFrom(buffer);
            CompilePredicates(query, buffer);
            CompileJoins(query, buffer);

            return new QueryInfo
            {
                CommandText = Merge(buffer)
            };
        }

        private void CompileJoins(IBuiltedQuery query, CompilerBuffer buffer)
        {
            foreach ( var join in query.Joins.Cast<LambdaExpression>() )
            {
                var entityInfo = _mapper.Map(join.Parameters[1].Type);
                var binary = (BinaryExpression) join.Body;

                var leftMemberName = ExtractName(binary.Left, buffer.EntityInfo);
                var rightMemberName = ExtractName(binary.Right, entityInfo);

                buffer.Joins += " Inner Join ";
                buffer.Joins += _formatter.FormatTableName(entityInfo.EntityName, ++_rootCompiler._depth);
                buffer.Joins += " On (";
                buffer.Joins += _formatter.FormatFieldWithTable(buffer.EntityInfo[leftMemberName].ColumnName, _index);
                buffer.Joins += " = ";
                buffer.Joins += _formatter.FormatFieldWithTable(entityInfo[rightMemberName].ColumnName, _rootCompiler._depth);
                buffer.Joins += ")";
            }
        }

        private string ExtractName(Expression expression, IEntityInfo entityInfo)
        {
            if (expression is MemberExpression)
                return ((MemberExpression) expression).Member.Name;
            if (expression is ParameterExpression)
                return entityInfo.KeyColumns.First().ColumnName;

            return null;
        }


        private string Merge(CompilerBuffer buffer)
        {
            return new StringBuilder()
                .Append("Select ").Append(buffer.Projection)
                .Append(" From ").Append(buffer.From)
                .Append(buffer.Predicates)
                .Append(buffer.Joins)
                .ToString();
        }

        private void CompileProjection(IBuiltedQuery query, CompilerBuffer buffer)
        {
            var lambda = (LambdaExpression)query.Projection;

            var returnType = lambda != null ? ExtractType(lambda.Body) : query.SourceType;

            buffer.Projection = String.Join(_formatter.FieldSeparator,
                returnType
                    .Properties()
                    .Select(e => buffer.EntityInfo[e.Name])
                    .Select(e => _formatter.FormatFieldWithTable(e.ColumnName, _index))
            );
        }

        private void CompileFrom(CompilerBuffer buffer)
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
            CompilePredicateExpression(query, predicate, buffer);

            foreach ( var expression in query.Predicates.Skip(1) )
            {
                buffer.Predicates += _formatter.FormatNode(ExpressionType.AndAlso);
                CompilePredicateExpression(query, expression, buffer);
            }
        }

        private void CompilePredicateExpression(IBuiltedQuery query, Expression expression, CompilerBuffer buffer)
        {
            switch ( expression.NodeType )
            {
                case ExpressionType.Lambda:
                    CompilePredicateExpression(query, ((LambdaExpression)expression).Body, buffer);
                    break;

                case ExpressionType.New:
                    CompileNew(expression, buffer);
                    break;

                case ExpressionType.MemberInit:
                    CompileMemberInit(expression, buffer);
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

                case ExpressionType.Not:
                    CompileNot(expression, buffer, query);
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
                    CompileBinaryExpression(query, expression, buffer);
                    break;

                default:
                    throw new UnsupportedException(Errors.SQL_ExpressionTypeNotSupported, expression);
            }

        }

        private void CompileNot(Expression expression, CompilerBuffer buffer, IBuiltedQuery query)
        {
            var unary = ((UnaryExpression) expression);

            CompilePredicateExpression(query, Expression.NotEqual(unary.Operand, Expression.Constant(true)), buffer);
        }

        private void CompileNew(Expression expression, CompilerBuffer buffer)
        {
            var newExpression = (NewExpression) expression;
            
            object value;

            if (newExpression.Constructor != null)
            {
                value = newExpression.Constructor.Invoke(GetParameters(newExpression));
            }
            else
            {
                value = newExpression.Type.CreateInstance();
            }
            
            
            CompileConstant(Expression.Constant(value), buffer);
        }

        private void CompileMemberInit(Expression expression, CompilerBuffer buffer)
        {
            var value = Expression.Lambda(expression).Compile().DynamicInvoke();

            CompileConstant(Expression.Constant(value), buffer);
        }

        private static object ParseValue(Expression arg)
        {
            var constantExpression = arg as ConstantExpression;
            if ( constantExpression != null )
            {
                return constantExpression.Value;
            }

            var fieldExpression = arg as MemberExpression;
            if ( fieldExpression != null )
            {
                return Expression.Lambda(arg).Compile().DynamicInvoke();
            }

            return null;
        }

        private static object[] GetParameters(NewExpression expression)
        {
            return expression.Arguments.Select(ParseValue).ToArray();
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

            if ( CompileIfConstant(expression, buffer) ) return;


            //
            // If the member is from the current entity.
            //
            if ( member.Expression is ParameterExpression )
            {
                buffer.Predicates += _formatter
                    .FormatFieldWithTable(buffer.EntityInfo[member.Member.Name].ColumnName, _index);
            }
            else
            {
                CompileMemberAccess(member, member.Expression, buffer);
            }
        }

        private IEntityInfo CompileMemberAccess(MemberExpression expression, Expression nextExpression, CompilerBuffer buffer)
        {
            if ( nextExpression is ParameterExpression )
            {
                return _mapper.Map(nextExpression.Type);
            }

            var nextMember = nextExpression as MemberExpression;

            Debug.Assert(nextMember != null, "CompileMemberAccess : nextMember != null");

            var entityInfo = CompileMemberAccess(nextMember, nextMember.Expression, buffer);

            var foreignKey = entityInfo.GetFieldAssociatedWith(expression.Member.Name);

            //
            // Use the binded field on the predicate.
            //
            buffer.Predicates += _formatter.FormatFieldWithTable(foreignKey.ColumnName, _index);
            buffer.Predicates += " In (Select ";
            buffer.Parenthesis++;

            var queryCompiler = CreateQueryCompiler();

            //
            // Use the key of the other type to predicate.
            //
            foreignKey = foreignKey.ForeignKey;
            buffer.Predicates += _formatter.FormatFieldWithTable(foreignKey.ColumnName, queryCompiler._index);
            buffer.Predicates += " From ";
            buffer.Predicates += _formatter.FormatTableName(foreignKey.EntityInfo.EntityName, queryCompiler._index);
            buffer.Predicates += " Where ";
            buffer.Predicates += _formatter.FormatFieldWithTable(foreignKey.ColumnName, queryCompiler._index);

            return entityInfo;
        }

        private bool CompileIfConstant(Expression expression, CompilerBuffer buffer)
        {
            var constant = ExtractConstant(expression);

            if ( constant != null )
            {
                CompileConstant(constant, buffer);
                return true;
            }
            return false;
        }

        private void CompileBinaryExpression(IBuiltedQuery query, Expression expression, CompilerBuffer buffer)
        {
            var binary = expression as BinaryExpression;

            if ( binary == null )
            {
                throw new UnsupportedException(Errors.Internal_WrongMethodCall, expression);
            }

            buffer.Predicates += "(";

            CompilePredicateExpression(query, binary.Left, buffer);
            CompileNodeType(binary.NodeType, buffer);
            CompilePredicateExpression(query, binary.Right, buffer);

            buffer.Predicates += new string(')', buffer.Parenthesis + 1);
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
        public int Parenthesis { get; set; }
        public String Joins { get; set; }
    }

}