using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Framework.EF
{
	public class QueryBuilder<T> : IQueryBuilder<T> where T : class
    {
        private DbContext _context;
        private IQueryable<T> _query;

        public QueryBuilder(DbContext context)
        {
            _context = context;
            _query = _context.Set<T>();
        }

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            _query = _query.Where(predicate);
            return this;
        }

        public IQueryBuilder<T> Include(Expression<Func<T, object>> path)
        {
            _query = _query.Include(path);
            return this;
        }

        public IQueryBuilder<T> OrderBy(Expression<Func<T, object>> path)
        {
            _query = _query.OrderBy(path);
            return this;
        }

        public IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> path)
        {
            _query = _query.OrderByDescending(path);
            return this;
        }
        
        public IQueryBuilder<T> Page(int page, int pageSize)
        {
            _query = _query.Skip(page * pageSize).Take(pageSize);
            return this;
        }

        public T FirstOrDefault()
        {
            return _query.FirstOrDefault<T>();
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return _query.FirstOrDefaultAsync();
        }

        public List<T> ToList()
        {
            return _query.ToList();
        }

        public Task<List<T>> ToListAsync()
        {
            return _query.ToListAsync();
        }

        public int Count()
        {
            return _query.Count();
        }

        public Task<int> CountAsync()
        {
            return _query.CountAsync();
        }
    }
}
