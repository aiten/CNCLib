////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

namespace Framework.Contract.Repository.Entities
{
    public class Log
    {
        public int      Id            { get; set; }
        public DateTime LogDate       { get; set; }
        public string   Application   { get; set; }
        public string   Level         { get; set; }
        public string   Message       { get; set; }
        public string   UserName      { get; set; }
        public string   ServerName    { get; set; }
        public string   MachineName   { get; set; }
        public string   Port          { get; set; }
        public string   Url           { get; set; }
        public string   ServerAddress { get; set; }
        public string   RemoteAddress { get; set; }
        public string   Logger        { get; set; }
        public string   Exception     { get; set; }
        public string   StackTrace    { get; set; }
    }
}