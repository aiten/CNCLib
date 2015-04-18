using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Framework.EF
{
	public interface IUnitOfWork : IDisposable
    {
        void MarkDirty(object entity);
		void MarkNew(object entity);
		void MarkDeleted(object entity);

		IQueryBuilder<T> Query<T>() where T : class;

		void Save();

		// Transaction

		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();

		// Global

		void InitializeDatabase();
    }
}
