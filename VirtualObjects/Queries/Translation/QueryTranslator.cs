using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Fasterflect;
using VirtualObjects.Config;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries.Annotations;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries.Translation
{
    class QueryTranslator : IQueryTranslator
    {
        #region Internal types

        public class CompilerBuffer
        {
            private StringBuffer _projection;

            public StringBuffer OldProjection { get; set; }

            public StringBuffer Projection
            {
                get { return _projection; }
                set
                {
                    OldProjection = _projection;
                    _projection = value;
                    if ( value == null )
                    {
                        PredicatedColumns.Clear();
                    }
                }
            }

            public StringBuffer From { get; set; }
            public StringBuffer Predicates { get; set; }
            public StringBuffer OrderBy { get; set; }
            public StringBuffer GroupBy { get; set; }
            public CompilerBuffer Union { get; set; }

            public IEntityInfo EntityInfo { get; set; }
            public IList<IEntityColumnInfo> PredicatedColumns { get; set; }

            public void AddPredicatedColumn(IEntityColumnInfo column)
            {
                if ( String.IsNullOrEmpty(Projection) )
                {
                    PredicatedColumns.Add(column);
                }
            }

            public void AddPredicatedColumns(IEnumerable<IEntityColumnInfo> columns)
            {
                if ( String.IsNullOrEmpty(Projection) )
                {
                    columns.ForEach(AddPredicatedColumn);
                }
            }

            public int Parenthesis { get; set; }
            public int Take { get; set; }
            public int Skip { get; set; }
            public bool Distinct { get; set; }
            public bool WasAggregated { get; set; }


            public void ActAsStub()
            {
                From = new StubBuffer();
                Predicates = new StubBuffer();
                OrderBy = new StubBuffer();
                Projection = new StubBuffer();
                GroupBy = new StubBuffer();
            }

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

        class QueryParameter : IOperationParameter
        {
            public Type Type { get; set; }
            public object Value { get; set; }
            public string Name { get; set; }
            public IEntityColumnInfo Column { get; set; }
        }

        #endregion

        #region Declaration Zone

        private readonly int _index;
        private readonly IFormatter _formatter;
        private readonly IMapper _mapper;
        private readonly IDictionary<String, IOperationParameter> _parameters;
        private int _depth;
        private int _parameterCount = -1;
        private QueryTranslator _rootTranslator;
        private readonly IDictionary<ParameterExpression, QueryTranslator> _indexer;
        private readonly Stack<String> _compileStack = new Stack<String>();
        private readonly Stack<IEntityColumnInfo> _memberAccessStack = new Stack<IEntityColumnInfo>();

        public QueryTranslator(IFormatter formatter, IMapper mapper)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = _depth = 0;
            _parameters = new Dictionary<String, IOperationParameter>();
            _indexer = new Dictionary<ParameterExpression, QueryTranslator>(new ParameterEquality());
            _rootTranslator = this;
        }

        public QueryTranslator(IFormatter formatter, IMapper mapper, int index)
        {
            _formatter = formatter;
            _mapper = mapper;
            _index = index;
        }

        private IDictionary<string, IOperationParameter> Parameters { get { return _rootTranslator._parameters; } }

        public IDictionary<ParameterExpression, QueryTranslator> Indexer { get { return _rootTranslator._indexer; } }

        public int ParameterCount { get { return _parameterCount; } }

        /// <summary>
        /// This means that we have all the parameters.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [should return]; otherwise, <c>false</c>.
        /// </value>
        public Boolean ShouldReturn { get { return ParameterCount > 0 && ParameterCount == Parameters.Count; } }

        public IEntityInfo EntityInfo { get; set; }

        private Type _outpuType;

        public Type OutputType
        {
            get
            {
                return _outpuType;
            }
            set
            {
                _outpuType = value;
            }
        }

        #endregion

        #region IQueryTranslator implementation.

        /// <summary>
        /// Translates the query.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IQueryInfo TranslateQuery(IQueryable queryable)
        {
            queryable = EvaluateQuery(queryable);

            var buffer = CreateBuffer(queryable);

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

            CompileDistinct(buffer);

            return new QueryInfo
            {
                CommandText = Merge(buffer),
                Parameters = Parameters,
                PredicatedColumns = buffer.PredicatedColumns,
                OutputType = OutputType ?? queryable.ElementType,
                Buffer = buffer
            };
        }

        public IQueryInfo TranslateParametersOnly(IQueryable queryable, int howMany)
        {
            _parameterCount = howMany;

            if ( howMany == 0 )
            {
                return new QueryInfo
                {
                    Parameters = Parameters
                };
            }

            queryable = EvaluateQuery(queryable);

            var buffer = CreateBuffer(queryable);

            //
            // Use a different type of buffer that doesn't append nothing.
            //
            buffer.ActAsStub();

            //
            // Compiles the ExpressionTree
            //
            CompileExpression(queryable.Expression, buffer, true);

            return new QueryInfo
            {
                Parameters = Parameters
            };
        }

        public IQueryInfo TranslateParametersOnly(Expression expression, int howMany)
        {
            _parameterCount = howMany;

            if ( howMany == 0 )
            {
                return new QueryInfo
                {
                    Parameters = Parameters
                };
            }

            var queryable = ExtractQueryable(expression);

            return TranslateParametersOnly(new QueryableStub(queryable.ElementType, expression), howMany);
        }

        public IQueryInfo TranslateQuery(Expression expression)
        {
            var queryable = ExtractQueryable(expression);

            if ( queryable == null )
            {
                throw new TranslationException("\nUnable to extract the query from expression.");
            }

            return TranslateQuery(new QueryableStub(queryable.ElementType, expression));
        }

        #endregion

        #region Compiling Methods

        private void CompileDistinct(CompilerBuffer buffer)
        {
            if ( buffer.Distinct )
            {
                buffer.Projection = _formatter.Distinct + " " + buffer.Projection;
            }
        }

        private void CompileExpression(Expression expression, CompilerBuffer buffer, bool parametersOnly = false)
        {
            if ( ShouldReturn )
            {
                return;
            }

            switch ( expression.NodeType )
            {
                case ExpressionType.Call:
                    CompileMethodCall((MethodCallExpression)expression, buffer, parametersOnly); break;

            }
        }

        private void CompileMethodCall(MethodCallExpression expression, CompilerBuffer buffer, bool parametersOnly)
        {
            if ( ShouldReturn )
            {
                return;
            }

            if ( !expression.Arguments.Any() )
            {
                return;
            }

            CompileExpression(expression.Arguments.FirstOrDefault(), buffer);

            _compileStack.Push(expression.Method.Name);

            if ( parametersOnly && expression.Arguments.Count > 1 && (
                expression.Method.Name == "Where" ||
                expression.Method.Name == "Count" ||
                expression.Method.Name == "Any" ||
                expression.Method.Name == "LongCount" ||
                expression.Method.Name == "LastOrDefault" ||
                expression.Method.Name == "Last" ||
                expression.Method.Name == "FirstOrDefault" ||
                expression.Method.Name == "First" ||
                expression.Method.Name == "SingleOrDefault" ||
                expression.Method.Name == "Single") )
            {
                CompileBinaryExpression(expression.Arguments[1], buffer, true);
                return;
            }

            if ( parametersOnly && expression.Method.Name == "Contains" )
            {
                CompileContains(expression, buffer);
                return;
            }

            if ( parametersOnly && expression.Method.Name == "Union" )
            {
                CompileUnion(expression.Arguments[1], buffer, true);
                return;
            }

            if ( parametersOnly )
            {
                return;
            }

            switch ( expression.Method.Name )
            {
                case "Union":
                    CompileUnion(expression.Arguments[1], buffer);
                    break;
                case "Select":
                    buffer.Projection = null;
                    CompileCustomProjection(expression.Arguments[1], buffer, expression);
                    break;
                case "Last":
                case "LastOrDefault":
                    CompileLastMethodCall(expression, buffer);
                    break;
                case "Take":
                case "Skip":
                case "First":
                case "FirstOrDefault":
                case "Single":
                case "SingleOrDefault":
                    CompileTakeSkip(expression, buffer);
                    break;
                case "GroupJoin":
                case "Join":
                    CompileJoin(expression, buffer);
                    break;
                case "Where":
                    InitBinaryExpressionCall(buffer);
                    CompileBinaryExpression(expression.Arguments[1], buffer);
                    break;
                case "Distinct":
                    buffer.Distinct = true;
                    break;
                case "ThenBy":
                case "OrderBy":
                    CompileOrderBy(expression, buffer);
                    break;
                case "OrderByDescending":
                    CompileOrderByDescending(expression, buffer);
                    break;
                case "GroupBy":
                    CompileGroupBy(expression, buffer);
                    break;
                case "Contains":
                    CompileContains(expression, buffer);
                    break;
                case "Any":
                case "LongCount":
                case "Count":
                    CompileCountOrAnyCall(expression, buffer);
                    break;
                case "Sum":
                    CompileMethod(expression.Arguments[1], _formatter.Sum, buffer);
                    break;
                case "Average":
                    CompileMethod(expression.Arguments[1], _formatter.Avg, buffer);
                    break;
                case "Min":
                    CompileMinMaxMethod(expression, _formatter.Min, buffer);
                    break;
                case "Max":
                    CompileMinMaxMethod(expression, _formatter.Max, buffer);
                    break;
                default:
                    throw new TranslationException(Errors.Translation_MethodNotSupported, expression);
            }

            _compileStack.Pop();
        }

        private void CompileUnion(Expression expression, CompilerBuffer buffer, bool parametersOnly = false)
        {
            var translator = CreateNewTranslator();

            if ( parametersOnly )
            {
                translator.TranslateParametersOnly(expression, _parameterCount);
                return;
            }

            buffer = GetLastUnionBuffer(buffer);

            buffer.Union = ((QueryInfo)translator.TranslateQuery(expression)).Buffer;
        }

        private CompilerBuffer GetLastUnionBuffer(CompilerBuffer buffer)
        {
            if (buffer.Union == null)
            {
                return buffer;
            }

            return GetLastUnionBuffer(buffer.Union);
        }

        private void CompileLastMethodCall(MethodCallExpression expression, CompilerBuffer buffer)
        {
            var firstMethod = typeof(Queryable)
                .Methods(Flags.Static | Flags.StaticPublic, "First").First(e => e.Parameters().Count == expression.Arguments.Count)
                .MakeGenericMethod(EntityInfo.EntityType);

#if NET35
            CompileMethodCall(Expression.Call(firstMethod, expression.Arguments.ToArray()), buffer, false);
#else
            CompileMethodCall(Expression.Call(firstMethod, expression.Arguments), buffer, false);
#endif

            foreach ( var column in EntityInfo.KeyColumns )
            {
                var orderByDesc = typeof(Queryable)
                .Methods(Flags.Static | Flags.StaticPublic, "OrderByDescending").First(e => e.Parameters().Count == 2)
                .MakeGenericMethod(EntityInfo.EntityType, column.Property.PropertyType);

                var parameter = Expression.Parameter(EntityInfo.EntityType, "e");

                var args = new[]
                {
                    expression.Arguments.First(),
                    Expression.Lambda(Expression.MakeMemberAccess(parameter, column.Property), parameter)
                };

                var orderByCall = Expression.Call(orderByDesc, args);

                CompileMethodCall(orderByCall, buffer, false);
            }

        }

        private void CompileGroupBy(MethodCallExpression expression, CompilerBuffer buffer)
        {
            buffer.GroupBy = CompileOrderOrGroupBy(expression.Arguments[1], buffer, buffer.GroupBy, _formatter.GroupBy);
        }

        private void CompileContains(MethodCallExpression expression, CompilerBuffer buffer)
        {
            // Strategy
            // Create a new MethodCallExpression to Any with the proper predicate.

            var method = typeof(Queryable)
                .Methods(Flags.Static | Flags.StaticPublic, "Any")
                .First(e => e.Parameters().Count == 2)
                .MakeGenericMethod(EntityInfo.EntityType);


            var parameter = Expression.Parameter(EntityInfo.EntityType, "e");

            var lambda = Expression.Lambda(
                Expression.MakeBinary(
                    ExpressionType.Equal,
                    parameter,
                    expression.Arguments[1]),
                parameter);


            var call = Expression.Call(method, expression.Arguments.First(), lambda);
            CompileCountOrAnyCall(call, buffer);
        }

        private void CompileMinMaxMethod(MethodCallExpression callExpression, string method, CompilerBuffer buffer)
        {
            //
            // If there are arguments on min or max use it.
            //
            if ( callExpression.Arguments.Count > 1 )
            {
                
                CompileMethod(callExpression.Arguments[1], method, buffer);
                return;
            }

            throw new TranslationException(Errors.Translation_Method_NoArgs_NotSupported, callExpression.Method);

            //
            // Create the proper binary clause to filter the query by min or max.
            // the first key is used by default for this.
            //

            //var parameter = Expression.Parameter(EntityInfo.EntityType, "e");

            //var member = Expression.MakeMemberAccess(parameter, EntityInfo.KeyColumns.First().Property);

            //Indexer[parameter] = this;

            //InitBinaryExpressionCall(buffer);
            //buffer.Predicates += _formatter.BeginWrap();
            //{
            //    buffer.Predicates += method;
            //    buffer.Predicates += _formatter.BeginWrap();
            //    {
            //        CompileMemberAccess(member, buffer);    
            //    }
            //    buffer.Predicates += _formatter.EndWrap();

            //    buffer.Predicates += " ";
            //    buffer.Predicates += _formatter.FormatNode(ExpressionType.Equal);
            //    buffer.Predicates += " ";
            //    CompileMemberAccess(member, buffer);
            //}

            //buffer.Predicates += _formatter.EndWrap();
        }

        private void CompileMethod(Expression expression, String functionName, CompilerBuffer buffer)
        {
            buffer.WasAggregated = true;
            var lambda = ExtractLambda(expression, false);
            Indexer[lambda.Parameters.First()] = this;

            buffer.Projection = CompileAndGetBuffer(() =>
            {
                buffer.Predicates += functionName;
                buffer.Predicates += _formatter.BeginWrap();
                CompileMemberAccess(lambda.Body, buffer);
                buffer.Predicates += _formatter.EndWrap();
            }, buffer);
        }

        private void CompileCountOrAnyCall(MethodCallExpression callExpression, CompilerBuffer buffer)
        {
            if ( callExpression.Arguments.Count > 1 )
            {
                //
                // Appends Where or And before the next predicate.
                //
                InitBinaryExpressionCall(buffer);
                CompileBinaryExpression(callExpression.Arguments[1], buffer);
            }

            buffer.WasAggregated = true;

            switch ( callExpression.Method.Name )
            {
                case "LongCount":
                case "Count":
                    buffer.Projection = _formatter.Count;
                    break;
                case "Any":
                    buffer.Projection = _formatter.Any;
                    break;
            }

        }

        private void CompileOrderByDescending(MethodCallExpression expression, CompilerBuffer buffer)
        {
            CompileOrderBy(expression, buffer);
            buffer.OrderBy += " " + _formatter.Descending;
        }

        private void CompileOrderBy(MethodCallExpression expression, CompilerBuffer buffer)
        {
            buffer.OrderBy = CompileOrderOrGroupBy(expression.Arguments[1], buffer, buffer.OrderBy, _formatter.OrderBy);
        }

        private StringBuffer CompileOrderOrGroupBy(Expression expression, CompilerBuffer buffer, StringBuffer stringBuffer, string starter)
        {
            if ( String.IsNullOrEmpty(stringBuffer) )
            {
                stringBuffer += " " + starter + " ";
            }
            else
            {
                stringBuffer += _formatter.FieldSeparator;
            }

            var lambda = ExtractLambda(expression, false);
            Indexer[lambda.Parameters.First()] = this;

            stringBuffer += CompileAndGetBuffer(() => CompileMemberAccess(lambda.Body, buffer), buffer);

            return stringBuffer;
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

            // The first table will only be added in the first call.
            if ( String.IsNullOrEmpty(buffer.From) )
            {
                buffer.From += _formatter.FormatTableName(entityInfo1.EntityName, _index);
            }

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


            buffer.Projection = null;
            CompileCustomProjection(expression.Arguments[4], buffer, expression);
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

        private void CompileCustomProjection(Expression expression, CompilerBuffer buffer, MethodCallExpression callExpression)
        {
            var lambda = ExtractLambda(expression, false);
            Indexer[lambda.Parameters.First()] = this;

            if ( lambda.Body is MemberExpression )
            {
#if NET35
                OutputType = lambda.Body.Type;
#else
                OutputType = lambda.ReturnType;
#endif

                buffer.Projection = CompileAndGetBuffer(() => CompileMemberAccess(lambda.Body, buffer), buffer);
            }
            else
            {
                var newExpression = lambda.Body as NewExpression;
                if ( newExpression != null )
                {

                    if ( !String.IsNullOrEmpty(buffer.Projection) )
                    {
                        return;
                    }

                    OutputType = newExpression.Type;

                    buffer.Projection = CompileAndGetBuffer(() =>
                    {
                        int memberIndex = 0;
                        foreach ( var arg in newExpression.Arguments )
                        {
                            var member = newExpression.Members[memberIndex++];
                            var tmpExp = RemoveDynamicFromMemberAccess(arg);

                            CompileCustomProjectionArgument(buffer, callExpression, tmpExp, member);

                        }

                        buffer.Predicates.RemoveLast(_formatter.FieldSeparator);
                    }, buffer);

                }
            }
        }

        private void CompileCustomProjectionArgument(CompilerBuffer buffer, MethodCallExpression callExpression, Expression tmpExp, MemberInfo member, bool finalize = true)
        {

            if ( CompileCustomProjectionParameter(buffer, callExpression, tmpExp) ||
                   CompileCustomProjectionMemberAccess(buffer, tmpExp, member) ||
                   CompileCustomProjectionMethodCall(buffer, tmpExp, member, finalize) ||
                   CompileCustomProjectionBinary(buffer, callExpression, tmpExp, member, finalize) ||
                   CompileCustomProjectionConstant(buffer, tmpExp) ||
                   CompileCustomProjectionConvert(buffer, callExpression, tmpExp, member, finalize) )
            {

                if ( finalize )
                {
                    buffer.Predicates += _formatter.FieldSeparator;
                }
            }
            else
            {
                throw new TranslationException("\nThe translation of {NodeType} is not yet supported on projections.", tmpExp);
            }
        }

        private bool CompileCustomProjectionConvert(CompilerBuffer buffer, MethodCallExpression callExpression, Expression tmpExp, MemberInfo member, bool finalize)
        {
            var unaryExpression = tmpExp as UnaryExpression;

            if ( unaryExpression == null )
            {
                return false;
            }

            CompileCustomProjectionArgument(buffer, callExpression, unaryExpression.Operand, member, finalize);

            return true;
        }

        private bool CompileCustomProjectionConstant(CompilerBuffer buffer, Expression tmpExp)
        {
            var constant = tmpExp as ConstantExpression;

            if ( constant == null )
            {
                return false;
            }

            buffer.Predicates += _formatter.FormatConstant(ParseValue(tmpExp));
            return true;
        }

        private bool CompileCustomProjectionBinary(CompilerBuffer buffer, MethodCallExpression callExpression, Expression tmpExp, MemberInfo member, bool finalize = true)
        {
            var binary = tmpExp as BinaryExpression;

            if ( binary == null )
            {
                return false;
            }

            buffer.Predicates += _formatter.BeginWrap();
            {
                CompileCustomProjectionArgument(buffer, callExpression, binary.Left, member, false);
                buffer.Predicates += " " + _formatter.FormatNode(binary.NodeType) + " ";
                CompileCustomProjectionArgument(buffer, callExpression, binary.Right, member, false);
            }
            buffer.Predicates += _formatter.EndWrap();

            if ( finalize )
            {
                buffer.Predicates += " " + _formatter.FormatField(member.Name);
            }

            return true;
        }

        private Boolean CompileCustomProjectionMethodCall(CompilerBuffer buffer, Expression tmpExp, MemberInfo member, bool finalize = true)
        {
            var call = tmpExp as MethodCallExpression;

            if ( call == null )
            {
                return false;
            }

            ThrowIfContainsAPredicate(call);

            //
            // The CompileMethodCall for a sum or other aggregate function
            // will save the predicates on stack. So we need to forget this save.
            //
            if ( _predicates.Count > 0 )
            {
                _predicates.Pop();
            }

            CompileMethodCall(call, buffer, false);

            buffer.Predicates += buffer.Projection;

            if ( finalize )
            {
                buffer.Predicates += " " + _formatter.FormatField(member.Name);
            }

            return true;
        }

        private Boolean CompileCustomProjectionMemberAccess(CompilerBuffer buffer, Expression tmpExp, MemberInfo member)
        {
            var memberExpression = tmpExp as MemberExpression;

            if ( memberExpression == null )
            {
                return false;
            }

            CompileMemberAccess(memberExpression, buffer, member);
            return true;
        }

        private Boolean CompileCustomProjectionParameter(CompilerBuffer buffer, MethodCallExpression callExpression, Expression tmpExp)
        {
            var parameterExpression = tmpExp as ParameterExpression;

            if ( parameterExpression == null )
            {
                return false;
            }

            QueryTranslator translator = null;

            //
            // Handle collection projections on Joins.
            //
            if ( parameterExpression.Type.InheritsOrImplements(typeof(IEnumerable)) && callExpression.Arguments.Count > 3 )
            {
                var genericType = parameterExpression.Type.GetGenericArguments().First();

                var entry = Indexer.Reverse().FirstOrDefault(e => e.Key.Type == genericType);

                translator = Indexer[entry.Key];

                //
                // After finding the proper translator bind the current parameter to the 
                // found translator.
                //
                Indexer[parameterExpression] = translator;
            }

            translator = translator ?? Indexer[parameterExpression];

            buffer.Predicates += _formatter.FormatFields(translator.EntityInfo.Columns, translator._index);

            buffer.AddPredicatedColumns(translator.EntityInfo.Columns);

            //
            // If the whole entity is used we need to ungroup.
            //
            if ( !String.IsNullOrEmpty(buffer.GroupBy) )
            {
                throw new TranslationException(
                    "\nIts not possible to have a detailed entity and GroupBy clause.\nTo achieve this use .ToList() before the GroupBy methodCall.");
            }

            return true;
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
                case "FirstOrDefault":
                case "SingleOrDefault":
                case "First":
                case "Single":
                    if ( expression.Arguments.Count > 1 )
                    {
                        InitBinaryExpressionCall(buffer);
                        CompileBinaryExpression(expression.Arguments[1], buffer);
                    }
                    buffer.Take = 1;
                    break;
            }
        }

        private void CompileDefaultProjection(CompilerBuffer buffer)
        {
            if ( !String.IsNullOrEmpty(buffer.Projection) )
            {
                return;
            }

            if ( buffer.Take > 0 && buffer.Skip <= 0 )
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

                    if ( !String.IsNullOrEmpty(buffer.OrderBy) )
                    {
                        buffer.From += _formatter.FormatRowNumber(
                            buffer.OrderBy.Replace(_formatter.GetTableAlias(_index), _formatter.GetTableAlias(100 + _index)),
                            _index);
                        buffer.OrderBy = null;
                    }
                    else
                    {
                        //
                        // Default order by clause.
                        //
                        buffer.From += _formatter.FormatRowNumber(buffer.EntityInfo.KeyColumns, _index);
                    }


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
                if ( String.IsNullOrEmpty(buffer.Predicates) )
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

            if ( OutputType == null )
            {
                OutputType = buffer.EntityInfo.EntityType;
            }

            buffer.From = _formatter.FormatTableName(buffer.EntityInfo.EntityName, _index);
        }

        private void CompilePredicateExpression(Expression expression, CompilerBuffer buffer)
        {
            if ( ShouldReturn )
            {
                return;
            }

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
                    var callExpression = (MethodCallExpression)expression;

                    if ( callExpression.Object != null && IsConstant(expression) )
                    {
                        CompileConstant(Expression.Constant(Expression.Lambda(callExpression).Compile().DynamicInvoke()), buffer);
                        return;
                    }

                    if ( callExpression.Object != null && IsMemberAccess(callExpression) )
                    {
                        CompileMemberCallPredicate(callExpression, buffer);
                        return;
                    }

                    CompileCallPredicate(callExpression, buffer);
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

        private void CompileMemberCallPredicate(MethodCallExpression callExpression, CompilerBuffer buffer)
        {
            CompilePredicateExpression(callExpression.Object, buffer);

            buffer.Predicates += _formatter.BeginMethodCall(callExpression.Method.Name);

            CompilePredicateExpression(callExpression.Arguments.First(), buffer);

            buffer.Predicates += _formatter.EndMethodCall(callExpression.Method.Name);
        }

        private void CompileParameter(ParameterExpression expression, CompilerBuffer buffer)
        {
            var entityInfo = _mapper.Map(expression.Type);

            var column = entityInfo.KeyColumns.First();
            _memberAccessStack.Push(column);

            buffer.Predicates += _formatter.FormatFieldWithTable(column.ColumnName, Indexer[expression]._index);
        }

        private void CompileCallPredicate(MethodCallExpression expression, CompilerBuffer buffer)
        {
            if ( expression.Method.Name != "Contains" )
            {
                throw new TranslationException("\nMethod [{Name}] not yet supported on nested query.", expression.Method);
            }

            var newTranslator = CreateNewTranslator();
            IQueryInfo result;

            try
            {
                Expression nestedExpression = expression.Arguments.First();
                nestedExpression = BuildMissingProjection(nestedExpression, expression.Arguments[1] as MemberExpression);
                var queryable = ExtractQueryable(nestedExpression);

                nestedExpression = JoinExpressions(nestedExpression, queryable.Expression);

                result = newTranslator.TranslateQuery(nestedExpression);
            }
            catch ( TranslationException ex )
            {
                throw new TranslationException("\nUnable to compile nested query.", ex);
            }

            CompileMemberAccess(expression.Arguments[1], buffer);

            buffer.Predicates += " " + _formatter.In + " ";
            buffer.Predicates += _formatter.BeginWrap();
            buffer.Predicates += result.CommandText;

            //
            // To be closed later on.
            buffer.Parenthesis++;
        }

        private Expression BuildMissingProjection(Expression nestedExpression, MemberExpression arg1)
        {
            var callExpression = nestedExpression as MethodCallExpression;

            if ( arg1 == null || callExpression == null || callExpression.Method.Name == "Select" )
            {
                return nestedExpression;
            }

            var foreignKey = EntityInfo[arg1.Member.Name];
            var entityType = foreignKey.ForeignKey.EntityInfo.EntityType;

            var method = typeof(Queryable)
                .Methods(Flags.Static | Flags.StaticPublic, "Select")
                .First(e => e.Parameters().Count == 2)
                .MakeGenericMethod(entityType, foreignKey.ForeignKey.Property.PropertyType);

            var parameter = Expression.Parameter(entityType, "e");

            return Expression.Call(method, nestedExpression,
                        Expression.Lambda(
                            Expression.MakeMemberAccess(
                                parameter,
                                foreignKey.ForeignKey.Property
                            ) /* Make Member Access */ ,
                            parameter
                        ) /* Make Lambda for member */
                  );
        }

        /// <summary>
        /// Joins the expressions when a query is passed via context capture.
        /// </summary>
        /// <param name="nestedExpression">The nested expression.</param>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        private Expression JoinExpressions(Expression nestedExpression, Expression expression)
        {
            if ( expression == null )
            {
                return nestedExpression;
            }

            var callExpression = nestedExpression as MethodCallExpression;

            if ( callExpression == null )
            {
                return null;
            }

            var exp = JoinExpressions(callExpression.Arguments.First(), expression);

            if ( exp == null )
            {
                return Expression.Call(callExpression.Method, expression, callExpression.Arguments[1]);
            }

            return exp;
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
            if ( !IsConstant(expression) )
            {
                throw new UnsupportedException(Errors.Internal_WrongMethodCall, expression);
            }

            var value = ParseValue(expression);

            if ( value != null && !value.GetType().IsFrameworkType() )
            {
                var member= _memberAccessStack.Peek();
                member = member.GetLastBind();
                value = member.GetFieldFinalValue(value);
            }

            var formatted = _formatter.FormatConstant(value, Parameters.Count);

            Parameters[formatted] = new QueryParameter
            {
                Value = value
            };

            buffer.Predicates += formatted;
        }

        private void CompileMemberAccess(Expression expression, CompilerBuffer buffer, MemberInfo memberInfo = null)
        {
            var member = expression as MemberExpression;

            if ( member == null )
            {
                throw new UnsupportedException(Errors.Internal_WrongMethodCall, expression);
            }

            if ( _compileStack.Peek() == "Select" && HasManyMemberAccess(expression) )
            {
                throw new TranslationException("\nMore than one member accessed in a projection.\nExpression: {Expression}", new { Expression = expression.ToString() });
            }

            if ( CompileIfConstant(expression, buffer) ) return;

            if ( CompileIfDatetime(expression, buffer) ) return;


            //
            // Indicates that the accessor is a Dynamic type.
            // This will happend when using join queries when the projection 
            // is a dynamic type and a Where clause is added using the dynamic type.
            //
            while ( ExtractAccessor(member).Type.IsDynamic() )
            {
                var parsedMember = RemoveDynamicType(member) as MemberExpression;

                member = parsedMember ?? member;

                if ( parsedMember == null ) break;
            }

            if ( memberInfo == null )
            {
                memberInfo = member.Member;
            }

            //
            // If the member is from the current entity.
            //
            var parameterExpression = member.Expression as ParameterExpression;
            if ( parameterExpression != null )
            {
                var entityInfo = Indexer[parameterExpression].EntityInfo;

                var column = entityInfo[memberInfo.Name] ?? entityInfo[member.Member.Name];

                _memberAccessStack.Push(column);
                buffer.Predicates += _formatter.FormatFieldWithTable(column.ColumnName, Indexer[parameterExpression]._index);

                buffer.AddPredicatedColumn(column);
            }
            else
            {
                CompileMemberAccess(member, member.Expression, buffer);
            }
        }

        private QueryTranslator CompileMemberAccess(MemberExpression expression, Expression nextExpression, CompilerBuffer buffer)
        {
            var parameterExpression = nextExpression as ParameterExpression;

            if ( parameterExpression != null )
            {
                return Indexer[parameterExpression];
            }

            var nextMember = nextExpression as MemberExpression;

            Debug.Assert(nextMember != null, "CompileMemberAccess : nextMember != null");

            var translator = CompileMemberAccess(nextMember, nextMember.Expression, buffer);

            var foreignKey = translator.EntityInfo[nextMember.Member.Name];

            QueryTranslator queryTranslator;

            //
            // Handle DateTime Member Access
            // Handle String MemberAccess
            //
            if ( CompileDateTimeMemberAccess(expression, buffer, nextMember, foreignKey, translator, out queryTranslator) ||
                CompileStringMemberAccess(expression, buffer, nextMember, foreignKey, translator, out queryTranslator) )
            {
                return queryTranslator;
            }

            //
            // Use the binded field on the predicate.
            //
            _memberAccessStack.Push(foreignKey);
            buffer.Predicates += _formatter.FormatFieldWithTable(foreignKey.ColumnName, translator._index);
            buffer.Predicates += " " + _formatter.In + " ";
            buffer.Predicates += _formatter.BeginWrap();
            buffer.Predicates += _formatter.Select + " ";
            buffer.Parenthesis++;

            var queryCompiler = CreateNewTranslator();

            //
            // Use the key of the other type to predicate.
            //
            foreignKey = foreignKey.ForeignKey;
            _memberAccessStack.Push(foreignKey);
            buffer.Predicates += _formatter.FormatFieldWithTable(foreignKey.ColumnName, queryCompiler._index);
            buffer.Predicates += " " + _formatter.From + " ";
            buffer.Predicates += _formatter.FormatTableName(foreignKey.EntityInfo.EntityName, queryCompiler._index);
            buffer.Predicates += " " + _formatter.Where + " ";
            buffer.Predicates += _formatter.FormatFieldWithTable(foreignKey.EntityInfo[expression.Member.Name].ColumnName, queryCompiler._index);

            return translator;
        }

        private bool CompileStringMemberAccess(MemberExpression expression, CompilerBuffer buffer, MemberExpression nextMember,
            IEntityColumnInfo foreignKey, QueryTranslator translator, out QueryTranslator queryTranslator)
        {
            if ( nextMember.Member.Type() == typeof(String) )
            {
                switch ( expression.Member.Name )
                {
                    case "Length":
                        buffer.Predicates += _formatter.FormatLengthWith(foreignKey.ColumnName, translator._index);
                        break;
                    default:
                        throw new TranslationException(Errors.Translation_String_MemberAccess_NotSupported, expression.Member);
                }

                {
                    queryTranslator = translator;
                    return true;
                }
            }
            queryTranslator = null;
            return false;
        }

        private bool CompileDateTimeMemberAccess(MemberExpression expression, CompilerBuffer buffer, MemberExpression nextMember,
                                                 IEntityColumnInfo foreignKey, QueryTranslator translator,
                                                 out QueryTranslator queryTranslator)
        {
            if ( nextMember.Member.Type() == typeof(DateTime) )
            {
                switch ( expression.Member.Name )
                {
                    case "Year":
                        buffer.Predicates += _formatter.FormatYearOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "Month":
                        buffer.Predicates += _formatter.FormatMonthOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "Day":
                        buffer.Predicates += _formatter.FormatDayOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "Hour":
                        buffer.Predicates += _formatter.FormatHourOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "Minute":
                        buffer.Predicates += _formatter.FormatMinuteOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "Second":
                        buffer.Predicates += _formatter.FormatSecondOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "Millisecond":
                        buffer.Predicates += _formatter.FormatMillisecondOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "DayOfWeek":
                        buffer.Predicates += _formatter.FormatDayOfWeekOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "DayOfYear":
                        buffer.Predicates += _formatter.FormatDayOfYearOf(foreignKey.ColumnName, translator._index);
                        break;
                    case "Date":
                        buffer.Predicates += _formatter.FormatDateOf(foreignKey.ColumnName, translator._index);
                        break;
                    default:
                        throw new TranslationException(Errors.Translation_Datetime_Member_NotSupported, expression.Member);
                }

                {
                    queryTranslator = translator;
                    return true;
                }
            }

            queryTranslator = null;

            return false;
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
                        case "Year":
                        case "Day":
                        case "Hour":
                        case "Minute":
                        case "Second":
                        case "Millisecond":
                        case "Month":
                        case "DayOfWeek":
                        case "DayOfYear":
                        case "Date":

                            buffer.Predicates += _formatter.BeginMethodCall(member.Member.Name);
                            buffer.Predicates += _formatter.FormatGetDate();
                            buffer.Predicates += _formatter.EndMethodCall(member.Member.Name);

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
            if ( !IsConstant(expression) )
            {
                return false;
            }

            CompileConstant(expression, buffer);
            return true;
        }

        private void CompileBinaryExpression(Expression expression, CompilerBuffer buffer, bool parametersOnly = false)
        {
            if ( ShouldReturn )
            {
                return;
            }

            var methodCalled = String.Empty;

            var binary = expression as BinaryExpression;
            if ( binary == null )
            {
                var lambda = ExtractLambda(expression);

                Indexer[lambda.Parameters.First()] = this;

                var callExpression = lambda.Body as MethodCallExpression;

                //
                // Predicate is call to another function. Like a nested query with contains.
                //
                if ( callExpression != null && !(callExpression.Object is MemberExpression) )
                {
                    if ( parametersOnly )
                    {
                        return;
                    }

                    buffer.Predicates += _formatter.BeginWrap();
                    {
                        CompileCallPredicate(callExpression, buffer);
                    }

                    FlushParenthesis(buffer);
                    return;
                }

                //
                // extract the method call.
                // remaks the binary expression.
                //
                if ( callExpression != null )
                {
                    //
                    // To be checked below.
                    methodCalled = callExpression.Method.Name;

                    //
                    // Remake the Binary expresion using the callExpression Object
                    // that should hold the member access without the method call.
                    // use the argument as right side.
                    //
                    binary = Expression.MakeBinary(
                        ExpressionType.Equal,
                        callExpression.Object,
                        callExpression.Arguments.First());
                }
                else
                {
                    binary = (BinaryExpression)lambda.Body;
                }

            }


            buffer.Predicates += _formatter.BeginWrap();
            {
                var left = RemoveDynamicFromMemberAccess(binary.Left);
                var right = RemoveDynamicFromMemberAccess(binary.Right);

                //
                // In case the constant part is in the left part.
                // Not very used but still...
                // e => 1 == e.EmployeeId
                //
                if ( IsConstant(left) )
                {
                    left = binary.Right;
                    right = binary.Left;
                }
                else if ( !HasManyMemberAccess(left) && HasManyMemberAccess(right) )
                {
                    //
                    // To keep some code consistency switch the side that has many members to the right side.
                    //
                    left = binary.Right;
                    right = binary.Left;
                }
                else if ( HasManyMemberAccess(left) && HasManyMemberAccess(right) )
                {
                    throw new TranslationException(Errors.Translation_ManyMembersAccess_On_BothSides_NotSupported);
                }
                else if ( left is ParameterExpression && IsConstant(right) && right.Type == left.Type )
                {
                    CompileParameterToObject(right, buffer, parametersOnly);
                    return;
                }


                CompilePredicateExpression(left, buffer);

                if ( IsConstant(right) && right.ToString() == "null" )
                {
                    //
                    // Compilation for a null equals.
                    //
                    switch ( binary.NodeType )
                    {
                        case ExpressionType.Equal:
                            buffer.Predicates += " " + _formatter.IsNull; break;
                        case ExpressionType.NotEqual:
                            buffer.Predicates += " " + _formatter.IsNotNull; break;
                        default: // This will happen only if jesus really wants.
                            throw new TranslationException(Errors.SQL_UnableToFormatNode, binary);
                    }
                }
                else
                {
                    if ( methodCalled != String.Empty )
                    {
                        //
                        // Compiles method calls.
                        //
                        buffer.Predicates += _formatter.BeginMethodCall(methodCalled);

                        CompilePredicateExpression(right, buffer);

                        buffer.Predicates += _formatter.EndMethodCall(methodCalled);
                    }
                    else
                    {
                        CompileNodeType(binary.NodeType, buffer);
                        CompilePredicateExpression(right, buffer);
                    }
                }

            }
            FlushParenthesis(buffer);
        }

        private void CompileParameterToObject(Expression right, CompilerBuffer buffer, bool parametersOnly)
        {
            var value = ParseValue(right);

            foreach ( var keyColumn in EntityInfo.KeyColumns )
            {
                var fieldFinalValue = keyColumn.GetFieldFinalValue(value);
                if ( parametersOnly )
                {
                    CompileConstant(Expression.Constant(fieldFinalValue), buffer);
                    continue;
                }

                _memberAccessStack.Push(keyColumn);
                buffer.Predicates += _formatter.FormatFieldWithTable(keyColumn.ColumnName, _rootTranslator._index);
                buffer.Predicates += " " + _formatter.FormatNode(ExpressionType.Equal) + " ";

                CompileConstant(Expression.Constant(fieldFinalValue), buffer);

                buffer.Predicates += " " + _formatter.And + " ";
            }

            if ( parametersOnly )
            {
                return;
            }

            buffer.Predicates.RemoveLast(_formatter.And.Length + 2);

            FlushParenthesis(buffer);
        }

        private void CompileNodeType(ExpressionType nodeType, CompilerBuffer buffer)
        {
            buffer.Predicates += _formatter.FormatNode(nodeType);
        }

        #endregion

        #region Auxiliary Methods

        private static void InitBinaryExpressionCall(CompilerBuffer buffer)
        {
            if ( String.IsNullOrEmpty(buffer.Predicates) )
            {
                buffer.Predicates += " Where ";
            }
            else
            {
                buffer.Predicates += " And ";
            }
        }

        private static void ThrowIfContainsAPredicate(MethodCallExpression call)
        {
            if ( call.Arguments.Count == 2 )
            {
                var tmpLambda = ExtractLambda(call.Arguments[1], false);
#if NET35
                var outputType = tmpLambda.Body.Type;
#else
                var outputType = tmpLambda.ReturnType;
#endif
                if ( outputType == typeof(Boolean) )
                {
                    throw new TranslationException(Errors.Translation_PredicateOnProjection);
                }
            }
        }

        private void FlushParenthesis(CompilerBuffer buffer)
        {
            buffer.Predicates += _formatter.EndWrap(buffer.Parenthesis + 1);
            buffer.Parenthesis = 0;
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
                return ParseValue(constant) as IQueryable;
            }

            throw new TranslationException(Errors.Translation_UnableToExtractQueryable);
        }

        private Expression RemoveDynamicFromMemberAccess(Expression tmpExp)
        {
            if ( IsConstant(tmpExp) )
            {
                return tmpExp;
            }

            while ( tmpExp is MemberExpression && ExtractAccessor(tmpExp).Type.IsDynamic() )
            {
                tmpExp = RemoveDynamicType(tmpExp as MemberExpression);
            }

            return tmpExp;
        }

        private Expression RemoveDynamicType(MemberExpression member)
        {
            if ( IsConstant(member) )
            {
                return member;
            }

            if ( member.Expression is ParameterExpression )
            {
                return Expression.Parameter(member.Type, member.Member.Name);
            }

            var nextMember = member.Expression as MemberExpression;

            var expMember = RemoveDynamicType(nextMember);

            return Expression.MakeMemberAccess(expMember, member.Member);
        }

        private static Boolean HasManyMemberAccess(Expression expression)
        {
            if ( IsConstant(expression) )
            {
                return false;
            }

            var methodCall = expression as MethodCallExpression;

            if ( methodCall != null )
            {
                expression = methodCall.Object;
            }

            var member = expression as MemberExpression;

            if ( member != null && member.Member.ReflectedType == typeof(DateTime) )
            {
                member = member.Expression as MemberExpression;
            }

            if ( member != null && IsStringMember(member, member.Expression as MemberExpression) )
            {
                return HasManyMemberAccess(member.Expression);
            }

            return member != null && member.Expression is MemberExpression;
        }

        private static bool IsStringMember(MemberExpression member, MemberExpression next)
        {
            return next != null && next.Member.Type() == typeof(String) &&
                member.Member.Name == "Length";
        }

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
#if NET35
            return Expression.Lambda(body, parameters.ToArray());
#else
            return Expression.Lambda(body, parameters);
#endif

        }

        /*
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
        */

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

        private static Boolean IsConstant(Expression expression)
        {
            return ExtractConstant(expression) != null;
        }

        private bool IsMemberAccess(MethodCallExpression expression)
        {
            return expression.Object is MemberExpression;
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
                case ExpressionType.Call:
                    return ExtractConstant(((MethodCallExpression)expression).Object);
                default: return null;
            }
        }

        private static object ParseValue(Expression arg)
        {
            var constantExpression = arg as ConstantExpression;
            if ( constantExpression != null )
            {
                if ( Attribute.IsDefined(constantExpression.Type, typeof(CompilerGeneratedAttribute)) )
                {
                    // TODO: this is a little bit weird code. Do I really have to check the fields of this type?!
                    return constantExpression.Type.Fields()
                        .First(e => !e.Name.Contains("<>"))
                        .Get(constantExpression.Value);
                }

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

        /*
                private string ExtractName(Expression expression, IEntityInfo entityInfo)
                {
                    if ( expression is MemberExpression )
                        return ((MemberExpression)expression).Member.Name;
                    if ( expression is ParameterExpression )
                        return entityInfo.KeyColumns.First().ColumnName;

                    return null;
                }
        */

        private string Merge(CompilerBuffer buffer)
        {
            if ( buffer.Union != null && buffer.WasAggregated )
            {
                var tableAlias = _formatter.GetTableAlias(_depth + 1);
                //
                // I might need to store the index on the buffer.
                //
                var projection = buffer.Projection.Replace(_formatter.GetTableAlias(_index), tableAlias);

                buffer.Projection = buffer.OldProjection;
                if ( String.IsNullOrEmpty(buffer.Projection) )
                {
                    CompileDefaultProjection(buffer);
                }

                return new StringBuilder()
                    .Append(_formatter.Select + " ").Append(projection)
                    .Append(" " + _formatter.From + " ").Append(_formatter.BeginWrap())
                    .Append(MergeAll(buffer))
                    .Append(_formatter.EndWrap())
                    .Append(tableAlias)
                    .ToString();
            }

            return MergeAll(buffer);
        }

        private string MergeAll(CompilerBuffer buffer)
        {
            return new StringBuilder()
                .Append(_formatter.Select + " ").Append(buffer.Projection)
                .Append(" " + _formatter.From + " ").Append(buffer.From)
                .Append(buffer.Predicates)
                .Append(buffer.GroupBy)
                .Append(buffer.OrderBy)
                .Append(MergeUnion(buffer.Union, buffer.Projection))
                .ToString();
        }

        private string MergeUnion(CompilerBuffer union, StringBuffer projection)
        {
            if ( union == null )
            {
                return null;
            }
            return " " + _formatter.Union + " " + Merge(union);
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

        private CompilerBuffer CreateBuffer(IQueryable queryable)
        {
            var buffer = new CompilerBuffer
            {
                EntityInfo = _mapper.Map(queryable.ElementType),
                PredicatedColumns = new Collection<IEntityColumnInfo>()
            };

            if ( queryable.ElementType.IsDynamic() )
            {
                return buffer;
            }

            EntityInfo = buffer.EntityInfo;

            buffer.EntityInfo.RaiseIfNull(
                Errors.Query_EntityInfoNotFound,
                new { ElementTypeName = queryable.ElementType.Name });

            return buffer;
        }

        private IQueryable EvaluateQuery(IQueryable queryable)
        {
            if ( !queryable.ElementType.IsDynamic() )
            {
                return queryable;
            }

            var callExpression = queryable.Expression as MethodCallExpression;

            if ( callExpression != null )
            {
                switch ( callExpression.Method.Name )
                {
                    case "Select":
                        var lambda = ExtractLambda(callExpression.Arguments[1], false);

                        //
                        // Unit-Test: SqlTranslation_2Joins
                        // In multiple join situations the First parameter is a dynamic type. 
                        // so we ignore this fact for now. Will be resolved later on.
                        //
                        if ( lambda.Parameters.First().Type.IsDynamic() )
                        {
                            return queryable;
                        }

                        Indexer[lambda.Parameters.First()] = this;
                        return new QueryableStub(lambda.Parameters.First().Type, queryable.Expression);
                }
            }

            return queryable;
        }

        private readonly Stack<String> _predicates = new Stack<string>();

        private void SafePredicate(CompilerBuffer buffer)
        {
            _predicates.Push(buffer.Predicates);
            buffer.Predicates = null;
        }

        private void RestorePredicate(CompilerBuffer buffer)
        {
            if ( _predicates.Count > 0 )
            {
                buffer.Predicates = _predicates.Pop();
            }
            else
            {
                buffer.Predicates = null;
            }
        }

        #endregion
    }

}