using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
	public interface IFactory
	{
		TInterface Create<TInterface>() where TInterface : IDisposable;

		void Register(Type from, Type to);
	}
}
