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

namespace CNCLib.GCode.Generate.Commands;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using CNCLib.GCode.Generate.Parser;

using Framework.Drawing;
using Framework.Parser;

public abstract class Command
{
    public enum CommandMoveType
    {
        NoMove,
        Fast,  // Go
        Normal // G1,G2
    }

    #region crt

    protected Command()
    {
        PositionValid = false;
        MoveType      = CommandMoveType.NoMove;
    }

    private Point3D        _calculatedEndPosition;
    private List<Variable> _variables = new List<Variable>();

    #endregion

    #region Property

    public Command NextCommand { get; set; }
    public Command PrevCommand { get; set; }

    public Point3D CalculatedStartPosition =>
        PrevCommand == null ? new Point3D() : PrevCommand.CalculatedEndPosition;

    public Point3D CalculatedEndPosition => _calculatedEndPosition;

    public bool            UseWithoutPrefix { get; protected set; }
    public bool            PositionValid    { get; protected set; }
    public CommandMoveType MoveType         { get; protected set; }

    public string SubCode { get; protected set; }
    public string Code    { get; protected set; }

    public int? LineNumber { get; set; }

    /// <summary>
    /// ImportInfo, e.g. Hpgl Command
    /// </summary>
    public string ImportInfo { get; set; }

    #endregion

    #region GCode-Variables

    public class Variable
    {
        public char    Name            { get; set; }
        public double? Value           { get; set; }
        public string  Parameter       { get; set; }
        public bool    ParameterIsTerm { get; set; }

        public bool ForceFloatingPoint { get; set; }

        public string ToGCode()
        {
            if (Value.HasValue)
            {
                string ret = Name + Value.Value.ToString(CultureInfo.InvariantCulture);
                if (ForceFloatingPoint && ret.IndexOf('.') == -1)
                {
                    return ret + ".0";
                }

                return ret;
            }

            if (string.IsNullOrEmpty(Parameter))
            {
                return Name.ToString();
            }

            if (ParameterIsTerm)
            {
                return Name + "[" + Parameter + "]";
            }

            return Name + "#" + Parameter;
        }

        public Variable ShallowCopy()
        {
            return (Variable)MemberwiseClone();
        }
    }

    public void AddVariable(Variable var)
    {
        _variables.Add(var);
    }

    public void AddVariable(char name, Variable var)
    {
        var newVar = var.ShallowCopy();
        newVar.Name = name;
        _variables.Add(newVar);
    }

    public void AddVariable(char name, double value, bool isFloatingPoint)
    {
        AddVariable(new Variable { Name = name, Value = value, ForceFloatingPoint = isFloatingPoint });
    }

    public void AddVariableNoValue(char name)
    {
        AddVariable(new Variable { Name = name });
    }

    public void AddVariableParam(char name, string paramValue, bool isTerm)
    {
        AddVariable(new Variable { Name = name, Parameter = paramValue, ParameterIsTerm = isTerm });
    }

    public void AddVariable(char name, decimal value)
    {
        AddVariable(new Variable { Name = name, Value = (double)value });
    }

    public double GetVariable(char name, CommandState state, double defaultValue)
    {
        if (TryGetVariable(name, state, out var ret))
        {
            return ret;
        }

        return defaultValue;
    }

    public Variable GetVariable(char name)
    {
        return _variables.Find(n => n.Name == name);
    }

    public bool TryGetVariable(char name, CommandState state, out double val)
    {
        Variable var = GetVariable(name);
        if (var?.Value != null)
        {
            val = var.Value.Value;
            return true;
        }

        if (var?.Parameter != null)
        {
            if (var.ParameterIsTerm)
            {
                var lineStream       = new ParserStreamReader() { Line                         = var.Parameter };
                var expressionParser = new GCodeExpressionParser(lineStream) { ParameterValues = state.ParameterValues };
                expressionParser.Parse();
                if (!expressionParser.IsError())
                {
                    val = expressionParser.Answer;
                    return true;
                }
            }
            else if (int.TryParse(var.Parameter, out int parameterNo) && state.ParameterValues.ContainsKey(parameterNo))
            {
                val = state.ParameterValues[parameterNo];
                return true;
            }
        }

        val = 0;
        return false;
    }

    public string TryGetVariableGCode(char name)
    {
        Variable var = GetVariable(name);
        if (var?.Value != null)
        {
            return var.ToGCode();
        }

        return null;
    }

    public bool CopyVariable(char name, Command dest)
    {
        Variable var = GetVariable(name);
        if (var?.Value == null)
        {
            return false;
        }

        dest.AddVariable(var.ShallowCopy());

        return true;
    }

    #endregion

    #region Iteration

    public virtual void SetCommandState(CommandState state)
    {
    }

    #endregion

    #region Draw

    public virtual Command[] ConvertCommand(CommandState state, ConvertOptions options)
    {
        return new[] { this };
    }

    public DrawType Convert(CommandMoveType moveType, CommandState state)
    {
        var drawType = DrawType.NoDraw;

        if (moveType != CommandMoveType.NoMove)
        {
            if (state.IsSelected)
            {
                drawType |= DrawType.Selected;
            }

            drawType |= DrawType.Draw;

            if (state.UseLaser)
            {
                if (state.LaserOn == false)
                {
                    return DrawType.NoDraw;
                }

                drawType |= DrawType.Laser;
            }

            if (moveType == CommandMoveType.Normal)
            {
                drawType |= DrawType.Cut;
            }
        }

        return drawType;
    }

    public virtual void Draw(IOutputCommand output, CommandState state, object param)
    {
        output.DrawLine(this, param, Convert(MoveType, state), CalculatedStartPosition, CalculatedEndPosition);
    }

    #endregion

    #region GCode

    public virtual void SetCode(string code)
    {
    } // allow generic Gxx & Mxx to set code

    public string GCodeAdd { get; set; }

    protected string GCodeHelper(Point3D current)
    {
        var sb = new StringBuilder();

        if (LineNumber.HasValue)
        {
            sb.Append(GCodeLineNumber(""));
        }

        if (!string.IsNullOrEmpty(Code))
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(Code);
            if (!string.IsNullOrEmpty(SubCode))
            {
                sb.Append('.');
                sb.Append(SubCode);
            }
        }

        foreach (Variable p in _variables)
        {
            sb.Append(' ');
            sb.Append(p.ToGCode());
        }

        if (!string.IsNullOrEmpty(GCodeAdd))
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(GCodeAdd);
        }

        return sb.ToString();
    }

    protected string GCodeLineNumber(string postString)
    {
        return LineNumber.HasValue ? $"N{LineNumber}{postString}" : "";
    }

    public virtual string[] GetGCodeCommands(Point3D startFrom, CommandState state)
    {
        var ret = new[]
        {
            GCodeHelper(startFrom)
        };
        return ret;
    }

    #endregion

    #region Serialisation

    public virtual void UpdateCalculatedEndPosition(CommandState state)
    {
        if (PositionValid)
        {
            var    sc = new Point3D();
            double val;

            if (TryGetVariable('X', state, out val))
            {
                sc.X = val;
            }

            if (TryGetVariable('Y', state, out val))
            {
                sc.Y = val;
            }

            if (TryGetVariable('Z', state, out val))
            {
                sc.Z = val;
            }

            if (!sc.HasAllValues && PrevCommand != null)
            {
                sc.AssignMissing(PrevCommand.CalculatedEndPosition);
            }

            _calculatedEndPosition = sc;
        }
        else
        {
            _calculatedEndPosition = PrevCommand == null ? new Point3D() : PrevCommand._calculatedEndPosition;
        }
    }

    protected void ReadFromToEnd(Parser parser)
    {
        GCodeAdd = "";
        while (!parser.IsEOF())
        {
            GCodeAdd += parser.NextChar;
            parser.Next();
        }
    }

    protected double? ReadVariable(Parser parser, char param, bool allowNameOnly)
    {
        parser.Next();
        parser.SkipSpaces();
        if (parser.NextChar == '#')
        {
            parser.Next();
            int paramNr = parser.GetInt();
            AddVariableParam(param, paramNr.ToString(), false);
            return 0;
        }

        if (parser.NextChar == '[')
        {
            int depth = 1;
            parser.Next();
            var sb = new StringBuilder();

            while (!parser.IsEndCommand() && depth != 0)
            {
                switch (parser.NextChar)
                {
                    case '[':
                        depth++;
                        break;
                    case ']':
                        depth--;
                        break;
                }

                if (depth != 0)
                {
                    sb.Append(parser.NextChar);
                }

                parser.Next();
            }

            AddVariableParam(param, sb.ToString(), true);
            return 0;
        }

        parser.SkipSpaces();

        if (parser.IsNumber())
        {
            bool   isFloatingPoint;
            double val = parser.GetDouble(out isFloatingPoint);
            AddVariable(param, val, isFloatingPoint);
            return val;
        }

        if (allowNameOnly)
        {
            AddVariableNoValue(param);
            return null;
        }

        throw new ArgumentOutOfRangeException(nameof(param), param, @"Illegal Variable name.");
    }

    public virtual void ReadFrom(Parser parser)
    {
        var ep = new Point3D();

        if (parser.NextChar == '.')
        {
            parser.Next();
            SubCode = parser.GetInt().ToString();
        }

        if (PositionValid)
        {
            while (true)
            {
                switch (parser.SkipSpacesToUpper())
                {
                    case 'X':
                        ep.X = ReadVariable(parser, parser.NextCharToUpper, false);
                        break;
                    case 'Y':
                        ep.Y = ReadVariable(parser, parser.NextCharToUpper, false);
                        break;
                    case 'Z':
                        ep.Z = ReadVariable(parser, parser.NextCharToUpper, false);
                        break;
                    case 'F':
                        ReadVariable(parser, parser.NextCharToUpper, true);
                        break;
                    case 'P':
                    case 'R':
                    case 'I':
                    case 'J':
                    case 'K':
                        ReadVariable(parser, parser.NextCharToUpper, false);
                        break;
                    default:
                    {
                        ReadFromToEnd(parser);
                        return;
                    }
                }
            }
        }
        else
        {
            while (true)
            {
                switch (parser.SkipSpacesToUpper())
                {
                    case 'P':
                        ReadVariable(parser, parser.NextCharToUpper, false);
                        break;
                    default:
                    {
                        ReadFromToEnd(parser);
                        return;
                    }
                }
            }
        }
    }

    #endregion
}