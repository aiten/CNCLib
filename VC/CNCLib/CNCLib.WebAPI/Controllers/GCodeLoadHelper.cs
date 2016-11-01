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

using System.IO;
using CNCLib.GCode.Load;
using CNCLib.Logic.Contracts.DTO;

namespace CNCLib.WebAPI.Controllers
{
	public class GCodeLoadHelper
	{
		public static LoadBase CallLoad(string filename, byte[] filecontent, LoadOptions opt)
		{
			string pathfilename = Path.GetFileName(filename);
			string tmpfile = Path.GetTempPath() + pathfilename;

			opt.FileName = tmpfile;
			opt.ImageWriteToFileName = null;

			try
			{
				File.WriteAllBytes(tmpfile, filecontent);

				LoadBase load = LoadBase.Create(opt);

				if (load == null)
					return null;

				load.Load();
				return load;
			}
			finally
			{
				File.Delete(tmpfile);
			}
		}
	}
}