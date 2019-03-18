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

namespace Framework.Tools.Uri
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class UriQueryBuilder
    {
        private readonly List<Tuple<string, object>> _list = new List<Tuple<string, object>>();
        private readonly string                      _old  = "";

        public UriQueryBuilder()
        {
        }

        public UriQueryBuilder(string old)
        {
            _old = old;
        }

        public UriQueryBuilder(UriQueryBuilder old)
        {
            _list.AddRange(old._list);
        }

        public UriQueryBuilder AddRange<T>(string filterName, IList<T> valueList)
        {
            _list.AddRange(valueList.Select(val => new Tuple<string, object>(filterName, val)));
            return this;
        }

        public UriQueryBuilder Add<T>(string filterName, T val)
        {
            _list.Add(new Tuple<string, object>(filterName, val));
            return this;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(_old);

            foreach (var filter in _list)
            {
                if (sb.Length != 0)
                {
                    sb.Append(@"&");
                }

                sb.Append(filter.Item1);
                sb.Append(@"=");
                sb.Append(filter.Item2.ToUriAsQuery());
            }

            return sb.ToString();
        }
    }
}