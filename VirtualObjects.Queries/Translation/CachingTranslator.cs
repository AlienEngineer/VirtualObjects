using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Config;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries.Translation
{
    class CachingTranslator :  IQueryTranslator
    {
        private readonly IFormatter _formatter;
        private readonly IMapper _mapper;
        private readonly IDictionary<Expression, IQueryInfo> _cachedQueries;

        class ExpressionEquality : IEqualityComparer<Expression>
        {
            public bool Equals(Expression x, Expression y)
            {
                return (x == null && y == null) ||
                    ((x != null && y != null) && (x.ToString() == y.ToString()));
            }

            public int GetHashCode(Expression obj)
            {
                return obj.GetHashCode();
            }
        }

        public CachingTranslator(IFormatter formatter, IMapper mapper)
        {
            _formatter = formatter;
            _mapper = mapper;

            _cachedQueries = new Dictionary<Expression, IQueryInfo>(new ExpressionEquality());
        }


        public IQueryInfo TranslateQuery(Expression expression)
        {
            IQueryInfo result;
            if (_cachedQueries.TryGetValue(expression, out result))
            {
                return TranslateParametersOnly(expression, result.Parameters.Count);
            }

            return _cachedQueries[expression] = new QueryTranslator(_formatter, _mapper).TranslateQuery(expression);
        }

        public IQueryInfo TranslateQuery(IQueryable queryable)
        {
            return new QueryTranslator(_formatter, _mapper).TranslateQuery(queryable.Expression);
        }

        public IQueryInfo TranslateParametersOnly(IQueryable queryable, int howMany)
        {
            return new QueryTranslator(_formatter, _mapper).TranslateParametersOnly(queryable, howMany);
        }

        public IQueryInfo TranslateParametersOnly(Expression expression, int howMany)
        {
            return new QueryTranslator(_formatter, _mapper).TranslateParametersOnly(expression, howMany);
        }
    }
}