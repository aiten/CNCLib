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

namespace CNCLib.GCode.Generate;

using System.IO;
using System.Xml.Serialization;

using CNCLib.GCode.Generate.Commands;
using CNCLib.GCode.Generate.Load;

public static class GCodeLoadExtensions
{
    public static void WriteGCodeFile(this LoadBase load, StreamWriter sw)
    {
        Command? last  = null;
        var      state = new CommandState();
        foreach (var r in load.Commands)
        {
            var cmds = r.GetGCodeCommands(last?.CalculatedEndPosition, state);

            foreach (string str in cmds)
            {
                sw.WriteLine(str);
            }

            last = r;
        }
    }

    public static void WriteCamBamFile(this LoadBase load, StreamWriter writer)
    {
        var x = new XmlSerializer(typeof(CamBam.CamBam));
        x.Serialize(writer, load.CamBam);
    }

    public static void WriteImportInfoFile(this LoadBase load, StreamWriter writer)
    {
        if (load.Commands.Exists(c => !string.IsNullOrEmpty(c.ImportInfo)))
        {
            load.Commands.ForEach(
                c =>
                {
                    if (!string.IsNullOrEmpty(c.ImportInfo))
                    {
                        writer.WriteLine(c.ImportInfo);
                    }
                });
        }
    }
}