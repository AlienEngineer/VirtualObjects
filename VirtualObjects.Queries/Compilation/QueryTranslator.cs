using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using VirtualObjects.Config;
using Fasterflect;
using System.Linq;
using System.Text;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries.Compilation
{
    class QueryTranslator : IQueryTranslator
    {
        private readonly int _index;
        private readonly IFormatter _formatter;
        private readonly IMapper _mapper;
        private readonly IDictionary<String, Object> _parameters;
        private int _depth;
        private QueryTranslator _rootTranslator;

        public QueryTranslator(IFormatter formatter, IMapper mapper)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = _depth = 0;
            _parameters = new Dictionary<String, Object>();
            _rootTranslator = this;
        }

        public QueryTranslator(IFormatter formatter, IMapper mapper, int index)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = index;
        }

        private IDictionary<string, object> Parameters
        {
            get { return _rootTranslator._parameters; }
        }

        /// <summary>
        /// Translates the query.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IQueryInfo TranslateQuery(IQueryable queryable)
        {
            var buffer = new CompilerBuffer
            {
                EntityInfo = _mapper.Map(queryable.ElementType)
            };

            buffer.EntityInfo.RaiseIfNull(Errors.Query_EntityInfoNotFound, queryable);

            //
            // Starts by compiling a standard From [Source]
            //
            CompileFrom(buffer);
            
            //
            // Compiles the ExpressionTree
            //
            CompileExpression(queryable.Expression, buffer);

            //
            // Conditional Default Projection Compilation.
            //
            CompileDefaultProjection(queryable, buffer);

            return new QueryInfo
            {
                CommandText = Merge(buffer)
            };
        }

        #region Compiling Methods

        private void CompileExpression(Expression expression, CompilerBuffer buffer)
        {
            switch ( expression.NodeType )
            {
                case ExpressionType.Call:
                    CompileMethodCall((MethodCallExpression)expression, buffer); break;

            }
        }

        private void CompileMethodCall(MethodCallExpression expression, CompilerBuffer buffer)
        {
            CompileExpression(expression.Arguments.FirstOrDefault(), buffer);

            if ( expression.Arguments.Count == 1 )
            {
                return;
            }

            switch ( expression.Method.Name )
            {
                case "Select": break;
                case "Where":
                    if (String.IsNullOrEmpty(buffer.Predicates))
                    {
                        buffer.Predicates += " Where ";
                    }
                    else
                    {
                        buffer.Predicates += " And ";
                    }

                    CompileBinaryExpression(expression.Arguments[1], buffer);
                    break;

                case "OrderBy": break;
                default:
                    throw new TranslationException(Errors.Translation_MethodNotSupported, expression);
            }

        }
        
        private void CompileDefaultProjection(IQueryable query, CompilerBuffer buffer)
        {
            if ( !String.IsNullOrEmpty(buffer.Projection) )
            {
                return;
            }

            buffer.Projection = String.Join(_formatter.FieldSeparator,
                    buffer.EntityInfo
                    .Columns
                    .Select(e => _formatter.FormatFieldWithTable(e.ColumnName, _index))
            );
        }

        private void CompileFrom(CompilerBuffer buffer)
        {
            buffer.From = _formatter.FormatTableName(buffer.EntityInfo.EntityName, _index);
        }
        
        private void CompilePredicateExpression(Expression expression, CompilerBuffer buffer)
        {
            switch ( expression.NodeType )
            {
                case ExpressionType.Lambda:
                    CompilePredicateExpression(((LambdaExpression)expression).Body, buffer);
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
                    CompileNot(expression, buffer);
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

        private void CompileNot(Expression expression, CompilerBuffer buffer)
        {
            var unary = ((UnaryExpression)expression);

            CompilePredicateExpression(Expression.NotEqual(unary.Operand, Expression.Constant(true)), buffer);
        }

        private void CompileNew(Expression expression, CompilerBuffer buffer)
        {
            var newExpression = (NewExpression)expression;

            // This is a ReSharper bug this is not allways true...
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            var value = newExpression.Constructor != null ?
                newExpression.Constructor.Invoke(GetParameters(newExpression)) :
                newExpression.Type.CreateInstance();

            CompileConstant(Expression.Constant(value), buffer);
        }

        private void CompileMemberInit(Expression expression, CompilerBuffer buffer)
        {
            var value = Expression.Lambda(expression).Compile().DynamicInvoke();

            CompileConstant(Expression.Constant(value), buffer);
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

            if ( CompileIfDatetime(expression, buffer) ) return;

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

        private bool CompileIfDatetime(Expression expression, CompilerBuffer buffer)
        {
            //
            // This accessor should be the first member of the expression.
            // A parameter or a member.
            //
            var accessor = ExtractAccessor(expression);

            if (accessor.Type != typeof (DateTime))
            {
                return false;
            }


            switch ( expression.NodeType )
            {
                case ExpressionType.MemberAccess:
                    var member = (MemberExpression)expression;

                    switch (member.Member.Name)
                    {
                        case "Now":
                            buffer.Predicates += _formatter.FormatGetDate();
                            break;
                        default: throw new TranslationException(Errors.Translation_DateTimeMemberNotSupported, member);
                    }

                    return true;

                default: 
                    return false;
            }

            return false;
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

        private void CompileBinaryExpression(Expression expression, CompilerBuffer buffer)
        {
            var binary = (BinaryExpression)ExtractLambda(expression).Body;

            buffer.Predicates += "(";

            CompilePredicateExpression(binary.Left, buffer);
            CompileNodeType(binary.NodeType, buffer);
            CompilePredicateExpression(binary.Right, buffer);

            buffer.Predicates += new string(')', buffer.Parenthesis + 1);
        }

        private void CompileNodeType(ExpressionType nodeType, CompilerBuffer buffer)
        {
            buffer.Predicates += _formatter.FormatNode(nodeType);
        }

        #endregion

        #region Auxiliary Methods

        private QueryTranslator CreateQueryCompiler()
        {
            return new QueryTranslator(_formatter, _mapper, ++_rootTranslator._depth)
            {
                //
                // Bind the new compile to the root.
                // This binding is used to share parameters and such.
                _rootTranslator = _rootTranslator
            };
        }

        private static LambdaExpression ExtractLambda(Expression arg)
        {

            if ( arg is LambdaExpression )
            {
                var lambda = (LambdaExpression) arg;
                
                if (lambda.Body is BinaryExpression)
                {
                    return (LambdaExpression) arg;
                }

                //
                // In case of a MemberExpression Lambda return an explicit equality.
                //
                var parameters = lambda.Parameters;

                Expression body;

                if (lambda.Body is UnaryExpression)
                {
                    var unary = (UnaryExpression) lambda.Body;

                    body = Expression.NotEqual(unary.Operand, Expression.Constant(true));
                }
                else
                {
                    body = Expression.Equal(lambda.Body, Expression.Constant(true));
                }

                return Expression.Lambda(body, parameters);
            }

            if ( arg is UnaryExpression )
            {
                return ExtractLambda(((UnaryExpression)arg).Operand);
            }

            throw new TranslationException(Errors.Translation_UnableToExtractLambda);
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

        private static Expression ExtractAccessor(Expression expression)
        {
            var member = expression as MemberExpression;

            if (member != null)
            {
                var accessor = ExtractAccessor(member.Expression);
                if (accessor != null)
                {
                    expression = accessor;
                }
            }

            return expression;
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
                    var member = (MemberExpression)expression;
                    return ExtractConstant(member.Expression);
                case ExpressionType.Constant:
                    return expression;
                default: return null;
            }
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

        private string ExtractName(Expression expression, IEntityInfo entityInfo)
        {
            if ( expression is MemberExpression )
                return ((MemberExpression)expression).Member.Name;
            if ( expression is ParameterExpression )
                return entityInfo.KeyColumns.First().ColumnName;

            return null;
        }

        private static string Merge(CompilerBuffer buffer)
        {
            return new StringBuilder()
                .Append("Select ").Append(buffer.Projection)
                .Append(" From ").Append(buffer.From)
                .Append(buffer.Predicates)
                .Append(buffer.Joins)
                .ToString();
        }

        #endregion
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