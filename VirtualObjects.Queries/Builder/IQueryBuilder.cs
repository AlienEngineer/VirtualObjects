using System;
using System.Linq.Expressions;

namespace VirtualObjects.Queries.Builder
{
    public interface IQueryBuilder
    {
        void Project(Expression projection);
        void Project<T>(Expression<Func<T, Object>> projection);

        void From(Type src);
        void From<T>();

        void Where(Expression predicate);
        void Where<T>(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Builds the query.
        /// </summary>
        IQueryInfo BuildQuery();

    }
}