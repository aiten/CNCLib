﻿/*
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

namespace CNCLib.GCode.Generate.Commands;

using System.Collections.Generic;

using CNCLib.GCode.Generate.Parser;

using Framework.Drawing;
using Framework.Parser;

[IsGCommand("#")]
public class SetParameterCommand : Command
{
    #region crt + factory

    public SetParameterCommand()
    {
        Code = "#";
    }

    public int    ParameterNo    { get; set; } = -1;
    public double ParameterValue { get; private set; }

    #endregion

    #region GCode

    public override IEnumerable<string> GetGCodeCommands(Point3D? startFrom, CommandState? state)
    {
        string[] ret;
        if (ParameterNo >= 0)
        {
            ret = new[]
            {
                GCodeLineNumber(" ") + Code + ParameterNo.ToString() + " =" + GCodeAdd
            };
        }
        else
        {
            ret = new[]
            {
                GCodeLineNumber(" ") + GCodeAdd
            };
        }

        return ret;
    }

    #endregion

    public override void SetCommandState(CommandState state)
    {
        base.SetCommandState(state);

        if (ParameterNo >= 0)
        {
            state.ParameterValues[ParameterNo] = ParameterValue;
        }
    }

    #region Serialization

    public override void ReadFrom(Parser parser)
    {
        int saveIndex = parser.PushPosition();

        parser.Next();

        if (parser.IsNumber())
        {
            int parameter = parser.GetInt();

            if (parameter >= 0 && parser.SkipSpacesToUpper() == '=')
            {
                parser.Next();
                ParameterNo = parameter;
            }
            else
            {
                // error => do not analyze line
                parser.PopPosition(saveIndex);
            }
        }

        ReadFromToEnd(parser);
    }

    public override void UpdateCalculatedEndPosition(CommandState state)
    {
        if (ParameterNo >= 0 && EvaluateParameterValue(state, out double paramValue))
        {
            ParameterValue = paramValue;
            SetCommandState(state);
        }

        base.UpdateCalculatedEndPosition(state);
    }

    private bool EvaluateParameterValue(CommandState state, out double paramValue)
    {
        var lineStream       = new ParserStreamReader() { Line                         = GCodeAdd ?? string.Empty };
        var expressionParser = new GCodeExpressionParser(lineStream) { ParameterValues = state.ParameterValues };
        expressionParser.Parse();
        if (expressionParser.IsError())
        {
            paramValue = 0;
            return false;
        }

        paramValue = expressionParser.Answer;
        return true;
    }

    #endregion

    #region Draw

    #endregion
}