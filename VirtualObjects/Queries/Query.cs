using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VirtualObjects.Queries
{
    abstract class Query : IQueryable
    {
        public Query(IQueryProvider provider, Expression expression, Type elementType, SessionContext context)
        {
            Provider = provider;
            ElementType = elementType;
            Context = context;
            Expression = expression;
        }

        public abstract IEnumerator GetEnumerator();

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
        public SessionContext Context { get; set; }
        public IQueryProvider Provider { get; private set; }

        public override string ToString()
        {
            var provider = Provider as QueryProvider;
            if (provider != null)
            {
                return provider.Translate(this);
            }

            return base.ToString();
        }
    }

    class Query<TElement> : Query, IOrderedQueryable<TElement>
    {
        public Query(IQueryProvider provider, Expression expression, SessionContext context)
            : base(provider, expression, typeof(TElement), context)
        {
        }

        public Query(IQueryProvider provider, SessionContext context)
            : this(provider, new List<TElement>().AsQueryable().Expression, context)
        {

        }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            var result = Provider.Execute<IEnumerable<TElement>>(Expression);

            return result.GetEnumerator();
        }

        public override IEnumerator GetEnumerator()
        {
            var result = Provider.Execute<IEnumerable<TElement>>(Expression);

            return result.GetEnumerator();
        }
    }
}
