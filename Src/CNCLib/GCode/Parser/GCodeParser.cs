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

using Framework.Parser;

///////////////////////////////////////////////////////

namespace CNCLib.GCode.Parser
{
    public class GCodeParser : Framework.Parser.Parser
    {
        readonly string MESSAGE_GCODE_CommentNestingError = "Comment nesting error";

        public GCodeParser(CommandStream reader) : base(reader)
        {
        }

        public override void Parse()
        {
        }

        public static bool IsCommentStart(char ch)
        {
            return ch == '(' || ch == '*' || ch == ';';
        }

        ////////////////////////////////////////////////////////////

        public char SkipSpacesOrComment()
        {
            switch (_reader.SkipSpaces())
            {
                case '(':
                    SkipCommentNested();
                    break;
                case '*':
                case ';':
                    SkipCommentSingleLine();
                    break;
            }

            return _reader.NextChar;
        }

        ////////////////////////////////////////////////////////////

        void SkipCommentSingleLine()
        {
            _reader.ReadToEnd();
        }

        void SkipCommentNested()
        {
            int cnt = 0;

//            char* start = (char*)_reader->GetBuffer();

            for (char ch = _reader.NextChar; ch != 0; ch = _reader.Next())
            {
                switch (ch)
                {
                    case ')':
                    {
                        cnt--;
                        if (cnt == 0)
                        {
                            _reader.Next();

//                            CommentMessage(start);
                            return;
                        }

                        break;
                    }
                    case '(':
                        cnt++;
                        break;
                }
            }

            Error(MESSAGE_GCODE_CommentNestingError);
        }
    }
}