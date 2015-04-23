using Framework.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.EF
{
	public class UnitOfWork<T> : IUnitOfWork where T : DbContext, new()
	{
		private DbContext _context;

		private DbContext Context 
		{
			  get 
			  {
				  if (_context == null)
					  _context = new T();
					return _context;
			  }
		}

		IQueryBuilder<To> IUnitOfWork.Query<To>()
		{
			return new QueryBuilder<To>(Context);
		}

		public void MarkDirty(object entity)
		{
			Context.Entry(entity).State = EntityState.Modified;
		}

		public void MarkNew(object entity)
		{
			Context.Entry(entity).State = EntityState.Added;
		}

		public void MarkDeleted(object entity)
		{
			Context.Entry(entity).State = EntityState.Deleted;
		}

		public void Save()
		{
			Context.SaveChanges();
		}

		public int ExecuteSqlCommand(string sql)
		{
			return Context.Database.ExecuteSqlCommand(sql);
		}

		public int ExecuteSqlCommand(string sql, params object[] parameters)
		{
			return Context.Database.ExecuteSqlCommand(sql, parameters);
		}

		bool _disposed; 

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				if (InTransaction)
					RollbackTransaction();

				_context.Dispose();
			}

			_disposed = true;
			_context = null;
		}


		#region Transaction

		private System.Data.Entity.DbContextTransaction _dbTran;

		public bool InTransaction { get { return _dbTran != null; } }

		public void BeginTransaction()
		{
			if (InTransaction) throw new ArgumentException();
			_dbTran = Context.Database.BeginTransaction();
		}

		public void CommitTransaction()
		{
			if (InTransaction==false) throw new ArgumentException();
			_dbTran.Commit();
			_dbTran = null;
		}

		public void RollbackTransaction()
		{
			if (InTransaction==false) throw new ArgumentException();
			_dbTran.Rollback();
			_dbTran = null;
		}


		#endregion

		public void InitializeDatabase()
		{
			Context.Database.Initialize(true);        
		}

	}
}
