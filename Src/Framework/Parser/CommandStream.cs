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

namespace Framework.Parser
{
    using System;
    using System.Linq;
    using System.Text;

    public class CommandStream
    {
        #region privat properties/members

        string          _line;
        int             _idx;
        readonly char   _endCommandChar = ';';
        readonly string _spaceChar      = " \t";

        #endregion

        public string Line
        {
            set
            {
                _line = value;
                _idx  = 0;
            }
            get => _line.Substring(_idx);
        }

        public char NextChar        => IsEOF() ? ((char)0) : _line[_idx];
        public char NextCharToUpper => char.ToUpper(NextChar);

        public int PushIdx()
        {
            return _idx;
        }

        public void PopIdx(int idx)
        {
            _idx = idx;
        }

        public char SkipSpaces()
        {
            while (!IsEOF() && _spaceChar.Contains(_line[_idx]))
            {
                _idx++;
            }

            return NextChar;
        }

        public char SkipSpacesToUpper()
        {
            return char.ToUpper(SkipSpaces());
        }

        public void SkipEndCommand()
        {
            while (!IsEOF() && _line[_idx] != _endCommandChar)
            {
                _idx++;
            }

            if (NextChar == _endCommandChar)
            {
                _idx++;
            }

            SkipSpaces();
        }

        public char Next()
        {
            if (!IsEOF())
            {
                _idx++;
            }

            return NextChar;
        }

        public bool IsEOF()
        {
            return _line.Length <= _idx;
        }

        public bool IsEndCommand()
        {
            SkipSpaces();
            return IsEOF() || NextChar == _endCommandChar;
        }

        public bool IsCommand(string cmd)
        {
            if (_line.Length < _idx + cmd.Length)
            {
                return false;
            }

            int i = 0;
            foreach (char ch in cmd)
            {
                if (_line[_idx + i] != ch)
                {
                    return false;
                }

                i++;
            }

            //           if (!Line.StartsWith(cmd)) => slow
            //               return false;

            _idx += cmd.Length;
            return true;
        }

        public int IsCommand(string[] cmds)
        {
            int i = 0;
            foreach (string cmd in cmds)
            {
                if (IsCommand(cmd))
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        #region next char type

        public static bool IsAlpha(char ch)
        {
            ch = char.ToUpper(ch);
            return ch == '_' || IsUpperAZ(ch);
        }

        public static bool IsLowerAZ(char ch)
        {
            return ch >= 'a' && ch <= 'z';
        }

        public static bool IsUpperAZ(char ch)
        {
            return ch >= 'A' && ch <= 'Z';
        }

        public static bool IsDigit(char ch)
        {
            return char.IsDigit(ch);
        }

        public bool IsNumber()
        {
            SkipSpaces();
            return IsNumber(NextChar);
        }

        public static bool IsNumber(char ch)
        {
            return ch == '-' || char.IsDigit(ch) || ch == '.';
        }

        #endregion

        #region Get Numbers

        public int GetInt()
        {
            SkipSpaces();

            int startIdx = _idx;

            if (NextChar == '-')
            {
                Next();
            }

            while (char.IsDigit(NextChar))
            {
                Next();
            }

            string str = _line.Substring(startIdx, _idx - startIdx);
            int    ret = int.Parse(str);
            SkipSpaces();
            return ret;
        }

        public double GetDouble(out bool isFloatingPoint)
        {
            return (double)GetDecimal(out isFloatingPoint);
        }

        public decimal GetDecimal(out bool isFloatingPoint)
        {
            isFloatingPoint = false;
            SkipSpaces();
            bool negative = NextChar == '-';
            if (negative)
            {
                Next();
            }

            decimal val = 0;
            if (NextChar != '.')
            {
                val = Math.Abs(GetInt()); // -0.4 will return 0 => will be positive
            }

            if (NextChar == '.')
            {
                isFloatingPoint = true;
                Next();
                decimal scale = 0.1m;
                while (char.IsDigit(NextChar))
                {
                    val   += scale * (NextChar - '0');
                    scale /= 10;
                    Next();
                }
            }

            if (negative)
            {
                val = -val;
            }

            return val;
        }

        #endregion

        #region Read

        public string ReadString(char[] termChar)
        {
            var sb = new StringBuilder();

            while (!IsEOF() && !termChar.Contains(NextChar))
            {
                sb.Append(NextChar);
                Next();
            }

            return sb.ToString();
        }

        public string ReadDigits()
        {
            var str = new StringBuilder();

            while (char.IsDigit(NextChar))
            {
                str.Append(NextChar);
                Next();
            }

            return str.ToString();
        }

        public string ReadToEnd()
        {
            return ReadString(new char[0]);
        }
    }

    #endregion
}