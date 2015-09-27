using Framework.EF;
using CNCLib.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository
{
	public static class UnitOfWorkFactory
	{
		static public IUnitOfWork Create()
		{
			return new UnitOfWork<CNCLibContext>();
		}
	}
}
