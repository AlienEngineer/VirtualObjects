﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using VirtualObjects.Queries.Compilation;

namespace VirtualObjects.Queries.Builder
{
    class ExpressionBasedBuilder : IBuiltedQuery, IQueryBuilder 
    {
        private readonly IQueryCompiler _queryCompiler;

        public ExpressionBasedBuilder(IQueryCompiler queryCompiler)
        {
            _queryCompiler = queryCompiler;
            Predicates = new Collection<Expression>();
            Joins = new Collection<Expression>();
        }

        public Expression Projection { get; set; }
        public Type SourceType { get; set; }
        public ICollection<Expression> Predicates { get; set; }
        public ICollection<Expression> Joins { get; set; }


        public IQueryInfo BuildQuery()
        {
            return _queryCompiler.CompileQuery(this);
        }
  
        public void Project(Expression projection)
        {
            Projection = projection;
        }

        public void Project<T>(Expression<Func<T, Object>> projection)
        {
            Project((Expression)projection);
        }

        public void From(Type src)
        {
            SourceType = src;
        }

        public void From<T>()
        {
            From(typeof(T));
        }

        public void Where(Expression predicate)
        {
            var lambda = (LambdaExpression)predicate;

            if ( lambda.Body is MemberExpression )
            {
                predicate = Expression.Equal(lambda.Body, Expression.Constant(true));
            }

            Predicates.Add(predicate);
        }

        public void Where<T>(Expression<Func<T, bool>> predicate)
        {
            Where((Expression) predicate);
        }

        public void Join(Expression joinExpression)
        {
            Joins.Add(joinExpression);
        }

        public void Join<T, T1>(Expression<Func<T, T1, Boolean>> joinExpression)
        {
            Join((Expression)joinExpression);
        }

        public override string ToString()
        {
            try
            {
                return BuildQuery().CommandText;
            }
            catch (Exception)
            {
                return base.ToString();
            }
            
        }

    }
}
