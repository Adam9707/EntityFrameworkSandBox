using EFSandBoxAPI.Contracts;
using EFSandBoxAPI.DBContexts;
using EFSandBoxAPI.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Principal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EFSandBoxAPI.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private DbContext context;
        private DbSet<T> dbSet;

        public GenericRepository(DbContext _context)
        {
            this.context = _context;
            this.dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = dbSet.AsQueryable();
            if (includes.Any())
            {
               result = includes.Aggregate(result, (current, include) => current.Include(include));
            }
            return await result.Where(predicate).AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<T>> GetManyWithTrackingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = dbSet.AsQueryable();
            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) => current.Include(include));
            }
            return await result.Where(predicate).ToListAsync();
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = dbSet.AsQueryable();
            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) => current.Include(include));
            }
            return await result.AsNoTracking().FirstAsync(predicate);
        }

        public async Task<T> GetOneWithTrackingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = dbSet.AsQueryable();
            if (includes.Any())
            {
                result = includes.Aggregate(result, (current, include) => current.Include(include));
            }
            return await result.FirstAsync(predicate);
        }

        public async Task Insert(T obj)
        {
            await context.Set<T>().AddAsync(obj);
        }

        public void Update(T obj)
        {
            var entry = context.Attach(obj);
            entry.State = EntityState.Modified;
        }

        public void Delete(T obj)
        {
            var entry = context.Attach(obj);
            entry.State = EntityState.Deleted;
        }

        public async Task<PagedResult<T>> Pagination(PaginationParameters<T> paginationParameters)
        {
            var sortProperty = typeof(T).GetProperties().SingleOrDefault(p => p.Name.Equals(paginationParameters.SortBy));       

            var query = dbSet.AsQueryable()
                        .Where(paginationParameters.Filterpredicate);

            var totalCount = query.Count();

            if (paginationParameters.SortBy != null)
            {
                query = paginationParameters.SortByDesceding
                    ? query.OrderByDescending(x => EF.Property<T>(x,sortProperty.Name))
                    : query.OrderBy(x => EF.Property<T>(x, sortProperty.Name));
            }

            var result = await query.Skip(paginationParameters.PageSize * (paginationParameters.PageNumber - 1))
               .Take(paginationParameters.PageSize)
               .ToListAsync();

            var pagedResult = new PagedResult<T>(result, totalCount, paginationParameters.PageSize, paginationParameters.PageNumber);

            return pagedResult;
        }
    }
}
