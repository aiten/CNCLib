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

using CNCLib.GCode.Parser;

using Framework.Drawing;
using Framework.Parser;

namespace CNCLib.GCode.Commands
{
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

        public override string[] GetGCodeCommands(Point3D startFrom, CommandState state)
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

        public override void ReadFrom(CommandStream stream)
        {
            int saveIndex = stream.PushIdx();

            stream.Next();

            if (stream.IsNumber())
            {
                int parameter = stream.GetInt();

                if (parameter >= 0 && stream.SkipSpacesToUpper() == '=')
                {
                    stream.Next();
                    ParameterNo = parameter;
                }
                else
                {
                    // error => do not analyze line
                    stream.PopIdx(saveIndex);
                }
            }

            ReadFromToEnd(stream);
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
            var lineStream       = new CommandStream() { Line                              = GCodeAdd };
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
}