using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Config;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries
{
    public interface IQueryTranslator
    {
        IQueryInfo TranslateQuery(IQueryable queryable);
        IQueryInfo TranslateQuery(Expression expression);
        IQueryInfo TranslateParametersOnly(IQueryable queryable, int howMany);
        IQueryInfo TranslateParametersOnly(Expression expression, int howMany);
    }

    public interface IQueryTranslatorProvider
    {
        IQueryTranslator CreateQueryTranslator(IFormatter formatter, IMapper mapper);
    }
}
