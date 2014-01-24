using System.Linq;
using System.Linq.Expressions;

namespace VirtualObjects.Queries
{
    public interface IQueryTranslator
    {
        IQueryInfo TranslateQuery(IQueryable queryable);
        IQueryInfo TranslateQuery(Expression expression);
        IQueryInfo TranslateParametersOnly(IQueryable queryable, int howMany);
        IQueryInfo TranslateParametersOnly(Expression expression, int howMany);
        
    }
}
