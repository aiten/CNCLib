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

namespace Framework.Tools
{
    using System;
    using System.Collections.Generic;

    public static class ListExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, @"Must not be < 1");
            }

            var      listList = new List<IEnumerable<T>>();
            int      count    = 0;
            IList<T> lastList = null;

            foreach (var element in list)
            {
                if ((count % size) == 0)
                {
                    lastList = new List<T>();
                    listList.Add(lastList);
                }

                lastList.Add(element);
                count++;
            }

            return listList;
        }

        public static IEnumerable<IEnumerable<T>> SplitBefore<T>(this IEnumerable<T> list, Func<T, bool> askSplitBefore)
        {
            var      listList = new List<IEnumerable<T>>();
            IList<T> lastList = null;

            foreach (var element in list)
            {
                if (askSplitBefore(element) || lastList == null)
                {
                    lastList = new List<T>();
                    listList.Add(lastList);
                }

                lastList.Add(element);
            }

            return listList;
        }

        public static IEnumerable<IEnumerable<T>> SplitAfter<T>(this IEnumerable<T> list, Func<T, bool> askSplitAfter)
        {
            var      listList = new List<IEnumerable<T>>();
            IList<T> lastList = null;

            foreach (var element in list)
            {
                if (lastList == null)
                {
                    lastList = new List<T>();
                    listList.Add(lastList);
                }

                lastList.Add(element);

                if (askSplitAfter(element))
                {
                    lastList = null;
                }
            }

            return listList;
        }
    }
}