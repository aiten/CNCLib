////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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


namespace CNCLib.Repository.Contracts.Entities
{
	public class Configuration
	{
		public string Group { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }
        public int? UserID { get; set; }
        public virtual User User { get; set; }


        public Configuration()
		{
		}
		public Configuration(string group, string name, object value)
		{
			Name = name;
			Group = group;
			Value = value.ToString();

			Type = value.GetType().ToString();
		}
	}
}
