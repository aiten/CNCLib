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