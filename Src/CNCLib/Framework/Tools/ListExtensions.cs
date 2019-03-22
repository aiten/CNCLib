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