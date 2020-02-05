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

using System.Collections.Generic;

namespace CNCLib.Repository.Abstraction.Entities
{
    public class Machine
    {
        public int  MachineId { get; set; }
        public int  UserId    { get; set; }
        public User User      { get; set; }

        public string Name                 { get; set; }
        public string SerialServer         { get; set; }
        public int    SerialServerPort     { get; set; }
        public string SerialServerProtocol { get; set; }
        public string SerialServerUser     { get; set; }
        public string SerialServerPassword { get; set; }
        public string ComPort              { get; set; }
        public int    Axis                 { get; set; }
        public int    BaudRate             { get; set; }
        public bool   DtrIsReset           { get; set; }

        public bool NeedDtr { get; set; } // do not delete the column because SQLite limitation of drop columns

        public decimal SizeX          { get; set; }
        public decimal SizeY          { get; set; }
        public decimal SizeZ          { get; set; }
        public decimal SizeA          { get; set; }
        public decimal SizeB          { get; set; }
        public decimal SizeC          { get; set; }
        public int     BufferSize     { get; set; }
        public bool    CommandToUpper { get; set; }
        public decimal ProbeSizeX     { get; set; }
        public decimal ProbeSizeY     { get; set; }
        public decimal ProbeSizeZ     { get; set; }
        public decimal ProbeDistUp    { get; set; }
        public decimal ProbeDist      { get; set; }
        public decimal ProbeFeed      { get; set; }
        public bool    SDSupport      { get; set; }
        public bool    Spindle        { get; set; }
        public bool    Coolant        { get; set; }
        public bool    Laser          { get; set; }
        public bool    Rotate         { get; set; }
        public int     CommandSyntax  { get; set; }
        public int     WorkOffsets    { get; set; }

        public ICollection<MachineCommand>     MachineCommands     { get; set; }
        public ICollection<MachineInitCommand> MachineInitCommands { get; set; }
    }
}