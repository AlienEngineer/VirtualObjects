using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VirtualObjects.Queries
{
    abstract class Query : IQueryable
    {
        protected Query(IQueryProvider provider, Expression expression, Type elementType)
        {
            Provider = provider;
            ElementType = elementType;
            Expression = expression;
        }

        public abstract IEnumerator GetEnumerator();

        public Expression Expression { get; }
        public Type ElementType { get; }
        public IQueryProvider Provider { get; }

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
        public Query(IQueryProvider provider, Expression expression)
            : base(provider, expression, typeof(TElement))
        {
        }

        public Query(IQueryProvider provider)
            : this(provider, new List<TElement>().AsQueryable().Expression)
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
