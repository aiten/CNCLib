/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2019 Herbert Aitenbichler

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

namespace Framework.Tools.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class CSVImport<T> : CSVImportBase where T : new()
    {
        public IEnumerable<T> Read(string fileName)
        {
            var lines = ReadStringMatrixFromCsv(fileName, false);
            return MapTo(lines);
        }

        public IEnumerable<T> MapTo(string[][] lines)
        {
            // first line is columnLineHeader!!!!

            var  list  = new List<T>();
            var  props = GetPropertyMapping(lines[0]);
            bool first = true;

            if (props.Any(prop => prop == null))
            {
                throw new ArgumentException("Coloumn cannot be mapped");
            }

            foreach (var line in lines)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    list.Add(Map(line, props));
                }
            }

            return list;
        }

        private PropertyInfo[] GetPropertyMapping(string[] columnNames)
        {
            Type t = typeof(T);
            return columnNames.Select((columnName) => t.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)).ToArray();
        }

        private T Map(string[] line, PropertyInfo[] props)
        {
            var newT = new T();
            int idx  = 0;

            foreach (var column in line)
            {
                AssignProperty(newT, column, props[idx++]);
            }

            return newT;
        }

        private void AssignProperty(object obj, string valueAsString, PropertyInfo pi)
        {
            if (pi != null && pi.CanWrite)
            {
                if (pi.PropertyType == typeof(string))
                {
                    pi.SetValue(obj, ExcelString(valueAsString));
                }
                else if (pi.PropertyType == typeof(int))
                {
                    pi.SetValue(obj, ExcelInt(valueAsString));
                }
                else if (pi.PropertyType == typeof(short))
                {
                    pi.SetValue(obj, ExcelShort(valueAsString));
                }
                else if (pi.PropertyType == typeof(decimal))
                {
                    pi.SetValue(obj, ExcelDecimal(valueAsString));
                }
                else if (pi.PropertyType == typeof(byte))
                {
                    pi.SetValue(obj, ExcelByte(valueAsString));
                }
                else if (pi.PropertyType == typeof(bool))
                {
                    pi.SetValue(obj, ExcelBool(valueAsString));
                }
                else if (pi.PropertyType == typeof(DateTime))
                {
                    pi.SetValue(obj, ExcelDateYMD(valueAsString));
                }
                else if (pi.PropertyType == typeof(double))
                {
                    pi.SetValue(obj, ExcelDouble(valueAsString));
                }
                else if (pi.PropertyType == typeof(int?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (int?)null : ExcelInt(valueAsString));
                }
                else if (pi.PropertyType == typeof(short?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (short?)null : ExcelShort(valueAsString));
                }
                else if (pi.PropertyType == typeof(byte?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (byte?)null : ExcelByte(valueAsString));
                }
                else if (pi.PropertyType == typeof(decimal?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (decimal?)null : ExcelDecimal(valueAsString));
                }
                else if (pi.PropertyType == typeof(DateTime?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (DateTime?)null : ExcelDateYMD(valueAsString));
                }
                /*
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
                */
                else
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}