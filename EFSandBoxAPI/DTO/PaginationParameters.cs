using System.Linq.Expressions;

namespace EFSandBoxAPI.DTO
{
    public class PaginationParameters<T>
    {
        public string Filter;
        public string FilterBy;
        public string SortBy;
        public bool SortByDesceding;
        public int PageNumber;
        public int PageSize;
        public Expression<Func<T, bool>> Filterpredicate = x => true;
    }
}
