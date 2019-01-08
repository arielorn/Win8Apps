using System.Linq;

namespace Win8Apps.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> query, int currentPage, int defaultPage, int pageSize)
        {
            return query
                .Skip((currentPage - defaultPage) * pageSize)
                .Take(pageSize);
        }
    }
}