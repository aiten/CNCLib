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
*/
////////////////////////////////////////////////////////

using System.Collections.Generic;

using Framework.Parser;

////////////////////////////////////////////////////////

namespace CNCLib.GCode.Parser
{
    public class GCodeExpressionParser : ExpressionParser
    {
        public GCodeExpressionParser(CommandStream reader) : base(reader)
        {
            LeftParenthesis  = '[';
            RightParenthesis = ']';
        }

        public Dictionary<int, double> ParameterValues { get; set; }

        protected override void ScannNextToken()
        {
            char ch = _reader.NextChar;
            while (ch != 0)
            {
                if (ch == ';' || ch == '(') // comment
                {
                    ch = new GCodeParser(_reader).SkipSpacesOrComment();
                    Error("NotImplemented yet");
                }
                else
                {
                    break;
                }
            }

            if (ch == '\0')
            {
                _state._detailtoken = ETokenType.EndOfLineSy;
                return;
            }

            base.ScannNextToken();
        }

        protected override string ReadIdent()
        {
            // read variable name of gcode : #1 or #<_x>

            var idx = _reader.PushIdx();

            char ch = _reader.NextChar;
            if (ch == '#')
            {
                // start of GCODE variable => format #1 or #<_x>
                _reader.Next();
                _state._number = _reader.GetInt();
            }

            _reader.PopIdx(idx);
            return base.ReadIdent();
        }

        protected override bool IsIdentStart(char ch)
        {
            return ch == '#' || base.IsIdentStart(ch);
        } // start of function or variable

        protected override bool EvalVariable(string varName, ref double answer)
        {
            if (varName[0] == '#')
            {
                // assigned in ReadIdent
                int paramNo = int.Parse(varName.TrimStart('#'));
                answer = ParameterValues.ContainsKey(paramNo) ? ParameterValues[paramNo] : 0.0;

                return true;
            }

            return base.EvalVariable(varName, ref answer);
        }
    }
}