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

using Framework.Tools.Pattern;

namespace CNCLib.Repository.Contracts
{
	public interface IItemRepository: IBaseRepository
	{
		Entities.Item[] Get();
		Entities.Item[] Get(string typeidstring);
		Entities.Item Get(int id);
		void Delete(Entities.Item o);
        void Store(Entities.Item o);
	}
}
