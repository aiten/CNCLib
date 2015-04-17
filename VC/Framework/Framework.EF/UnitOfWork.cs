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

		public DbContext Context 
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

		public void Save()
		{
			Context.SaveChanges();
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
				_context.Dispose();
			}

			_disposed = true;
			_context = null;
		}
	}
}
