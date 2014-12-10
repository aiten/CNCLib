using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
    public static class ObjectMapper
    {
        public static object MyCloneProperties(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] properties = t.GetProperties();

            Object p = t.InvokeMember("", System.Reflection.BindingFlags.CreateInstance,
                null, o, null);

            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanWrite)
                {
                    pi.SetValue(p, pi.GetValue(o, null), null);
                }
            }

            return p;
        }
        public static T CloneProperties<T>(this T src)
        {
            T newobj = (T) MyCloneProperties(src);
            return newobj;
        }

		public static void MyCopyProperties(object dest, object src)
		{
			Type t_dest = dest.GetType();
			Type t_src  = src.GetType();

			PropertyInfo[] properties = t_dest.GetProperties();

			foreach (PropertyInfo pd in properties)
			{
				if (pd.CanWrite)
				{
					PropertyInfo ps = t_src.GetProperty(pd.Name);

					if (ps != null)
					{
						pd.SetValue(dest, ps.GetValue(src, null), null);
					}
				}
			}
		/*
			foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(src))
			{
				item.SetValue(dest, item.GetValue(src));
			}
		 */
		}

        public static void CopyProperties(object dest, object src)
        {
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(src))
            {
                item.SetValue(dest, item.GetValue(src));
            }
        }
        public static void CopyProperties<T>(this T dest, T src )
        {
			CopyProperties((object)dest, (object)src);
        }
    }
}
