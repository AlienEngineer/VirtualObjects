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
        private readonly IEntityBag _entityBag;
        private readonly SessionConfiguration _configuration;
        private readonly IDictionary<int, IQueryInfo> _cachedQueries;
        private readonly IDictionary<Expression, IQueryInfo> _cachedExpressionQueries;

        public CachingTranslator(IFormatter formatter, IMapper mapper, IEntityBag entityBag, SessionConfiguration configuration)
        {
            _formatter = formatter;
            _mapper = mapper;
            _entityBag = entityBag;
            _configuration = configuration;

            _cachedQueries = new Dictionary<int, IQueryInfo>();
            _cachedExpressionQueries = new Dictionary<Expression, IQueryInfo>();
        }


        public IQueryInfo TranslateQuery(Expression expression)
        {
            IQueryInfo result;

            if (_cachedExpressionQueries.TryGetValue(expression, out result))
            {
                if (result.Parameters.Count > 0)
                {
                    result.Parameters = TranslateParametersOnly(expression, result.Parameters.Count).Parameters;
                }

                return result;
            }

            var hashCode = expression.ToString().GetHashCode();

            if ( _cachedQueries.TryGetValue(hashCode, out result) )
            {
                if (result.Parameters.Count > 0)
                {
                    result.Parameters = TranslateParametersOnly(expression, result.Parameters.Count).Parameters;
                }

                return result;
            }

            return _cachedExpressionQueries[expression] = _cachedQueries[hashCode] = new QueryTranslator(_formatter, _mapper, _entityBag, _configuration).TranslateQuery(expression);
        }

        public IQueryInfo TranslateQuery(IQueryable queryable)
        {
            return TranslateQuery(queryable.Expression);
        }

        public IQueryInfo TranslateParametersOnly(IQueryable queryable, int howMany)
        {
            return new QueryTranslator(_formatter, _mapper, _entityBag, _configuration).TranslateParametersOnly(queryable, howMany);
        }

        public IQueryInfo TranslateParametersOnly(Expression expression, int howMany)
        {
            return new QueryTranslator(_formatter, _mapper, _entityBag, _configuration).TranslateParametersOnly(expression, howMany);
        }
    }
}