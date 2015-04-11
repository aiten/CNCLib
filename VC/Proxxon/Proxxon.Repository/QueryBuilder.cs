using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.Repository
{
    public class QueryBuilder<T> : IQueryBuilder<T> where T : class
    {
        private DbContext _context;
        private IQueryable<T> _query;

        public QueryBuilder(DbContext context)
        {
            this._context = context;
            this._query = this._context.Set<T>();
        }

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            this._query = this._query.Where(predicate);
            return this;
        }

        public IQueryBuilder<T> Include(Expression<Func<T, object>> path)
        {
            this._query = this._query.Include(path);
            return this;
        }

        public IQueryBuilder<T> OrderBy(Expression<Func<T, object>> path)
        {
            this._query = this._query.OrderBy(path);
            return this;
        }

        public IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> path)
        {
            this._query = this._query.OrderByDescending(path);
            return this;
        }
        
        public IQueryBuilder<T> Page(int page, int pageSize)
        {
            this._query = this._query.Skip(page * pageSize).Take(pageSize);
            return this;
        }

        public T FirstOrDefault()
        {
            return this._query.FirstOrDefault<T>();
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return this._query.FirstOrDefaultAsync();
        }

        public List<T> ToList()
        {
            return this._query.ToList();
        }

        public Task<List<T>> ToListAsync()
        {
            return this._query.ToListAsync();
        }

        public int Count()
        {
            return this._query.Count();
        }

        public Task<int> CountAsync()
        {
            return this._query.CountAsync();
        }
    }
}
