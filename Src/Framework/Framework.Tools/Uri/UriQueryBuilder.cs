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

namespace Framework.Tools.Uri
{
    public class UriQueryBuilder
    {
        private StringBuilder _sb = new StringBuilder();

        public UriQueryBuilder()
        {
        }

        public UriQueryBuilder(string old)
        {
            _sb.Append(old);
        }

        public string ToUriDate(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd");
        }

        private void AddNextFilter()
        {
            if (_sb.Length != 0)
            {
                _sb.Append("&");
            }
        }

        public UriQueryBuilder AddRange<T>(string filterName, IList<T> valueList)
        {
            AddNextFilter();
            _sb.Append($"{filterName}={ string.Join($"&{filterName}=", valueList) }");
            return this;
        }

        public UriQueryBuilder Add(string filterName, DateTime dateValue)
        {
            AddNextFilter();
            _sb.Append($"{filterName}={ ToUriDate(dateValue) }");
            return this;
        }
        public UriQueryBuilder Add<T>(string filterName, T val)
        {
            AddNextFilter();
            _sb.Append($"{filterName}={val}");
            return this;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}