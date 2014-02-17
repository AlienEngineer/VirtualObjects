using System.Linq;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// Converts the ExpressionTree instance to a QueryInfo.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Converts to query.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns></returns>
        IQueryInfo ConvertToQuery(IQueryable queryable);
    }
}