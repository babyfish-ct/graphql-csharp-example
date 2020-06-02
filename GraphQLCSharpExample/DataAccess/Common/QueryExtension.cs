using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GraphQLCSharpExample.DataAccess.Common
{
    public static class QueryExtension
    {
        public static IQueryable<T> OrderBy<T>(
            this IQueryable<T> query,
            bool descending,
            params Expression<Func<T, object>>[] expressions)
        {
            if (expressions.Length != 0)
            {
                if (descending)
                {
                    query = query.OrderByDescending(expressions[0]);
                    for (int i = 1; i < expressions.Length; i++)
                    {
                        query = ((IOrderedQueryable<T>)query).ThenByDescending(expressions[i]);
                    }
                }
                else
                {
                    query = query.OrderBy(expressions[0]);
                    for (int i = 1; i < expressions.Length; i++)
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy(expressions[i]);
                    }
                }
            }
            return query;
        }

        public static IQueryable<T> Limit<T>(this IQueryable<T> query, int? limit, int? offset)
        {
            if (limit == null && offset != null)
            {
                throw new ArgumentException("Offset cannot be specified when limit is not specified");
            }
            if (offset != null)
            {
                query = query.Skip(offset.Value); // "Skip" must be called before "Take", is it bug of Linq2DB?
            }
            if (limit != null)
            {
                query = query.Take(limit.Value);
            }
            return query;
        }
    }
}
