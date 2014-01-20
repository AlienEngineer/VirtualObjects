using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace VirtualObjects.Queries.Builder
{
    public interface IBuiltedQuery
    {
        Type SourceType { get; }

        Expression Projection { get; }

        ICollection<Expression> Predicates { get; set; }
    }
}