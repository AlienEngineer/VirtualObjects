using System;
using System.Linq.Expressions;

namespace VirtualObjects.Queries.Builder
{
    public interface IBuiltedQuery
    {
        Expression Projection { get; }
    }
}