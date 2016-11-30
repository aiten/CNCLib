////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

  CNCLib is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  CNCLib is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.
  http://www.gnu.org/licenses/
*/

using System;
using System.Collections.Generic;
using Framework.Logic;
using CNCLib.Logic.Contracts.DTO;
using System.Reflection;
using System.Linq;
using System.Globalization;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;
using CNCLib.Logic.Contracts;
using CNCLib.ServiceProxy;
using System.Threading.Tasks;

namespace CNCLib.Logic.Client
{
	public class DynItemController : IDynItemController
	{
		public Task<DynItem> Get(int id)
		{
			return Task.Run(() =>
			{
				using (var service = Dependency.Resolve<IItemService>())
				{
					var item = service.Get(id).Result;
					if (item == null)
						return null;

					return Convert(item);
				}
			});
		}

		public Task<IEnumerable<DynItem>> GetAll(Type t)
		{
			return Task.Run(() =>
			{
				using (var service = Dependency.Resolve<IItemService>())
				{
					var allitems = service.GetByClassName(GetClassName(t)).Result;
					return (IEnumerable<DynItem>) Convert(allitems);
				}
			});
		}

		public Task<IEnumerable<DynItem>> GetAll()
		{
			return Task.Run(() =>
			{
				using (var service = Dependency.Resolve<IItemService>())
				{
					var allitems = service.GetAll().Result;
					return Convert(allitems);
				}
			});
		}

		public object Create(int id)
        {
			using (var service = Dependency.Resolve<IItemService>())
			{
				var item = service.Get(id).Result;

				if (item == null)
					return null;

				Type t = Type.GetType(item.ClassName);
				var obj = Activator.CreateInstance(t);

				foreach (var ip in item.ItemProperties)
				{
					AssignProperty(obj, ip, t.GetProperty(ip.Name));
				}
				return obj;
			}
        }

		private static void AssignProperty(object obj, ItemProperty ip, PropertyInfo pi)
		{
			if (pi != null && pi.CanWrite)
			{
				if (pi.PropertyType == typeof(string))
				{
					pi.SetValue(obj, ip.Value);
				}
				else if (pi.PropertyType == typeof(int))
				{
					pi.SetValue(obj, int.Parse(ip.Value));
				}
				else if (pi.PropertyType == typeof(Byte))
				{
					pi.SetValue(obj, Byte.Parse(ip.Value));
				}
				else if (pi.PropertyType == typeof(bool))
				{
					pi.SetValue(obj, ip.Value == "true");
				}
				else if (pi.PropertyType == typeof(decimal))
				{
					pi.SetValue(obj, decimal.Parse(ip.Value, CultureInfo.InvariantCulture));
				}
				else if (pi.PropertyType == typeof(float))
				{
					pi.SetValue(obj, double.Parse(ip.Value, CultureInfo.InvariantCulture));
				}
				else if (pi.PropertyType == typeof(double))
				{
					pi.SetValue(obj, double.Parse(ip.Value, CultureInfo.InvariantCulture));
				}
				else if (pi.PropertyType == typeof(int?))
				{
					int? val = null;
					if (!string.IsNullOrEmpty(ip.Value))
						val = int.Parse(ip.Value);

					pi.SetValue(obj, val);
				}
				else if (pi.PropertyType == typeof(decimal?))
				{
					decimal? val = null;
					if (!string.IsNullOrEmpty(ip.Value))
						val = decimal.Parse(ip.Value, System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

					pi.SetValue(obj, val);
				}
				else if (pi.PropertyType == typeof(double?))
				{
					double? val = null;
					if (!string.IsNullOrEmpty(ip.Value))
						val = double.Parse(ip.Value, System.Globalization.NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

					pi.SetValue(obj, val);
				}
				else if (pi.PropertyType.IsEnum)
				{
					pi.SetValue(obj, Enum.Parse(pi.PropertyType, ip.Value));
				}
				else if (pi.PropertyType == typeof(Byte[]))
				{
					if (!string.IsNullOrEmpty(ip.Value))
					{
						Byte[] bytes = System.Convert.FromBase64String(ip.Value);
						pi.SetValue(obj, bytes);
					}
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}

		private List<ItemProperty> GetProperties(int id, object obj)
        {
            Type t = obj.GetType();

            var list = new List<ItemProperty>();

            foreach (PropertyInfo pi in t.GetProperties())
            {
                if (pi.CanWrite && pi.CanRead)
                {
                    string value = null;
                    if (pi.PropertyType == typeof(string))
                    {
						object str = pi.GetValue(obj);
						if (str!=null)
							value =  (string) str;
                    }
					else if (
						pi.PropertyType == typeof(int) ||
						pi.PropertyType == typeof(Byte))
					{
						value = pi.GetValue(obj).ToString();
					}
					else if (pi.PropertyType == typeof(bool))
                    {
                        value = (bool) pi.GetValue(obj) ? "true" : "false";
                    }
                    else if (pi.PropertyType == typeof(decimal))
                    {
                        value = ((decimal) pi.GetValue(obj)).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (pi.PropertyType == typeof(float))
                    {
                        value = ((float)pi.GetValue(obj)).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (pi.PropertyType == typeof(double))
                    {
                        value = ((double)pi.GetValue(obj)).ToString(CultureInfo.InvariantCulture);
                    }
                    else if (pi.PropertyType == typeof(int?))
                    {
                        int? val = (int?) pi.GetValue(obj);
                        if (val.HasValue)
                            value = val.Value.ToString();
                    }
                    else if (pi.PropertyType == typeof(decimal?))
                    {
                        decimal? val = (decimal?)pi.GetValue(obj);
                        if (val.HasValue)
                            value = val.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (pi.PropertyType == typeof(double?))
                    {
                        double? val = (double?)pi.GetValue(obj);
                        if (val.HasValue)
                            value = val.Value.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (pi.PropertyType.IsEnum)
                    {
                        value = pi.GetValue(obj).ToString();
                    }
					else if (pi.PropertyType == typeof(Byte[]))
					{
						Byte[] bytes = (Byte[]) pi.GetValue(obj);
						if (bytes != null)
							value = System.Convert.ToBase64String(bytes);
					}
                    else
                    {
                        throw new NotImplementedException();
                    }
                    var prop = new ItemProperty() { Name = pi.Name, ItemID = id };
                    if (!string.IsNullOrEmpty(value))
                        prop.Value = value;
                    list.Add(prop);
                }
            }
            return list;
        }

        public Task<int> Add(string name, object obj)
		{
			return Task.Run(() =>
			{
				using (var service = Dependency.Resolve<IItemService>())
				{
					return service.Add(ConvertToItem(name, obj, 0));
				}
			});
		}

		public Task Save(int id, string name, object obj)
        {
			return Task.Run(() =>
			{
				using (var service = Dependency.Resolve<IItemService>())
				{
					service.Update(ConvertToItem(name, obj, id));
				}
			});
        }

        public Task Delete(int id)
        {
			return Task.Run(() =>
			{
				using (var service = Dependency.Resolve<IItemService>())
				{
					var item = service.Get(id).Result;
					if (item != null)
						service.Delete(item);
				}
			});
        }

		private Item ConvertToItem(string name, object obj, int id)
		{
			var list = GetProperties(id, obj);
			var item = new CNCLib.Logic.Contracts.DTO.Item()
			{
				ItemID = id,
				Name = name,
				ClassName = GetClassName(obj.GetType()),
				ItemProperties = list.ToArray()
			};
			return item;
		}

		private static IEnumerable<DynItem> Convert(IEnumerable<Item> allitems)
		{
			List<DynItem> l = new List<DynItem>();
			foreach (var o in allitems)
			{
				l.Add(new DynItem() { ItemID = o.ItemID, Name = o.Name });
			}
			return l;
		}
		private static DynItem Convert(Item item)
		{
			return new DynItem() { ItemID = item.ItemID, Name = item.Name };
		}

		private static string GetClassName(Type t)
		{
			string[] names = t.AssemblyQualifiedName.Split(',');
			return names[0].Trim() + "," + names[1].Trim();
		}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~ItemController() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		void IDisposable.Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

        #endregion
    }
}
