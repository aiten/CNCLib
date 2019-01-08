////////////////////////////////////////////////////////
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

namespace CNCLib.Test.Logic
{
    public class DynItemManagerTestClass
    {
        public int      IntProperty         { get; set; }
        public string   StringProperty      { get; set; }
        public double   DoubleProperty      { get; set; }
        public decimal  DecimalProperty     { get; set; }
        public int?     IntNullProperty     { get; set; }
        public double?  DoubleNullProperty  { get; set; }
        public decimal? DecimalNullProperty { get; set; }
    }
}