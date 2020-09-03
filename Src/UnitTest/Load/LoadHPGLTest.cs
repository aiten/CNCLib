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

namespace CNCLib.UnitTest.Load
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CNCLib.GCode.Generate.Commands;
    using CNCLib.GCode.Generate.Load;
    using CNCLib.Logic.Abstraction.DTO;

    using FluentAssertions;

    using Xunit;

    public class LoadHpglTest
    {
        [Fact]
        public void LoadHpgl00()
        {
            var loadInfo = new LoadOptions
            {
                LoadType    = LoadOptions.ELoadType.Hpgl,
                AutoScale   = false,
                FileContent = Encoding.ASCII.GetBytes("IN;PU0,0")
            };

            var load = LoadBase.Create(loadInfo);

            load.Load();

            var list = load.Commands.Where(e => e is G00Command || e is G01Command);

            list.Count().Should().Be(2);

            list.First().Should().BeOfType<G01Command>();      // G0 F500
            list.ElementAt(1).Should().BeOfType<G00Command>(); // G0 z1

            //list.ElementAt(2).Should().BeOfType<G00Command>();    // G0 0,0 => PU0,0, is skipped by new version
        }

        [Fact]
        public void LoadHpglSkipPU()
        {
            var loadInfo = new LoadOptions
            {
                LoadType    = LoadOptions.ELoadType.Hpgl,
                AutoScale   = false,
                PenMoveType = LoadOptions.PenType.CommandString,
                MoveSpeed   = 499,
                FileContent = Encoding.ASCII.GetBytes("IN;PU1000,100;PU0,0;PD400,400;PU0,0")

                // leading PU1000,100 and trailing PU0,0 is skipped
            };

            var load = LoadBase.Create(loadInfo);

            load.Load();

            string[] gcode =
            {
                "G1 F499", "M107", "G0 X0 Y0", "M106 S255", "G1 X10 Y10", "M107", "M5" // ShutdownCommands
            };

            var list = load.Commands.Where(IsGCommand);

            CheckGCode(list, gcode);
        }

        [Fact]
        public void LoadHpglGengraveParam()
        {
            var loadInfo = new LoadOptions
            {
                LoadType              = LoadOptions.ELoadType.Hpgl,
                AutoScale             = false,
                PenMoveType           = LoadOptions.PenType.ZMove,
                EngravePosInParameter = true,
                FileContent           = Encoding.ASCII.GetBytes("IN;PU0,0;PD400,400")
            };

            var load = LoadBase.Create(loadInfo);

            load.Load();

            string[] gcode =
            {
                "G1 F500", "G0 Z#1", "G0 X0 Y0", "G1 Z#2", "G1 X10 Y10", "G0 Z#1"
            };

            var list = load.Commands.Where(e => e is G00Command || e is G01Command);

            CheckGCode(list, gcode);
        }

        [Fact]
        public void LoadHpglGengraveNoParamAndSpeed()
        {
            var loadInfo = new LoadOptions
            {
                LoadType              = LoadOptions.ELoadType.Hpgl,
                AutoScale             = false,
                PenMoveType           = LoadOptions.PenType.ZMove,
                EngravePosInParameter = false,
                EngravePosUp          = 1.23m,
                EngravePosDown        = 0.12m,
                EngraveDownSpeed      = 123,
                MoveSpeed             = 499,
                FileContent           = Encoding.ASCII.GetBytes("IN;PU0,0;PD400,400;PD800,400")
            };

            var load = LoadBase.Create(loadInfo);

            load.Load();

            string[] gcode =
            {
                "G1 F499", "G0 Z1.23", "G0 X0 Y0", "G1 Z0.12 F123", "G1 X10 Y10 F499", "G1 X20 Y10", "G0 Z1.23"
            };

            var list = load.Commands.Where(e => e is G00Command || e is G01Command);

            CheckGCode(list, gcode);
        }

        static bool IsGCommand(Command e)
        {
            return e is G00Command || e is G01Command || e is M3Command || e is M4Command || e is M5Command || e is M106Command || e is M107Command || e is MxxCommand;
        }

        [Fact]
        public void LoadHpglLaser()
        {
            var loadInfo = new LoadOptions
            {
                LoadType    = LoadOptions.ELoadType.Hpgl,
                AutoScale   = false,
                PenMoveType = LoadOptions.PenType.CommandString,
                MoveSpeed   = 499,
                FileContent = Encoding.ASCII.GetBytes("IN;PU0,0;PD400,400;PD800,400;PU800,800;PD1200,1200")
            };

            var load = LoadBase.Create(loadInfo);

            load.Load();

            string[] gcode =
            {
                "G1 F499", "M107", "G0 X0 Y0", "M106 S255", "G1 X10 Y10", "G1 X20 Y10", "M107", "G0 X20 Y20", "M106", "G1 X30 Y30", "M107", "M5" // ShutdownCommands
            };

            var list = load.Commands.Where(IsGCommand);

            CheckGCode(list, gcode);
        }

        [Fact]
        public void LoadHpglConvertOpenLine()
        {
            var loadInfo = new LoadOptions
            {
                LoadType    = LoadOptions.ELoadType.Hpgl,
                AutoScale   = false,
                PenMoveType = LoadOptions.PenType.CommandString,
                MoveSpeed   = 499,
                ConvertType = LoadOptions.ConvertTypeEnum.InvertLineSequence,
                FileContent = Encoding.ASCII.GetBytes("IN;PU0,0;PD400,400;PD800,400;PU800,800;PD1200,1200;PU;SP0;PU0,0")
            };

            var load = LoadBase.Create(loadInfo);

            load.Load();

            string[] gcode =
            {
                "G1 F499", "M107", "G0 X0 Y0", "M106 S255", "G1 X10 Y10", "G1 X20 Y10", "M107", "G0 X20 Y20", "M106", "G1 X30 Y30", "M107", "M5" // ShutdownCommands
            };

            var list = load.Commands.Where(IsGCommand);

            CheckGCode(list, gcode);
        }

        [Fact]
        public void LoadHpglConvertClosedLine()
        {
            var loadInfo = new LoadOptions
            {
                LoadType    = LoadOptions.ELoadType.Hpgl,
                AutoScale   = false,
                PenMoveType = LoadOptions.PenType.CommandString,
                MoveSpeed   = 499,
                CutterSize  = 0,
                ConvertType = LoadOptions.ConvertTypeEnum.InvertLineSequence,
                FileContent = Encoding.ASCII.GetBytes(
                    "IN;" + "PU0,0;PD0,400,400,400,400,0,0,0;" + "PU50,50;PD350,50,350,350,50,350,50,50;" + "PU100,100;PD300,100,300,300,100,300,100,100;" +
                    "PU150,150;PD250,150,250,250,150,250,150,150;" + "PU;SP0")
            };

            var load = LoadBase.Create(loadInfo);

            load.Load();

            string[] gcode =
            {
                "G1 F499", "M107", "G0 X3.75 Y3.75", "M106 S255", "G1 X6.25 Y3.75", "G1 X6.25 Y6.25", "G1 X3.75 Y6.25", "G1 X3.75 Y3.75", "M107", "G0 X2.5 Y2.5", "M106", "G1 X7.5 Y2.5",
                "G1 X7.5 Y7.5", "G1 X2.5 Y7.5", "G1 X2.5 Y2.5", "M107", "G0 X1.25 Y1.25", "M106", "G1 X8.75 Y1.25", "G1 X8.75 Y8.75", "G1 X1.25 Y8.75", "G1 X1.25 Y1.25", "M107", "G0 X0 Y0", "M106",
                "G1 X0 Y10", "G1 X10 Y10", "G1 X10 Y0", "G1 X0 Y0", "M107", "M5"
            };

            var list = load.Commands.Where(IsGCommand);

            CheckGCode(list, gcode);
        }

        private static void CheckGCode(IEnumerable<Command> list, string[] expectGcode)
        {
            list.Count().Should().Be(expectGcode.Length);

            int idx = 0;
            foreach (var command in list)
            {
                list.ElementAt(idx).GetGCodeCommands(null, null)[0].Should().BeEquivalentTo(expectGcode[idx]);
                idx++;
            }
        }
    }
}