using Sales.Shared.DTO;

namespace Sales.API.Helper
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDto pagination)
        {
            return queryable
                .Skip((pagination.Page - 1) * pagination.RecordNumber)
                .Take(pagination.RecordNumber);
        }
    }
}
