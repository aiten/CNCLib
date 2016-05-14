using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Logic.Contracts
{
	public interface IRestController<T>
	{
		T Get(int id);
		IEnumerable<T> GetAll();
		int Add(T value);
		int Update(T value);
		void Delete(T value);
	}
}
