using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VirtualObjects.Queries.Compilation;

namespace VirtualObjects.Queries.Builder
{
    public interface IBuiltedQuery
    {
        Type SourceType { get; }

        Expression Projection { get; }

        ICollection<Expression> Predicates { get; }
        ICollection<Expression> Joins { get; }

    }
}