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
        private DbContext context;
        private IQueryable<T> query;

        public QueryBuilder(DbContext context)
        {
            this.context = context;
            this.query = this.context.Set<T>();
        }

        public IQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            this.query = this.query.Where(predicate);
            return this;
        }

        public IQueryBuilder<T> Include(Expression<Func<T, object>> path)
        {
            this.query = this.query.Include(path);
            return this;
        }

        public IQueryBuilder<T> OrderBy(Expression<Func<T, object>> path)
        {
            this.query = this.query.OrderBy(path);
            return this;
        }

        public IQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> path)
        {
            this.query = this.query.OrderByDescending(path);
            return this;
        }
        
        public IQueryBuilder<T> Page(int page, int pageSize)
        {
            this.query = this.query.Skip(page * pageSize).Take(pageSize);
            return this;
        }

        public T FirstOrDefault()
        {
            return this.query.FirstOrDefault<T>();
        }

        public Task<T> FirstOrDefaultAsync()
        {
            return this.query.FirstOrDefaultAsync();
        }

        public List<T> ToList()
        {
            return this.query.ToList();
        }

        public Task<List<T>> ToListAsync()
        {
            return this.query.ToListAsync();
        }

        public int Count()
        {
            return this.query.Count();
        }

        public Task<int> CountAsync()
        {
            return this.query.CountAsync();
        }
    }
}
