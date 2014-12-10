using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
    public static class ObjectConverter
    {
        #region Simple

        public static object CloneProperties(object o)
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
            T newobj = (T) CloneProperties((object) src);
            return newobj;
        }
        public static TDest NewCloneProperties<TDest, TSrc>(this TSrc src) where TDest: new()
        {
            TDest dest = new TDest();
            CopyProperties((object)dest, (object)src);
           return dest;
        }

		public static void CopyProperties(object dest, object src)
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
		}

        public static void CopyPropertiesSameType(object dest, object src)
        {
            foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(src))
            {
                item.SetValue(dest, item.GetValue(src));
            }
        }
        public static void CopyProperties<T>(this T dest, T src )
        {
            CopyPropertiesSameType((object)dest, (object)src);
        }

        #endregion

        public static void AddCloneProperties<TDest, TSrc>(this IList<TDest> destlist, TSrc srclist)
            where TSrc : IEnumerable
            where TDest : new()
        {
            foreach (object src in srclist)
            {
                TDest dest = new TDest();
                CopyProperties((object)dest, (object)src);
                destlist.Add(dest);
            }
        }


        public static TDest[] CloneProperties<TDest, TSrc>(this TSrc srclist) where TSrc : IEnumerable   where TDest : new()
        {
            List<TDest> result = new List<TDest>();

            foreach (TSrc src in srclist)
            {
                TDest dest = new TDest();
                CopyProperties((object)dest, (object)src);
                result.Add(dest);

            }

            Type t = new TDest().GetType();

            Type tarray = t.GetType().Assembly.GetType(t.FullName + "[]");
            System.Array array = (System.Array)Activator.CreateInstance(tarray, result.Count);
            result.ToArray().CopyTo(array, 0);

            return (TDest[])array;
        }
    }
}
