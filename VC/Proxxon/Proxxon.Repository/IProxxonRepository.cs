using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.Repository
{
    public interface IProxxonRepository : IDisposable
    {
        IQueryBuilder<T> Query<T>() where T : class;
    }
}
