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
    public class UriBuilder
    {
        public string Api { get; set; }
        public string Route { get; set; }
        public string Filter { get; set; }

        public string Build()
        {
            if (string.IsNullOrEmpty(Filter))
            {
                return $"{Api}{Route}";
            }
            return $"{Api}{Route}?{Filter}";
        }

        public static string Build(string api, string route, string filter=null)
        {
            return new UriBuilder() { Api = api, Route = route, Filter = filter }.Build();
        }

        public static string Build(string api, string route, UriFilterBuilder filter)
        {
            return new UriBuilder() { Api = api, Route = route, Filter = filter.ToString() }.Build();
        }
    }
}