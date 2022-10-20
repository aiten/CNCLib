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

namespace CNCLib.GCode.Generate.Parser;

using System.Collections.Generic;

using Framework.Parser;

public class GCodeExpressionParser : ExpressionParser
{
    public GCodeExpressionParser(ParserStreamReader reader) : base(reader)
    {
        LeftParenthesis  = '[';
        RightParenthesis = ']';
    }

    public Dictionary<int, double> ParameterValues { get; set; }

    protected override void ScanNextToken()
    {
        char ch = Reader.NextChar;
        while (ch != 0)
        {
            if (ch == ';' || ch == '(') // comment
            {
                ch = new GCodeParser(Reader).SkipSpacesOrComment();
                Error("NotImplemented yet");
            }
            else
            {
                break;
            }
        }

        if (ch == '\0')
        {
            _state._detailToken = TokenType.EndOfLineSy;
            return;
        }

        base.ScanNextToken();
    }

    protected override string ReadIdent()
    {
        // read variable name of gcode : #1 or #<_x>

        char ch = NextChar;
        if (ch == '#')
        {
            // start of GCODE variable => format #1 or #<_x>
            ch = Next();

            if (ch == '<')
            {
                ch = Next();
                var ident = ReadString('>');
                if (NextChar == '>')
                {
                    Next();
                    return $"#<{ident}>";
                }

                Error("> expected");
                return "";
            }
            else
            {
                _state._number = GetInt();
                return $"#{_state._number}";
            }
        }

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

            if (varName[1] == '<')
            {
                return false;
            }
            else
            {
                int paramNo = int.Parse(varName.TrimStart('#'));
                answer = ParameterValues.ContainsKey(paramNo) ? ParameterValues[paramNo] : 0.0;
            }

            return true;
        }

        return base.EvalVariable(varName, ref answer);
    }
}