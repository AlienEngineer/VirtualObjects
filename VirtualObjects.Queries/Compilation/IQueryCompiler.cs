using System;
using System.Linq;
using VirtualObjects.Queries.Builder;

namespace VirtualObjects.Queries.Compilation
{
    public interface IQueryCompiler
    {
        /// <summary>
        /// Compiles the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        IQueryInfo CompileQuery(IBuiltedQuery query);
    }
}
