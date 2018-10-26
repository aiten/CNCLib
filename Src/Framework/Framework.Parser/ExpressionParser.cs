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

namespace Framework.Parser
{
    using System;
    using System.Text;

    public class ExpressionParser : Parser
    {
        private readonly string MESSAGE_EXPR_EMPTY_EXPR          = "Empty expression";
        private readonly string MESSAGE_EXPR_FORMAT              = "Expression format error";
        private readonly string MESSAGE_EXPR_UNKNOWN_FUNCTION    = "Unknown function";
        private readonly string MESSAGE_EXPR_SYNTAX_ERROR        = "Syntax error";
        private readonly string MESSAGE_EXPR_MISSINGRPARENTHESIS = "Missing right parenthesis";
        private readonly string MESSAGE_EXPR_ILLEGAL_OPERATOR    = "Illegal operator";
        private readonly string MESSAGE_EXPR_ILLEGAL_FUNCTION    = "Illegal function";
        private readonly string MESSAGE_EXPR_UNKNOWN_VARIABLE    = "Unknown variable";
        private readonly string MESSAGE_EXPR_FRACTORIAL          = "factorial";

        public ExpressionParser(CommandStream reader) : base(reader)
        {
        }

        protected char LeftParenthesis  { get; set; } = '(';
        protected char RightParenthesis { get; set; } = ')';

        public override void Parse()
        {
            Answer = 0;

            GetNextToken();
            if (GetTokenType() == ETokenType.EndOfLineSy)
            {
                ErrorAdd(MESSAGE_EXPR_EMPTY_EXPR);
                return;
            }

            Answer = ParseLevel1();

            if (IsError())
            {
                return;
            }

            // check for garbage at the end of the expression
            // an expression ends with a character '\0' and GetMainTokenType() = delimeter
            if (GetTokenType() != ETokenType.EndOfLineSy)
            {
                ErrorAdd(MESSAGE_EXPR_FORMAT);
                return;
            }
        }

        public double Answer { get; private set; }

        protected enum ETokenType
        {
            UnknownSy,
            NothingSy,
            EndOfLineSy,

            AssignSy,
            LeftParenthesisSy,
            RightParenthesisSy,

            // Operator
            // Level2
            AndSy,
            OrSy,
            BitShiftLeftSy,
            BitShiftRightSy,

            // Level3
            EqualSy,
            UnEqualSy,
            LessSy,
            GreaterSy,
            LessEqualSy,
            GreaterEqualSy,

            // Level 4
            PlusSy,
            MinusSy,

            // Level 5
            MultiplySy,
            DivideSy,
            ModuloSy,
            XOrSy,

            // Level 6
            PowSy,

            // Level 7
            FactorialSy,

            IntegerSy,
            FloatSy,

            VariableSy,

            // Functions
            FirstFunctionSy,
            AbsSy = FirstFunctionSy,
            ExpSy,
            SignSy,
            SqrtSy,
            LogSy,
            Log10Sy,
            SinSy,
            CosSy,
            TanSy,
            AsinSy,
            AcosSy,
            AtanSy,
            FixSy,
            FupSy,
            RoundSy,

            FactorialFncSy,
            LastFunctionSy = FactorialFncSy
        };

        protected struct SParserState
        {
            public double _number; // number if parsed integer or float or variable(content)

            public string _varName;

            public bool       _variableOK; // _number = variable with content
            public ETokenType _detailtoken;
        };

        protected SParserState _state;


        protected void GetNextToken()
        {
            _state._detailtoken = ETokenType.NothingSy;
            if (IsError())
            {
                return;
            }

            char ch = _reader.SkipSpaces();

            if (ch == '\0')
            {
                _state._detailtoken = ETokenType.EndOfLineSy;
                return;
            }

            ScannNextToken();
        }

        protected virtual void ScannNextToken()
        {
            char ch = _reader.NextChar;
            if (IsToken("||", false, false))
            {
                _state._detailtoken = ETokenType.XOrSy;
                return;
            }

            if (IsToken("<<", false, false))
            {
                _state._detailtoken = ETokenType.BitShiftLeftSy;
                return;
            }

            if (IsToken(">>", false, false))
            {
                _state._detailtoken = ETokenType.BitShiftRightSy;
                return;
            }

            if (IsToken("==", false, false))
            {
                _state._detailtoken = ETokenType.EqualSy;
                return;
            }

            if (IsToken("!=", false, false))
            {
                _state._detailtoken = ETokenType.UnEqualSy;
                return;
            }

            if (IsToken(">=", false, false))
            {
                _state._detailtoken = ETokenType.GreaterEqualSy;
                return;
            }

            if (IsToken("<=", false, false))
            {
                _state._detailtoken = ETokenType.LessEqualSy;
                return;
            }

            if (ch == LeftParenthesis)
            {
                _state._detailtoken = ETokenType.LeftParenthesisSy;
                _reader.Next();
                return;
            }

            if (ch == RightParenthesis)
            {
                _state._detailtoken = ETokenType.RightParenthesisSy;
                _reader.Next();
                return;
            }

            switch (ch)
            {
                case '>':
                    _state._detailtoken = ETokenType.GreaterSy;
                    _reader.Next();
                    return;
                case '<':
                    _state._detailtoken = ETokenType.LessSy;
                    _reader.Next();
                    return;
                case '&':
                    _state._detailtoken = ETokenType.AndSy;
                    _reader.Next();
                    return;
                case '|':
                    _state._detailtoken = ETokenType.OrSy;
                    _reader.Next();
                    return;
                case '-':
                    _state._detailtoken = ETokenType.MinusSy;
                    _reader.Next();
                    return;
                case '+':
                    _state._detailtoken = ETokenType.PlusSy;
                    _reader.Next();
                    return;
                case '*':
                    _state._detailtoken = ETokenType.MultiplySy;
                    _reader.Next();
                    return;
                case '/':
                    _state._detailtoken = ETokenType.DivideSy;
                    _reader.Next();
                    return;
                case '%':
                    _state._detailtoken = ETokenType.ModuloSy;
                    _reader.Next();
                    return;
                case '^':
                    _state._detailtoken = ETokenType.PowSy;
                    _reader.Next();
                    return;
                case '!':
                    _state._detailtoken = ETokenType.FactorialSy;
                    _reader.Next();
                    return;
                case '=':
                    _state._detailtoken = ETokenType.AssignSy;
                    _reader.Next();
                    return;
            }

            // check for a value
            if (CommandStream.IsNumber(ch))
            {
                _state._detailtoken = ETokenType.FloatSy;
                _state._number      = _reader.GetDouble(out bool istFloatingPoint);
                return;
            }

            // check for variables or functions
            if (IsIdentStart(ch))
            {
                var start = ReadIdent();
                ch = _reader.SkipSpaces();

                // check if this is a variable or a function.
                // a function has a parentesis '(' open after the name

                if (ch == LeftParenthesis)
                {
                    switch (start.ToUpper())
                    {
                        case "ABS":
                            _state._detailtoken = ETokenType.AbsSy;
                            return;

                        case "EXP":
                            _state._detailtoken = ETokenType.ExpSy;
                            return;

                        case "SIGN":
                            _state._detailtoken = ETokenType.SignSy;
                            return;

                        case "SQRT":
                            _state._detailtoken = ETokenType.SqrtSy;
                            return;

                        case "LOG":
                            _state._detailtoken = ETokenType.LogSy;
                            return;

                        case "LOG10":
                            _state._detailtoken = ETokenType.Log10Sy;
                            return;

                        case "SIN":
                            _state._detailtoken = ETokenType.SinSy;
                            return;

                        case "COS":
                            _state._detailtoken = ETokenType.CosSy;
                            return;

                        case "TAN":
                            _state._detailtoken = ETokenType.TanSy;
                            return;

                        case "ASIN":
                            _state._detailtoken = ETokenType.AsinSy;
                            return;

                        case "ACOS":
                            _state._detailtoken = ETokenType.AcosSy;
                            return;

                        case "ATAN":
                            _state._detailtoken = ETokenType.AtanSy;
                            return;

                        case "FACTORIAL":
                            _state._detailtoken = ETokenType.FactorialFncSy;
                            return;

                        case "FIX":
                            _state._detailtoken = ETokenType.FixSy;
                            return;

                        case "FUP":
                            _state._detailtoken = ETokenType.FupSy;
                            return;

                        case "ROUND":
                            _state._detailtoken = ETokenType.RoundSy;
                            return;

                        default:
                            Error(MESSAGE_EXPR_UNKNOWN_FUNCTION);
                            return;
                    }
                }
                else
                {
                    _state._detailtoken = ETokenType.VariableSy;
                    _state._variableOK  = EvalVariable(start, ref _state._number);
                }

                return;
            }

            // something unknown is found, wrong characters -> a syntax error
            _state._detailtoken = ETokenType.UnknownSy;
            Error(MESSAGE_EXPR_SYNTAX_ERROR);
            return;
        }

        protected virtual string ReadIdent()
        {
            var sb = new StringBuilder();

            char ch = _reader.NextChar;
            sb.Append(ch);
            ch = _reader.Next();

            while (CommandStream.IsAlpha(ch) || CommandStream.IsDigit(ch))
            {
                sb.Append(ch);
                ch = _reader.Next();
            }

            return sb.ToString();
        }

        protected virtual bool IsIdentStart(char ch)
        {
            return CommandStream.IsAlpha(ch);
        }

        protected virtual bool EvalVariable(string var_name, ref double answer)
        {
            _state._varName = var_name;

            // check for built-in variables
            switch (var_name.ToUpper())
            {
                case "E":
                    answer = (double) 2.7182818284590452353602874713527;
                    return true;
                case "PI":
                    answer = (double) 3.1415926535897932384626433832795;
                    return true;
            }

            return false;
        }

        protected virtual void AssignVariable(string var_name, double value)
        {
        }


        ETokenType GetTokenType()
        {
            return _state._detailtoken;
        }

        ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////
        // assignment of variable or function

        double ParseLevel1()
        {
            if (GetTokenType() == ETokenType.VariableSy)
            {
                // copy current state
                var          e_now     = _reader.PushIdx();
                SParserState state_now = _state;

                GetNextToken();
                if (GetTokenType() == ETokenType.AssignSy)
                {
                    // assignment
                    GetNextToken();
                    var ans = ParseLevel2();

                    AssignVariable(state_now._varName, ans);

                    return ans;
                }
                else
                {
                    if (!_state._variableOK)
                    {
                        // unknown variable
                        ErrorAdd(MESSAGE_EXPR_UNKNOWN_VARIABLE);
                        return 0;
                    }

                    // go back to previous token
                    _reader.PopIdx(e_now);
                    _state = state_now;
                }
            }

            return ParseLevel2();
        }

        ////////////////////////////////////////////////////////////
        // conditional operators and bitshift

        double ParseLevel2()
        {
            double     ans        = ParseLevel3();
            ETokenType operatorSy = GetTokenType();

            while (operatorSy == ETokenType.AndSy || operatorSy == ETokenType.OrSy || operatorSy == ETokenType.BitShiftLeftSy || operatorSy == ETokenType.BitShiftRightSy)
            {
                GetNextToken();
                ans        = EvalOperator(operatorSy, ans, ParseLevel3());
                operatorSy = GetTokenType();
            }

            return ans;
        }

        ////////////////////////////////////////////////////////////
        // conditional operators

        double ParseLevel3()
        {
            double     ans        = ParseLevel4();
            ETokenType operatorSy = GetTokenType();

            while (operatorSy == ETokenType.EqualSy || operatorSy == ETokenType.UnEqualSy || operatorSy == ETokenType.LessSy || operatorSy == ETokenType.LessEqualSy ||
                   operatorSy == ETokenType.GreaterSy || operatorSy == ETokenType.GreaterEqualSy)
            {
                GetNextToken();
                ans        = EvalOperator(operatorSy, ans, ParseLevel4());
                operatorSy = GetTokenType();
            }

            return ans;
        }

        ////////////////////////////////////////////////////////////
        // add or subtract

        double ParseLevel4()
        {
            double     ans        = ParseLevel5();
            ETokenType operatorSy = GetTokenType();

            while (operatorSy == ETokenType.PlusSy || operatorSy == ETokenType.MinusSy)
            {
                GetNextToken();
                ans        = EvalOperator(operatorSy, ans, ParseLevel5());
                operatorSy = GetTokenType();
            }

            return ans;
        }

        ////////////////////////////////////////////////////////////
        // multiply, divide, modulus, xor

        double ParseLevel5()
        {
            double     ans        = ParseLevel6();
            ETokenType operatorSy = GetTokenType();

            while (operatorSy == ETokenType.MultiplySy || operatorSy == ETokenType.DivideSy || operatorSy == ETokenType.ModuloSy || operatorSy == ETokenType.XOrSy)
            {
                GetNextToken();
                ans        = EvalOperator(operatorSy, ans, ParseLevel6());
                operatorSy = GetTokenType();
            }

            return ans;
        }

        ////////////////////////////////////////////////////////////
        // power

        double ParseLevel6()
        {
            double     ans        = ParseLevel7();
            ETokenType operatorSy = GetTokenType();

            while (operatorSy == ETokenType.PowSy)
            {
                GetNextToken();
                ans        = EvalOperator(operatorSy, ans, ParseLevel7());
                operatorSy = GetTokenType();
            }

            return ans;
        }

        ////////////////////////////////////////////////////////////
        // Factorial

        double ParseLevel7()
        {
            double     ans        = ParseLevel8();
            ETokenType operatorSy = GetTokenType();

            while (operatorSy == ETokenType.FactorialSy)
            {
                GetNextToken();
                // factorial does not need a value right from the
                // operator, so zero is filled in.
                ans        = EvalOperator(operatorSy, ans, 0.0);
                operatorSy = GetTokenType();
            }

            return ans;
        }

        ////////////////////////////////////////////////////////////
        // Unary minus

        double ParseLevel8()
        {
            if (GetTokenType() == ETokenType.MinusSy)
            {
                GetNextToken();
                return -ParseLevel9();
            }

            return ParseLevel9();
        }

        ////////////////////////////////////////////////////////////
        // functions

        double ParseLevel9()
        {
            if (GetTokenType() >= ETokenType.FirstFunctionSy && GetTokenType() <= ETokenType.LastFunctionSy)
            {
                ETokenType functionSy = GetTokenType();
                GetNextToken();
                return EvalFunction(functionSy, ParseLevel10());
            }

            return ParseLevel10();
        }

        ////////////////////////////////////////////////////////////
        // parenthesized expression or value

        double ParseLevel10()
        {
            // check if it is a parenthesized expression
            if (GetTokenType() == ETokenType.LeftParenthesisSy)
            {
                GetNextToken();
                double ans = ParseLevel2();
                if (GetTokenType() != ETokenType.RightParenthesisSy)
                {
                    ErrorAdd(MESSAGE_EXPR_MISSINGRPARENTHESIS);
                    return 0;
                }

                GetNextToken();
                return ans;
            }

            // if not parenthesized then the expression is a value
            return ParseNumber();
        }

        ////////////////////////////////////////////////////////////

        double ParseNumber()
        {
            double ans;

            switch (GetTokenType())
            {
                case ETokenType.FloatSy:
                case ETokenType.IntegerSy:
                case ETokenType.VariableSy:
                    // this is a number
                    ans = _state._number;
                    GetNextToken();
                    break;
                default:
                    // syntax error or unexpected end of expression
                    ErrorAdd(MESSAGE_EXPR_SYNTAX_ERROR);
                    return 0;
            }

            return ans;
        }

        double EvalOperator(ETokenType operatorSy, double lhs, double rhs)
        {
            switch (operatorSy)
            {
                // level 2
                case ETokenType.AndSy:           return (double) ((uint) (lhs) & (uint) (rhs));
                case ETokenType.OrSy:            return (double) ((uint) (lhs) | (uint) (rhs));
                case ETokenType.BitShiftLeftSy:  return (double) ((uint) (lhs) << (ushort) (rhs));
                case ETokenType.BitShiftRightSy: return (double) ((uint) (lhs) >> (ushort) (rhs));

                // level 3
                case ETokenType.EqualSy:        return lhs == rhs ? 1.0 : 0.0;
                case ETokenType.UnEqualSy:      return lhs != rhs ? 1.0 : 0.0;
                case ETokenType.LessSy:         return lhs < rhs ? 1.0 : 0.0;
                case ETokenType.GreaterSy:      return lhs > rhs ? 1.0 : 0.0;
                case ETokenType.LessEqualSy:    return lhs <= rhs ? 1.0 : 0.0;
                case ETokenType.GreaterEqualSy: return lhs >= rhs ? 1.0 : 0.0;

                // level 4
                case ETokenType.PlusSy:  return lhs + rhs;
                case ETokenType.MinusSy: return lhs - rhs;

                // level 5
                case ETokenType.MultiplySy: return lhs * rhs;
                case ETokenType.DivideSy:   return lhs / rhs;
                case ETokenType.ModuloSy:   return (double) ((uint) (lhs) % (uint) (rhs));
                case ETokenType.XOrSy:      return (double) ((uint) (lhs) ^ (uint) (rhs));

                // level 6
                case ETokenType.PowSy: return Math.Pow(lhs, rhs);

                // level 7
                case ETokenType.FactorialSy: return Factorial(lhs);
            }

            ErrorAdd(MESSAGE_EXPR_ILLEGAL_OPERATOR);
            return 0;
        }

        double EvalFunction(ETokenType operatorSy, double value)
        {
            switch (operatorSy)
            {
                // arithmetic
                case ETokenType.AbsSy:   return Math.Abs(value);
                case ETokenType.ExpSy:   return Math.Exp(value);
                case ETokenType.SignSy:  return Sign(value);
                case ETokenType.SqrtSy:  return Math.Sqrt(value);
                case ETokenType.LogSy:   return Math.Log(value, 2);
                case ETokenType.Log10Sy: return Math.Log10(value);

                // trigonometric
                case ETokenType.SinSy:  return Math.Sin(value);
                case ETokenType.CosSy:  return Math.Cos(value);
                case ETokenType.TanSy:  return Math.Tan(value);
                case ETokenType.AsinSy: return Math.Asin(value);
                case ETokenType.AcosSy: return Math.Acos(value);
                case ETokenType.AtanSy: return Math.Atan(value);

                // probability
                case ETokenType.FactorialFncSy: return Factorial(value);

                // cnc
                case ETokenType.FixSy:   return Math.Floor(value);
                case ETokenType.FupSy:   return Math.Ceiling(value);
                case ETokenType.RoundSy: return Math.Round(value);
            }

            ErrorAdd(MESSAGE_EXPR_ILLEGAL_FUNCTION);
            return 0;
        }


        double Factorial(double value)
        {
            var v = (uint) (value);

            if (value != (uint) (v))
            {
                ErrorAdd(MESSAGE_EXPR_FRACTORIAL);
                return 0;
            }

            var res = (double) v;
            v--;
            while (v > 1)
            {
                res *= v;
                v--;
            }

            if (res == 0)
            {
                res = 1; // 0! is per definition 1
            }

            return res;
        }

        double Sign(double value)
        {
            if (value > 0)
            {
                return 1;
            }

            if (value < 0)
            {
                return -1;
            }

            return 0;
        }


        //bool SaveAssign(char* buffer, char* current, char ch, uint8_t max);
    }
}