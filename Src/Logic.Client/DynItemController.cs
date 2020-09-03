/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) Herbert Aitenbichler

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
*/

namespace CNCLib.Logic.Client
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Threading.Tasks;

    using CNCLib.Logic.Abstraction.DTO;
    using CNCLib.Service.Abstraction;

    using Framework.Pattern;

    public class DynItemController : DisposeWrapper, IDynItemController
    {
        readonly IItemService _itemService;

        public DynItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task<DynItem> Get(int id)
        {
            Item item = await _itemService.Get(id);
            return item == null ? null : Convert(item);
        }

        public async Task<IEnumerable<DynItem>> GetAll(Type t)
        {
            IEnumerable<Item> allItems = await _itemService.GetByClassName(GetClassName(t));
            return Convert(allItems);
        }

        public async Task<IEnumerable<DynItem>> GetAll()
        {
            IEnumerable<Item> allItems = await _itemService.GetAll();
            return Convert(allItems);
        }

        public async Task<object> Create(int id)
        {
            Item item = await _itemService.Get(id);

            if (item == null)
            {
                return null;
            }

            Type t   = Type.GetType(item.ClassName);
            var  obj = Activator.CreateInstance(t ?? throw new InvalidOperationException());

            foreach (ItemProperty ip in item.ItemProperties)
            {
                AssignProperty(obj, ip, t.GetProperty(ip.Name));
            }

            return obj;
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
                else if (pi.PropertyType == typeof(byte))
                {
                    pi.SetValue(obj, byte.Parse(ip.Value));
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
                    {
                        val = int.Parse(ip.Value);
                    }

                    pi.SetValue(obj, val);
                }
                else if (pi.PropertyType == typeof(decimal?))
                {
                    decimal? val = null;
                    if (!string.IsNullOrEmpty(ip.Value))
                    {
                        val = decimal.Parse(ip.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    }

                    pi.SetValue(obj, val);
                }
                else if (pi.PropertyType == typeof(double?))
                {
                    double? val = null;
                    if (!string.IsNullOrEmpty(ip.Value))
                    {
                        val = double.Parse(ip.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
                    }

                    pi.SetValue(obj, val);
                }
                else if (pi.PropertyType.IsEnum)
                {
                    pi.SetValue(obj, Enum.Parse(pi.PropertyType, ip.Value));
                }
                else if (pi.PropertyType == typeof(byte[]))
                {
                    if (!string.IsNullOrEmpty(ip.Value))
                    {
                        byte[] bytes = System.Convert.FromBase64String(ip.Value);
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
                        if (str != null)
                        {
                            value = (string)str;
                        }
                    }
                    else if (pi.PropertyType == typeof(int) || pi.PropertyType == typeof(byte))
                    {
                        value = pi.GetValue(obj).ToString();
                    }
                    else if (pi.PropertyType == typeof(bool))
                    {
                        value = (bool)pi.GetValue(obj) ? "true" : "false";
                    }
                    else if (pi.PropertyType == typeof(decimal))
                    {
                        value = ((decimal)pi.GetValue(obj)).ToString(CultureInfo.InvariantCulture);
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
                        var val = (int?)pi.GetValue(obj);
                        if (val.HasValue)
                        {
                            value = val.Value.ToString();
                        }
                    }
                    else if (pi.PropertyType == typeof(decimal?))
                    {
                        var val = (decimal?)pi.GetValue(obj);
                        if (val.HasValue)
                        {
                            value = val.Value.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else if (pi.PropertyType == typeof(double?))
                    {
                        var val = (double?)pi.GetValue(obj);
                        if (val.HasValue)
                        {
                            value = val.Value.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                    else if (pi.PropertyType.IsEnum)
                    {
                        value = pi.GetValue(obj).ToString();
                    }
                    else if (pi.PropertyType == typeof(byte[]))
                    {
                        var bytes = (byte[])pi.GetValue(obj);
                        if (bytes != null)
                        {
                            value = System.Convert.ToBase64String(bytes);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    var prop = new ItemProperty { Name = pi.Name, ItemId = id };
                    if (!string.IsNullOrEmpty(value))
                    {
                        prop.Value = value;
                    }

                    list.Add(prop);
                }
            }

            return list;
        }

        public async Task<int> Add(string name, object obj)
        {
            return await _itemService.Add(ConvertToItem(name, obj, 0));
        }

        public async Task Save(int id, string name, object obj)
        {
            await _itemService.Update(ConvertToItem(name, obj, id));
        }

        public async Task Delete(int id)
        {
            var item = await _itemService.Get(id);
            if (item != null)
            {
                await _itemService.Delete(item);
            }
        }

        private Item ConvertToItem(string name, object obj, int id)
        {
            var list = GetProperties(id, obj);
            var item = new Item
            {
                ItemId         = id,
                Name           = name,
                ClassName      = GetClassName(obj.GetType()),
                ItemProperties = list.ToArray()
            };
            return item;
        }

        private static IEnumerable<DynItem> Convert(IEnumerable<Item> allItems)
        {
            if (allItems == null)
            {
                return null;
            }

            var l = new List<DynItem>();
            foreach (var o in allItems)
            {
                l.Add(new DynItem { ItemId = o.ItemId, Name = o.Name });
            }

            return l;
        }

        private static DynItem Convert(Item item)
        {
            return new DynItem { ItemId = item.ItemId, Name = item.Name };
        }

        public static string GetClassName(Type t)
        {
            string[] names = t.AssemblyQualifiedName.Split(',');
            return names[0].Trim() + "," + names[1].Trim();
        }
    }
}