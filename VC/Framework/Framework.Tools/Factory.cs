using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
	public class Factory : IFactory
	{
		public TInterface Create<TInterface>() where TInterface : IDisposable
		{
			Type to;
			if (_typemapping.TryGetValue(typeof(TInterface),out to) == false)
			{
				throw new ArgumentException("Invalid InterfaceType, not mapped");
			}
			return (TInterface)Activator.CreateInstance(to);
		}

		private Dictionary<Type, Type> _typemapping = new Dictionary<Type, Type>();

		public void Register(Type from, Type to)
		{
			Type fromtest;
			if (_typemapping.TryGetValue(from, out fromtest) == true)
			{
				throw new ArgumentException();
			}
			_typemapping[from] = to;
		}
	}
}
