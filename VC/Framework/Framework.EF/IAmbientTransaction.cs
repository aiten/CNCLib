using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.EF
{
	public interface IAmbientTransaction, IDisposable
	{
		void Commit();
		void Rollback();
	}
}
