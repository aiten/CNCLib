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
  http://www.gnu.org/licenses/
*/

using System;
using System.Linq;
using System.Text;

namespace Framework.Tools.Helpers
{
	public class CommandStream
    {
        string _line;
        int _idx;
        char _endCommandChar = ';';
        string _spaceChar = " \t";

        public string Line			{ set { _line = value; _idx = 0; } get => _line.Substring(_idx); }

        public char NextChar => IsEOF() ? ((char) 0) : _line[_idx];
        public char NextCharToUpper => char.ToUpper(NextChar);

        public int PushIdx()		{ return _idx; }
		public void PopIdx(int idx) { _idx = idx; }

        public char SkipSpaces()	
        {
            while (!IsEOF() && (_spaceChar.Contains(_line[_idx])))
                _idx++;

			return NextChar;
        }
		public char SkipSpacesToUpper()
		{
			return char.ToUpper(SkipSpaces());
		}

		public void SkipEndCommand()
        {
            while (!IsEOF() && _line[_idx] != _endCommandChar)
                _idx++;

            if (NextChar == _endCommandChar)
                _idx++;

            SkipSpaces();
        }

        public void Next()
        {
            if (!IsEOF())
                _idx++;
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
            if (_line.Length < _idx+cmd.Length)
                return false;

            int i = 0;
            foreach (char ch in cmd)
            {
                if (_line[_idx+i] != ch)
                    return false;
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
                    return i;
                i++;
            }
 
            return -1;
        }

		public string ReadString(char[] termchar)
		{
			var sb = new StringBuilder();

			while (!IsEOF() && !termchar.Contains(NextChar))
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

        public bool IsNumber()
        {
            SkipSpaces();
            return NextChar == '-' || char.IsDigit(NextChar);
        }
 
        public int GetInt()
        {
            SkipSpaces();

            int startidx = _idx;
            while (char.IsDigit(NextChar) || NextChar == '-')
                Next();

			string str = _line.Substring(startidx, _idx - startidx);
            int ret = int.Parse(str);
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
			bool negativ = NextChar == '-';
			if (negativ) Next();

			decimal val = 0;
			if (NextChar != '.')
				val = Math.Abs(GetInt());		// -0.4 will return 0 => will be positiv

			if (NextChar == '.')
			{
				isFloatingPoint = true;
				Next();
				decimal scale = 0.1m;
				while (char.IsDigit(NextChar))
				{
					val += scale * (NextChar - '0');
					scale /= 10;
					Next();
				}
			}

			if (negativ)
				val = -val;

			return val;
		}
    }
}
