using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
    public static class Singleton<T> where T: new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
                return _instance;
            }
        }

        public static bool Allocated
        {
            get { return _instance != null; }
        }

        public static void Free()
        {
            if (_instance != null)
            {
                foreach (MethodInfo mi in typeof(T).GetMethods())
                {
                    if (mi.Name == "Dispose" && mi.GetParameters().Count() == 0)
                    {
                        mi.Invoke(_instance, new object[0]);
                    }
                }

                _instance = default(T);
            }
        }
    }
}
