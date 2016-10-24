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
using CNCLib.Repository.Contracts;
using CNCLib.Logic.Converter;
using CNCLib.Logic.Contracts;
using CNCLib.Logic.Contracts.DTO;
using System.Reflection;
using System.Globalization;
using Framework.Tools.Dependency;
using Framework.Tools.Pattern;

namespace CNCLib.Logic
{
	public class ItemController : ControllerBase, IItemController
	{
		public Item Get(int id)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				var item = rep.Get(id);
				if (item == null)
					return null;

				var dto = item.Convert();
				return dto;
			}
		}

		public IEnumerable<Item> GetAll(Type t)
		{
			using (var uow = Dependency.Resolve<IUnitOfWork>())
			using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				var all = rep.Get(GetClassName(t));
				List<Item> l = new List<Item>();
				foreach (var o in all)
				{
					l.Add(o.Convert());
				}
				return l;
			}
		}

		public IEnumerable<Item> GetAll()
		{
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
			{
				var all = rep.Get();
				List<Item> l = new List<Item>();
				foreach (var o in all)
				{
					l.Add(o.Convert());
				}
				return l;
			}
		}

        public object Create(int id)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
            {
                var item = rep.Get(id);

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

		private static void AssignProperty(object obj, Repository.Contracts.Entities.ItemProperty ip, PropertyInfo pi)
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
						Byte[] bytes = Convert.FromBase64String(ip.Value);
						pi.SetValue(obj, bytes);
					}
				}
				else
				{
					throw new NotImplementedException();
				}
			}
		}

		private List<CNCLib.Repository.Contracts.Entities.ItemProperty> GetProperties(int id, object obj)
        {
            Type t = obj.GetType();

            var list = new List<CNCLib.Repository.Contracts.Entities.ItemProperty>();

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
							value = Convert.ToBase64String(bytes);
					}
                    else
                    {
                        throw new NotImplementedException();
                    }
                    var prop = new CNCLib.Repository.Contracts.Entities.ItemProperty() { Name = pi.Name, ItemID = id };
                    if (!string.IsNullOrEmpty(value))
                        prop.Value = value;
                    list.Add(prop);
                }
            }
            return list;
        }

        public int Add(string name, object obj)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
            {
                var list = GetProperties(0, obj);

                var item = new CNCLib.Repository.Contracts.Entities.Item()
                {
                    Name = name,
                    ClassName = GetClassName(obj.GetType()),
                    ItemProperties = list.ToArray()
                };
                rep.Store(item);
                uow.Save();
                return item.ItemID;
            }
        }

        public void Save(int id, string name, object obj)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
            {
                var list = GetProperties(id, obj);

                var item = new CNCLib.Repository.Contracts.Entities.Item()
                {
                    ItemID = id,
                    Name = name,
                    ClassName = GetClassName(obj.GetType()),
                    ItemProperties = list.ToArray()
                };
                rep.Store(item);
                uow.Save();
            }
        }

        public void Delete(int id)
        {
            using (var uow = Dependency.Resolve<IUnitOfWork>())
            using (var rep = Dependency.ResolveRepository<IItemRepository>(uow))
            {
                var item = rep.Get(id);
                if (item!=null)
                    rep.Delete(item);
                uow.Save();
            }
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
