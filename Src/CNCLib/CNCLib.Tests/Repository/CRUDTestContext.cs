using System;
using CNCLib.Repository.Context;
using Framework.Contracts.Repository;

namespace CNCLib.Tests.Repository
{
    public class CRUDTestContext<TEntry, TKey, TIRepository> : IDisposable where TEntry : class  where TIRepository : ICRUDRepository<TEntry, TKey>
    {
        public CNCLibContext DbContext { get; private set; }
        public IUnitOfWork UnitOfWork { get; private set; }
        public TIRepository Repository { get; private set; }

        public CRUDTestContext(CNCLibContext dbContext, IUnitOfWork uow, TIRepository repository)
        {
            DbContext = dbContext;
            UnitOfWork = uow;
            Repository = repository;
        }

        public void Dispose()
        {
            DbContext.Dispose();
            DbContext = null;
        }
    }
}