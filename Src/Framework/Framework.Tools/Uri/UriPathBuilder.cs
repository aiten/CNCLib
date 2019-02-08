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
    public class UriPathBuilder
    {
        public string Api { get; set; }
        public string Path { get; set; }
        public string Query { get; set; }

        public string Build()
        {
            if (string.IsNullOrEmpty(Query))
            {
                return $"{Api}{Path}";
            }
            return $"{Api}{Path}?{Query}";
        }

        public static string Build(string api, string path, string query=null)
        {
            return new UriPathBuilder() { Api = api, Path = path, Query = query }.Build();
        }

        public static string Build(string api, string route, UriQueryBuilder filter)
        {
            return new UriPathBuilder() { Api = api, Path = route, Query = filter.ToString() }.Build();
        }
    }
}