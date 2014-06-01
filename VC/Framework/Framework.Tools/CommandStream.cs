using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Tools
{
    public class CommandStream
    {
        string _line;
        int _idx;
        bool _lastError;
        char _endCommandChar = ';';
        string _spaceChar = " \t";

        public string Line			{ set { _line = value; _idx = 0; _lastError = false; } get { return _line.Substring(_idx); } }

        public char NextChar		{ get { return  IsEOF() ? ((char) 0) : _line[_idx];  } }
		public char NextCharToUpper { get { return char.ToUpper(NextChar); } }

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
        public bool IsInt()
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
		public decimal GetDecimal()
		{
			SkipSpaces();
			bool negativ = NextChar == '-';
			if (negativ) Next();

			decimal val = 0;
			if (NextChar != '.')
				val = Math.Abs(GetInt());		// -0.4 will return 0 => will be positiv

			if (NextChar == '.')
			{
				Next();
				decimal scale = 0.1m;
				while (char.IsDigit(NextChar))
				{
					val += scale * (decimal)(NextChar - '0');
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
