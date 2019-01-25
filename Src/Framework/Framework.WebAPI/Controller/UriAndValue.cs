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

namespace Framework.WebAPI.Controller
{
	using System.Collections.Generic;
	using System.Linq;

    public class UriAndValue<TDto> where TDto : class
    {
        public string Uri { get; set; }

        public TDto Value { get; set; }
    }

    public static class UriAndValueExtension
    {
        public static UrisAndValues<T> ToUrisAndValues<T>(this IEnumerable<UriAndValue<T>> from) where T : class
        {
            var fromArray = from.ToArray();
            return new UrisAndValues<T>() { Value = fromArray.Select(v => v.Value), Uri = fromArray.Select(u => u.Uri) };
        }
    }
}