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
        private bool disposed = false;
        private DbContext context;

        public ProxxonRepository(DbContext context)
        {
            this.context = context;
        }

        public IQueryBuilder<T> Query<T>() where T : class
        {
            return new QueryBuilder<T>(this.context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if(this.disposed)               
                return;

            if(disposing)
            {
                this.context.Dispose();
            }

            this.disposed = true;
            this.context = null;
        }
    }
}
