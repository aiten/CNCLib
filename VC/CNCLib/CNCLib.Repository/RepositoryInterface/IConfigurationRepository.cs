using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Repository.RepositoryInterface
{
	public interface IConfigurationRepository : IDisposable
	{
		Entities.Configuration Get(string group, string name);
		void Delete(Entities.Configuration configuration);
		void Save(Entities.Configuration configuration);
	}
}
