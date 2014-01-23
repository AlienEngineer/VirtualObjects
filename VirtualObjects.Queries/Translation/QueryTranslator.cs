using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Fasterflect;
using VirtualObjects.Config;
using VirtualObjects.Queries.Annotations;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries.Translation
{
    class QueryTranslator : IQueryTranslator
    {
        class CompilerBuffer
        {
            public StringBuffer Projection { get; set; }
            public StringBuffer From { get; set; }
            public IEntityInfo EntityInfo { get; set; }
            public StringBuffer Predicates { get; set; }
            public int Parenthesis { get; set; }
            public int Take { get; set; }
            public int Skip { get; set; }
        }

        class ParameterEquality : IEqualityComparer<ParameterExpression>
        {

            public bool Equals(ParameterExpression x, ParameterExpression y)
            {
                return (x == null && y == null) || 
                    (x != null && y != null && x.Name == y.Name && x.Type == y.Type);
            }

            public int GetHashCode(ParameterExpression obj)
            {
                return obj.Name.GetHashCode() + obj.Type.GetHashCode();
            }
        }

        class QueryableStub : IQueryable
        {
            public QueryableStub(Type elementType, Expression expression)
            {
                Expression = expression;
                ElementType = elementType;
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public Expression Expression { get; private set; }
            public Type ElementType { get; private set; }
            public IQueryProvider Provider { get; [UsedImplicitly] private set; }
        }

        private readonly int _index;
        private readonly IFormatter _formatter;
        private readonly IMapper _mapper;
        private readonly IDictionary<String, Object> _parameters;
        private int _depth;
        private QueryTranslator _rootTranslator;
        private readonly IDictionary<ParameterExpression, QueryTranslator> _indexer;

        public QueryTranslator(IFormatter formatter, IMapper mapper)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = _depth = 0;
            _parameters = new Dictionary<String, Object>();
            _indexer = new Dictionary<ParameterExpression, QueryTranslator>(new ParameterEquality());
            _rootTranslator = this;
        }

        public QueryTranslator(IFormatter formatter, IMapper mapper, int index)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = index;
        }

        private IDictionary<string, object> Parameters { get { return _rootTranslator._parameters; } }

        public IDictionary<ParameterExpression, QueryTranslator> Indexer { get { return _rootTranslator._indexer; } }

        public IEntityInfo EntityInfo { get; set; }

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

            EntityInfo = buffer.EntityInfo;

            buffer.EntityInfo.RaiseIfNull(Errors.Query_EntityInfoNotFound, queryable);


            //
            // Compiles the ExpressionTree
            //
            CompileExpression(queryable.Expression, buffer);

            //
            // Starts by compiling a standard From [Source]
            //
            CompileFrom(buffer);

            //
            // Conditional Default Projection Compilation.
            //
            CompileDefaultProjection(buffer);

            return new QueryInfo
            {
                CommandText = Merge(buffer)
            };
        }

        private IQueryInfo TranslateQuery(Expression expression)
        {
            var queryable = ExtractQueryable(expression);

            return TranslateQuery(new QueryableStub(queryable.ElementType, expression));
        }

        private IQueryable ExtractQueryable(Expression expression)
        {
            var callExpression = expression as MethodCallExpression;
            if ( callExpression != null )
            {
                if ( !callExpression.Arguments.Any() )
                {
                    return new QueryableStub(callExpression.Method.ReturnType.GetGenericArguments().First(), null);
                }

                return ExtractQueryable(callExpression.Arguments.First());
            }

            var constant = ExtractConstant(expression) as ConstantExpression;
            if ( constant != null )
            {
                return constant.Value as IQueryable;
            }

            throw new TranslationException(Errors.Translation_UnableToExtractQueryable);
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
            if ( !expression.Arguments.Any() )
            {
                return;
            }

            CompileExpression(expression.Arguments.FirstOrDefault(), buffer);

            if ( expression.Arguments.Count == 1 )
            {
                return;
            }

            switch ( expression.Method.Name )
            {
                case "Select":
                    CompileCustomProjection(expression.Arguments[1], buffer);
                    break;
                case "Take":
                case "Skip":
                    CompileTakeSkip(expression, buffer);
                    break;
                case "GroupJoin":
                case "Join":
                    CompileJoin(expression, buffer);
                    break;
                case "Where":
                    if ( String.IsNullOrEmpty(buffer.Predicates) )
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

        private void CompileJoin(MethodCallExpression expression, CompilerBuffer buffer)
        {
            var arg1 = ExtractQueryable(expression.Arguments[0]);
            var arg2 = ExtractQueryable(expression.Arguments[1]);

            var entityInfo1 = buffer.EntityInfo = _mapper.Map(arg1.ElementType);
            var entityInfo2 = _mapper.Map(arg2.ElementType);

            var newTranlator = CreateNewTranslator();

            EntityInfo = entityInfo1;
            newTranlator.EntityInfo = entityInfo2;

            //
            // From [Source] Join [Other Source]
            //

            buffer.From += _formatter.FormatTableName(entityInfo1.EntityName, _index);
            buffer.From += " " + _formatter.InnerJoin + " ";
            buffer.From += _formatter.FormatTableName(entityInfo2.EntityName, newTranlator._index);
            buffer.From += " " + _formatter.On + " ";
            buffer.From += _formatter.BeginWrap();

            //
            // On Clause 
            //
            {
                CompileJoinOnPart(expression.Arguments[2], buffer, this);

                buffer.From += _formatter.FormatNode(ExpressionType.Equal);

                CompileJoinOnPart(expression.Arguments[3], buffer, newTranlator);
            }

            buffer.From += _formatter.EndWrap();

            //
            // Custom Projection
            //
            CompileCustomProjection(expression.Arguments[4], buffer);

        }

        private void CompileJoinOnPart(Expression expression, CompilerBuffer buffer, QueryTranslator translator)
        {
            buffer.From += CompileAndGetBuffer(() =>
            {
                
                var lambda = ExtractLambda(expression, false);
                Indexer[lambda.Parameters.First()] = translator;

                CompilePredicateExpression(lambda.Body, buffer);

            }, buffer);
        }


        private void CompileCustomProjection(Expression expression, CompilerBuffer buffer)
        {
            var lambda = ExtractLambda(expression, false);

            if ( lambda.Body is MemberExpression )
            {
                Indexer[lambda.Parameters.First()] = this;

                buffer.Projection = CompileAndGetBuffer(() => CompileMemberAccess(lambda.Body, buffer), buffer);
            }
            else
            {
                var newExpression = lambda.Body as NewExpression;
                if ( newExpression != null )
                {
                    buffer.Projection = CompileAndGetBuffer(() =>
                    {
                        foreach ( var arg in newExpression.Arguments )
                        {
                            //
                            // Use the parameter as a projection. 
                            // This indicates that we should use the default projection for that parameter type.
                            //
                            var parameterExpression = arg as ParameterExpression;
                            if (parameterExpression != null)
                            {
                                var translator = Indexer[parameterExpression];

                                buffer.Predicates += _formatter.FormatFields(translator.EntityInfo.Columns, translator._index);
                            }

                            buffer.Predicates += _formatter.FieldSeparator;
                        }

                        buffer.Predicates.RemoveLast(_formatter.FieldSeparator);

                    }, buffer);

                }
            }
        }

        private void CompileTakeSkip(MethodCallExpression expression, CompilerBuffer buffer)
        {
            switch ( expression.Method.Name )
            {
                case "Skip":
                    buffer.Skip = (int)ParseValue(expression.Arguments[1]);
                    break;
                case "Take":
                    buffer.Take = (int)ParseValue(expression.Arguments[1]);
                    break;
            }
        }

        private void CompileDefaultProjection(CompilerBuffer buffer)
        {
            if ( !String.IsNullOrEmpty(buffer.Projection) )
            {
                return;
            }

            if ( buffer.Take > 0 && buffer.Skip == 0 )
            {
                buffer.Projection += _formatter.FormatTakeN(buffer.Take);
                buffer.Projection += " ";
            }

            buffer.Projection += _formatter.FormatFields(buffer.EntityInfo.Columns, _index);

        }

        private void CompileFrom(CompilerBuffer buffer)
        {
            if ( buffer.Skip > 0 )
            {
                buffer.From += _formatter.BeginWrap();
                {
                    buffer.From += _formatter.Select + " ";
                    buffer.From += _formatter.FormatRowNumber(buffer.EntityInfo.KeyColumns, _index);
                    buffer.From += " " + _formatter.From + " ";
                    buffer.From += _formatter.FormatTableName(buffer.EntityInfo.EntityName, 100 + _index);

                    //
                    // Here the problem is the [T0] must be [T100].
                    if ( buffer.Predicates != null )
                    {
                        buffer.From += buffer.Predicates.Replace(_formatter.GetTableAlias(_index), _formatter.GetTableAlias(100 + _index));
                    }
                }
                buffer.From += _formatter.EndWrap() + " ";
                buffer.From += _formatter.GetTableAlias(_index);

                //
                // Append the conditions to the predicates.
                //
                if ( buffer.Predicates == null )
                {
                    buffer.Predicates += " " + _formatter.Where + " ";
                }
                else
                {
                    buffer.Predicates += " " + _formatter.And + " ";
                }

                buffer.Predicates += _formatter.BeginWrap();
                {
                    buffer.Predicates += _formatter.GetRowNumberField(_index) + _formatter.FormatNode(ExpressionType.GreaterThan) + buffer.Skip;

                    if ( buffer.Take > 0 )
                    {
                        buffer.Predicates += " " + _formatter.And + " ";
                        buffer.Predicates += _formatter.GetRowNumberField(_index) + _formatter.FormatNode(ExpressionType.LessThanOrEqual) + (buffer.Take + buffer.Skip);
                    }
                }
                buffer.Predicates += _formatter.EndWrap();

                return;
            }

            if ( buffer.From != null )
            {
                return;
            }

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

                case ExpressionType.Call:
                    CompileCallPredicate((MethodCallExpression)expression, buffer);
                    break;

                case ExpressionType.Convert:
                    CompilePredicateExpression(((UnaryExpression)expression).Operand, buffer);
                    break;

                case ExpressionType.Parameter:
                    CompileParameter((ParameterExpression)expression, buffer);
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

        private void CompileParameter(ParameterExpression expression, CompilerBuffer buffer)
        {
            var entityInfo = _mapper.Map(expression.Type);

            buffer.Predicates += _formatter.FormatFieldWithTable(entityInfo.KeyColumns.First().ColumnName, Indexer[expression]._index);
        }

        private void CompileCallPredicate(MethodCallExpression expression, CompilerBuffer buffer)
        {
            if ( expression.Method.Name != "Contains" )
            {
                return;
            }

            var newTranslator = CreateNewTranslator();

            var result = newTranslator.TranslateQuery(expression.Arguments.First());

            CompileMemberAccess(expression.Arguments[1], buffer);

            buffer.Predicates += " " + _formatter.In + " ";
            buffer.Predicates += _formatter.BeginWrap();
            buffer.Predicates += result.CommandText;

            //
            // To be closed later on.
            buffer.Parenthesis++;
        }

        private void CompileNot(Expression expression, CompilerBuffer buffer)
        {
            var unary = ((UnaryExpression)expression);

            CompilePredicateExpression(Expression.NotEqual(unary.Operand, Expression.Constant(true)), buffer);
        }

        private void CompileNew(Expression expression, CompilerBuffer buffer)
        {
            var newExpression = (NewExpression)expression;

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
            var parameterExpression = member.Expression as ParameterExpression;
            if ( parameterExpression != null )
            {
                var entityInfo = Indexer[parameterExpression].EntityInfo;

                buffer.Predicates += _formatter.FormatFieldWithTable(entityInfo[member.Member.Name].ColumnName, Indexer[parameterExpression]._index);
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
            buffer.Predicates += " " + _formatter.In + " ";
            buffer.Predicates += _formatter.BeginWrap();
            buffer.Predicates += _formatter.Select + " ";
            buffer.Parenthesis++;

            var queryCompiler = CreateNewTranslator();

            //
            // Use the key of the other type to predicate.
            //
            foreignKey = foreignKey.ForeignKey;
            buffer.Predicates += _formatter.FormatFieldWithTable(foreignKey.ColumnName, queryCompiler._index);
            buffer.Predicates += " " + _formatter.From + " ";
            buffer.Predicates += _formatter.FormatTableName(foreignKey.EntityInfo.EntityName, queryCompiler._index);
            buffer.Predicates += " " + _formatter.Where + " ";
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

            if ( accessor.Type != typeof(DateTime) )
            {
                return false;
            }


            switch ( expression.NodeType )
            {
                case ExpressionType.MemberAccess:
                    var member = (MemberExpression)expression;

                    switch ( member.Member.Name )
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
            var binary = expression as BinaryExpression;
            if ( binary == null )
            {
                var lambda = ExtractLambda(expression);

                Indexer[lambda.Parameters.First()] = this;

                var callExpression = lambda.Body as MethodCallExpression;

                if ( callExpression != null )
                {
                    buffer.Predicates += _formatter.BeginWrap();
                    {
                        CompileCallPredicate(callExpression, buffer);
                    }
                    buffer.Predicates += _formatter.EndWrap(buffer.Parenthesis + 1);
                    return;
                }

                binary = (BinaryExpression)lambda.Body;
            }


            buffer.Predicates += _formatter.BeginWrap();
            {
                CompilePredicateExpression(binary.Left, buffer);
                CompileNodeType(binary.NodeType, buffer);
                CompilePredicateExpression(binary.Right, buffer);
            }
            buffer.Predicates += _formatter.EndWrap(buffer.Parenthesis + 1);
        }

        private void CompileNodeType(ExpressionType nodeType, CompilerBuffer buffer)
        {
            buffer.Predicates += _formatter.FormatNode(nodeType);
        }

        #endregion

        #region Auxiliary Methods

        private QueryTranslator CreateNewTranslator()
        {
            return new QueryTranslator(_formatter, _mapper, ++_rootTranslator._depth)
            {
                //
                // Bind the new compile to the root.
                // This binding is used to share parameters and such.
                _rootTranslator = _rootTranslator
            };
        }

        private static LambdaExpression ExtractLambda(Expression arg, bool shouldCreateBinary = true)
        {
            if ( !(arg is LambdaExpression) )
            {
                var unaryExpression = arg as UnaryExpression;

                if ( unaryExpression != null )
                {
                    return ExtractLambda(unaryExpression.Operand, shouldCreateBinary);
                }

                throw new TranslationException(Errors.Translation_UnableToExtractLambda);
            }

            //
            // From this point forward we know that arg is a LambdaExpression.
            //

            var lambda = (LambdaExpression)arg;

            if ( !shouldCreateBinary || lambda.Body is BinaryExpression || lambda.Body is MethodCallExpression )
            {
                return (LambdaExpression)arg;
            }

            //
            // In case of a MemberExpression Lambda return an explicit equality.
            //
            var parameters = lambda.Parameters;

            var unary = lambda.Body as UnaryExpression;

            Expression body = unary != null ?
                Expression.NotEqual(unary.Operand, Expression.Constant(true)) : // This is a NOT
                Expression.Equal(lambda.Body, Expression.Constant(true));       // This is a Member Access

            return Expression.Lambda(body, parameters);
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

            if ( member != null )
            {
                var accessor = ExtractAccessor(member.Expression);
                if ( accessor != null )
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

        private string Merge(CompilerBuffer buffer)
        {
            return new StringBuilder()
                .Append(_formatter.Select + " ").Append(buffer.Projection)
                .Append(" " + _formatter.From + " ").Append(buffer.From)
                .Append(buffer.Predicates)
                .ToString();
        }

        private StringBuffer CompileAndGetBuffer(Action action, CompilerBuffer buffer)
        {
            SafePredicate(buffer);
            try
            {
                action();
                return buffer.Predicates;
            }
            finally
            {
                RestorePredicate(buffer);
            }
        }

        private String _predicates;

        private void SafePredicate(CompilerBuffer buffer)
        {
            _predicates = buffer.Predicates;
            buffer.Predicates = null;
        }

        private void RestorePredicate(CompilerBuffer buffer)
        {
            buffer.Predicates = _predicates;
        }

        public static bool IsDynamic(Type type)
        {
            return type.Name.StartsWith("<>");
        }

        #endregion
    }



}