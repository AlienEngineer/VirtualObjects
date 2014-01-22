using System.Linq;

namespace VirtualObjects.Queries
{
    public interface IQueryTranslator
    {
        IQueryInfo TranslateQuery(IQueryable queryable);
    }
}
