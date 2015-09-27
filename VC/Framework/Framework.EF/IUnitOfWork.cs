using System;

namespace Framework.EF
{
	public interface IUnitOfWork : IDisposable
    {
        void MarkDirty(object entity);
		void MarkNew(object entity);
		void MarkDeleted(object entity);

		IQueryBuilder<T> Query<T>() where T : class;

		void Save();

		// SQL Commands

		int ExecuteSqlCommand(string sql);
		int ExecuteSqlCommand(string sql, params object[] parameters);

		// Transaction

		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();

		// Global

		void InitializeDatabase();
    }
}
