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

using System.IO;

using CNCLib.GCode.Load;
using CNCLib.Logic.Abstraction.DTO;

using Microsoft.Extensions.Logging;

namespace CNCLib.WebAPI.Controllers
{
    public class GCodeLoadHelper
    {
        public static LoadBase CallLoad(LoadOptions opt)
        {
            if (opt.FileContent == null)
            {
                return CallLoadWithFileName(opt);
            }

            return CallLoadWithContent(opt);
        }

        private static LoadBase CallLoadWithFileName(LoadOptions opt)
        {
            LoadBase load = LoadBase.Create(opt);

            if (load == null)
            {
                return null;
            }

            load.Load();
            return load;
        }

        private static LoadBase CallLoadWithContent(LoadOptions opt)
        {
            var filename = opt.FileName;
            var fileContent = opt.FileContent;
            
            string pathFileName = Path.GetFileName(filename);
            string tmpFile      = Path.GetTempPath() + pathFileName;

            opt.FileName             = tmpFile;
            opt.ImageWriteToFileName = null;

            try
            {
                File.WriteAllBytes(tmpFile, fileContent);

                LoadBase load = LoadBase.Create(opt);

                if (load == null)
                {
                    return null;
                }

                load.Load();
                return load;
            }
            finally
            {
                File.Delete(tmpFile);
            }
        }
    }
}