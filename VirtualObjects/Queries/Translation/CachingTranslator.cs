using System;
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
        private readonly IDictionary<int, IQueryInfo> _cachedQueries;

        public CachingTranslator(IFormatter formatter, IMapper mapper, IEntityBag entityBag)
        {
            _formatter = formatter;
            _mapper = mapper;
            _entityBag = entityBag;

            _cachedQueries = new Dictionary<int, IQueryInfo>();
        }


        public IQueryInfo TranslateQuery(Expression expression)
        {
            IQueryInfo result;

            var hashCode = expression.ToString().GetHashCode();

            if ( _cachedQueries.TryGetValue(hashCode, out result) )
            {
                if (result.Parameters.Count > 0)
                {
                    result.Parameters = TranslateParametersOnly(expression, result.Parameters.Count).Parameters;
                }

                return result;
            }

            return _cachedQueries[hashCode] = new QueryTranslator(_formatter, _mapper, _entityBag).TranslateQuery(expression);
        }

        public IQueryInfo TranslateQuery(IQueryable queryable)
        {
            return TranslateQuery(queryable.Expression);
        }

        public IQueryInfo TranslateParametersOnly(IQueryable queryable, int howMany)
        {
            return new QueryTranslator(_formatter, _mapper, _entityBag).TranslateParametersOnly(queryable, howMany);
        }

        public IQueryInfo TranslateParametersOnly(Expression expression, int howMany)
        {
            return new QueryTranslator(_formatter, _mapper, _entityBag).TranslateParametersOnly(expression, howMany);
        }
    }
}