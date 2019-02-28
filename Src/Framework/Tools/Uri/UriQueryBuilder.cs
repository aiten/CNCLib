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

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Tools.Uri
{
    public class UriQueryBuilder
    {
        private List<Tuple<string, object>> _list = new List<Tuple<string, object>>();
        private string                      _old  = "";

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