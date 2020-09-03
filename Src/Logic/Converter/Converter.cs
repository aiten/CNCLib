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

namespace CNCLib.Logic.Converter
{
    using System.Collections.Generic;

    using AutoMapper;

    using CNCLib.Logic.Abstraction.DTO;

    internal static class Converter
    {
        public static Machine ToDto(this Repository.Abstraction.Entities.Machine from, IMapper mapper)
        {
            return mapper.Map<Machine>(from);
        }

        public static Repository.Abstraction.Entities.Machine ToEntity(this Machine from, IMapper mapper)
        {
            return mapper.Map<Repository.Abstraction.Entities.Machine>(from);
        }

        public static Item ToDto(this Repository.Abstraction.Entities.Item from, IMapper mapper)
        {
            return mapper.Map<Item>(from);
        }

        public static Repository.Abstraction.Entities.Item ToEntity(this Item from, IMapper mapper)
        {
            return mapper.Map<Repository.Abstraction.Entities.Item>(from);
        }

        public static IEnumerable<Item> ToDto(this IEnumerable<Repository.Abstraction.Entities.Item> items, IMapper mapper)
        {
            return mapper.Map<IEnumerable<Item>>(items);
        }
    }
}