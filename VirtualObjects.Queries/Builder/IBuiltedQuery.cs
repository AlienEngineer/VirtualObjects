using System;
using System.Linq.Expressions;

namespace VirtualObjects.Queries.Builder
{
    public interface IBuiltedQuery
    {
        Type SourceType { get; }

        Expression Projection { get; }
    }
}