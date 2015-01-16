////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Ports;
using Framework.Logic;

namespace Plotter.Logic
{
    public class Communication : Framework.Logic.HPGLCommunication
    {
        public Communication()
        {
        }

        public void SendFile(string filename,bool singleStep)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                Abort = false;
                String line;
                List<String> lines = new List<string>();
                while ((line = sr.ReadLine()) != null && !Abort)
                {
                    lines.Add(line);
                }          
                SendCommands(lines.ToArray());
            }
        }
    }
}
