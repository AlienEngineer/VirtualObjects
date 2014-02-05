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
        private readonly IDictionary<int, IQueryInfo> _cachedQueries;

        public CachingTranslator(IFormatter formatter, IMapper mapper)
        {
            _formatter = formatter;
            _mapper = mapper;

            _cachedQueries = new Dictionary<int, IQueryInfo>();
        }


        public IQueryInfo TranslateQuery(Expression expression)
        {
            IQueryInfo result;

            var hashCode = expression.ToString().GetHashCode();

            if ( _cachedQueries.TryGetValue(hashCode, out result) )
            {
                result.Parameters = TranslateParametersOnly(expression, result.Parameters.Count).Parameters;
                return result;
            }

            return _cachedQueries[hashCode] = new QueryTranslator(_formatter, _mapper).TranslateQuery(expression);
        }

        public IQueryInfo TranslateQuery(IQueryable queryable)
        {
            return TranslateQuery(queryable.Expression);
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