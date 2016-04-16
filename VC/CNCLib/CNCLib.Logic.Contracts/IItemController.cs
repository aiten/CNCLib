////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
using System.Collections.Generic;

namespace CNCLib.Logic.Contracts
{
	public interface IItemController : IDisposable
	{
		IEnumerable<DTO.Item> GetAll();

		IEnumerable<DTO.Item> GetAll(Type t);

		object Create(int id);

        int Add(string name, object value);
        void Save(int id, string name, object value);
        void Delete(int id);
    }
}
