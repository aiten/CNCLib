using Framework.EF;
using Proxxon.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxxon.Repository
{
	public static class UnitOfWorkFactory
	{
		static public IUnitOfWork Create()
		{
			return new UnitOfWork<ProxxonContext>();
		}
	}
}
