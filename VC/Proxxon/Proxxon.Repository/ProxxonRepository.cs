using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.Repository
{
    public class ProxxonRepository : IProxxonRepository
    {
        private bool _disposed = false;
        private DbContext _context;

        public ProxxonRepository(DbContext context)
        {
            this._context = context;
        }

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new QueryBuilder<T>(this._context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if(this._disposed)               
                return;

            if(disposing)
            {
                this._context.Dispose();
            }

            this._disposed = true;
            this._context = null;
        }
    }
}
