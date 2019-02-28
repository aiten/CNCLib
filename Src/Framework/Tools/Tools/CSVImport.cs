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

namespace Framework.Tools.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class CSVImport<T> : CSVImportBase where T : new()
    {
        public IList<T> Read(string fileName)
        {
            var lines = ReadStringMatrixFromCsv(fileName, false);
            return MapTo(lines);
        }

        public IList<T> MapTo(IList<IList<string>> lines)
        {
            // first line is columnLineHeader!!!!

            var  list  = new List<T>();
            var  props = GetPropertyMapping(lines[0]);
            bool first = true;

            if (props.Any(prop => prop == null))
            {
                throw new ArgumentException($"Column cannot be mapped: {string.Join(", ", lines[0].Where((p, idx) => props[idx] == null))}");
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

        private PropertyInfo[] GetPropertyMapping(IList<string> columnNames)
        {
            Type t = typeof(T);
            return columnNames.Select((columnName) => t.GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)).ToArray();
        }

        private T Map(IList<string> line, PropertyInfo[] props)
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
                    pi.SetValue(obj, ExcelDateOrDateTime(valueAsString));
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
                else if (pi.PropertyType == typeof(double?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (double?)null : ExcelDouble(valueAsString));
                }
                else if (pi.PropertyType == typeof(decimal?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (decimal?)null : ExcelDecimal(valueAsString));
                }
                else if (pi.PropertyType == typeof(DateTime?))
                {
                    pi.SetValue(obj, string.IsNullOrEmpty(valueAsString) ? (DateTime?)null : ExcelDateOrDateTime(valueAsString));
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