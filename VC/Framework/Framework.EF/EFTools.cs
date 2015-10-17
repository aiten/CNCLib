using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Tools;

namespace Framework.EF
{
	public static class EFTools
	{
		static public void Sync<t>(IUnitOfWork uow, ICollection<t> inDb, ICollection<t> toDb, Func<t, t, bool> predicate) 
		{
			// 1. Delete from DB (in DB) and update
			List<t> delete = new List<t>();

			foreach (t entityInDb in inDb)
			{
                var entityToDb = toDb.FirstOrDefault(x => predicate(x, entityInDb));
				if (entityToDb != null && predicate(entityToDb, entityInDb))
				{
					entityInDb.CopyValueTypeProperties(entityToDb);
				}
				else
				{
					delete.Add(entityInDb);
				}
			}

			foreach (var del in delete)
			{
				uow.MarkDeleted(del);
			}

			// 2. Add To DB

			foreach (t entityToDb in toDb)
			{
				var entityInDb = inDb.FirstOrDefault(x => predicate(x, entityToDb));
				if (entityInDb == null || predicate(entityToDb, entityInDb) == false)
				{
					uow.MarkNew(entityToDb);
				}
			}
		}
	}
}
