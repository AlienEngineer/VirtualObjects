using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Config;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IQueryTranslator
    {
        /// <summary>
        /// Translates the query.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <returns></returns>
        IQueryInfo TranslateQuery(IQueryable queryable);
        /// <summary>
        /// Translates the query.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        IQueryInfo TranslateQuery(Expression expression);
        /// <summary>
        /// Translates the parameters only.
        /// </summary>
        /// <param name="queryable">The queryable.</param>
        /// <param name="howMany">The how many.</param>
        /// <returns></returns>
        IQueryInfo TranslateParametersOnly(IQueryable queryable, int howMany);
        /// <summary>
        /// Translates the parameters only.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="howMany">The how many.</param>
        /// <returns></returns>
        IQueryInfo TranslateParametersOnly(Expression expression, int howMany);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IQueryTranslatorProvider
    {
        /// <summary>
        /// Creates the query translator.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="entityBag">The entity bag.</param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        IQueryTranslator CreateQueryTranslator(IFormatter formatter, IMapper mapper, IEntityBag entityBag, SessionConfiguration configuration);
    }
}
