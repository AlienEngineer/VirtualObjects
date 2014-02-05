using System.Linq;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// Converts the ExpressionTree instance to a QueryInfo.
    /// </summary>
    public interface IConverter
    {
        IQueryInfo ConvertToQuery(IQueryable queryable);
    }
}