using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
	public class Factory<T> where T : IDisposable
	{
		public static T Create()
		{
			if (_t == null)
			{
				return (T)Activator.CreateInstance(_create); 
			}

			return _t;
		}

		private static T _t = default(T);

		private static Type _create;

		public static void Register(T x)
		{
			_t = x;
		}

		public static bool Register(Type create)
		{
			if (create.GetInterface(typeof(T).Name) == null)
				throw new ArgumentException();

			_create = create;
			return true;
        }
	}
}
