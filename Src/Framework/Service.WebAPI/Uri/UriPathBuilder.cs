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

using System;
using System.Linq;
using System.Web;

namespace Framework.Service.WebAPI.Uri
{
    public class UriPathBuilder
    {
        public string Path { get; set; }
        public string Query { get; set; }

        public string Build()
        {
            if (string.IsNullOrEmpty(Query))
            {
                return $"{Path}";
            }

            return $"{Path}?{Query}";
        }

        public UriPathBuilder AddPath(string path)
        {
            if (string.IsNullOrEmpty(Path))
            {
                Path = path;
            }
            else
            {
                if (Path[Path.Length - 1] != '/')
                {
                    Path += '/';
                }

                Path += path;
            }

            return this;
        }

        public UriPathBuilder AddPath(string[] pathElements)
        {
            return AddPath(string.Join("/", pathElements.Select(HttpUtility.UrlEncode)));
        }

        public UriPathBuilder AddQuery(string query)
        {
            if (!string.IsNullOrEmpty(Query))
            {
                throw new ArgumentException();
            }

            Query = query;
            return this;
        }

        public UriPathBuilder AddQuery(UriQueryBuilder filter)
        {
            return AddQuery(filter.ToString());
        }
    }
}