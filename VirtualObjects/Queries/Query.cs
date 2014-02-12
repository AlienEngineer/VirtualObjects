using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace VirtualObjects.Queries
{
    class Query : IQueryable
    {

        public Query(IQueryProvider provider, Expression expression, Type elementType)
        {
            Provider = provider;
            ElementType = elementType;
            Expression = expression;
        }
        
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
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
        public Query(IQueryProvider provider, Expression expression)
            : base(provider, expression, typeof(TElement))
        {
        }

        public Query(IQueryProvider provider) 
            : this(provider, new List<TElement>().AsQueryable().Expression)
        {

        }

        public new IEnumerator<TElement> GetEnumerator()
        {
            var result = Provider.Execute<IEnumerable<TElement>>(Expression);

            return result.GetEnumerator();
        }
    }
}
