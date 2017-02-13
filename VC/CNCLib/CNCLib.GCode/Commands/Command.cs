////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2017 Herbert Aitenbichler

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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Framework.Tools;
using Framework.Tools.Drawing;

namespace CNCLib.GCode.Commands
{
    public abstract class Command
	{
		public enum MoveType
		{
			NoMove,
			Fast,		// Go
			Normal		// G1,G2
		}

		#region crt

		public Command()
        {
			PositionValid = false;
			Movetype = MoveType.NoMove;
        }

		private Point3D _calculatedEndPosition;
		protected List<Variable> _variables = new List<Variable>();

		#endregion

        #region Property

		public Command NextCommand { get; set; }
		public Command PrevCommand { get; set; }

        public Point3D CalculatedStartPosition 
		{
			get
			{
				return PrevCommand == null ? new Point3D() : PrevCommand.CalculatedEndPosition;
			}
		}
		public Point3D CalculatedEndPosition
		{
			get
			{
				return _calculatedEndPosition;
			}
		}			

        public bool UseWithoutPrefix { get; protected set; }
        public bool PositionValid { get; protected set; }
		public MoveType Movetype { get; protected set; }

		public string SubCode { get; protected set;  }
		public string Code { get; protected set; }

		public int? LineNumber { get; set; }

		/// <summary>
		/// Importinfo, e.g. HPGL Command
		/// </summary>
		public string ImportInfo { get; set; }

		#endregion

		#region GCode-Variables

		public class Variable
		{
			public char Name { get; set; }
			public double? Value { get; set; }
			public string Parameter { get; set; }

			public bool ForceFloatingPoint { get; set; }

			public string ToGCode() 
			{
				if (Value.HasValue)
				{
					string ret = Name + Value.Value.ToString(CultureInfo.InvariantCulture);
					if (ForceFloatingPoint && ret.IndexOf('.')==-1)
					{
						return ret + ".0";
					}
					return ret;
				}

				if (string.IsNullOrEmpty(Parameter))
					return Name.ToString();

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
		public void AddVariable(char name,Variable var)
		{
			var newvar = var.ShallowCopy();
			newvar.Name = name;
			_variables.Add(newvar);
		}

		public void AddVariable(char name, double value, bool isFloatingPoint)
		{
			AddVariable(new Variable() { Name = name, Value = value, ForceFloatingPoint = isFloatingPoint });
		}
		public void AddVariableNoValue(char name)
		{
			AddVariable(new Variable() { Name = name });
		}
		public void AddVariableParam(char name, string paramvalue)
		{
			AddVariable(new Variable() { Name = name, Parameter = paramvalue });
		}
		public void AddVariable(char name, decimal value)
		{
			AddVariable(new Variable() { Name = name, Value = (double) value });
		}

		public double GetVariable(char name, double defaultvalue)
		{
			double ret;
			if (TryGetVariable(name, out ret))
				return ret;
			return defaultvalue;
		}

		public Variable GetVariable(char name)
		{
			return _variables.Find(n => n.Name == name);
		}

		public bool TryGetVariable(char name, out double val)
		{
			Variable var = GetVariable(name);
			if (var!=null && var.Value.HasValue)
			{
				val = var.Value.Value;
				return true;
			}
			val = 0;
			return false;
		}

		public string TryGetVariableGCode(char name)
		{
			Variable var = GetVariable(name);
			if (var != null && var.Value.HasValue)
			{
				return var.ToGCode();
			}
			return null;
		}

		public bool CopyVariable(char name, Command dest)
		{
			Variable var = GetVariable(name);
			if (var == null || !var.Value.HasValue) return false;

			dest.AddVariable(var.ShallowCopy());

			return true;
		}

		#endregion

		#region Draw

		public virtual Command[] ConvertCommand(CommandState state, ConvertOptions options)
		{
			return new Command[] { this };
		}

		public DrawType Convert(MoveType movetype, CommandState state)
        {
			DrawType drawtype = DrawType.NoDraw;

			if (movetype != MoveType.NoMove)
			{
				if (state.IsSelected)
					drawtype |= DrawType.Selected;

				drawtype |= DrawType.Draw;

				if (state.UseLaser)
				{
					if (state.LaserOn == false) return DrawType.NoDraw;
					drawtype |= DrawType.Laser;
				}

				if (movetype == MoveType.Normal)
				{
					drawtype |= DrawType.Cut;
				}
			}

            return drawtype;
        }

        public virtual void Draw(IOutputCommand output, CommandState state, object param)
		{
			output.DrawLine(this, param, Convert(Movetype, state), CalculatedStartPosition, CalculatedEndPosition);
		}

        #endregion

        #region GCode

        public virtual void SetCode(string code) { }     // allow genieric Gxx & Mxx to set code

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
					sb.Append(' ');

				sb.Append(Code);
				if (!string.IsNullOrEmpty(SubCode))
				{
					sb.Append('.');
					sb.Append(SubCode);
				}
			}

			foreach (Variable p in _variables )
			{
				sb.Append(' ');
				sb.Append(p.ToGCode());
			}

			if (!string.IsNullOrEmpty(GCodeAdd))
			{
				if (sb.Length > 0)
					sb.Append(' ');

				sb.Append(GCodeAdd);
			}
			return sb.ToString();
        }

		protected string GCodeLineNumber(string postString)
		{
			if (LineNumber.HasValue)
			{
				return $"N{ LineNumber }{postString}";
			}
			return "";
		}

		public virtual string[] GetGCodeCommands(Point3D startfrom, CommandState state)
		{
			string[] ret = new string[] 
            {
                GCodeHelper(startfrom)
            };
			return ret;
		}

		#endregion

		#region Serialisation

		public void UpdateCalculatedEndPosition()
		{
			if (PositionValid)
			{
				var sc = new Point3D();
				double val;

				if (TryGetVariable('X', out val)) sc.X = (double) val;
				if (TryGetVariable('Y', out val)) sc.Y = (double) val;
				if (TryGetVariable('Z', out val)) sc.Z = (double) val;
				if (!sc.HasAllValues && PrevCommand != null)
				{
					sc.AssignMissing(PrevCommand.CalculatedEndPosition);
				}
				_calculatedEndPosition = sc;;
			}
			else
			{
				_calculatedEndPosition = (PrevCommand == null) ? new Point3D() : PrevCommand._calculatedEndPosition;
			}
		}			

		private void ReadFromToEnd(CommandStream stream)
		{
			GCodeAdd = "";
			while (!stream.IsEOF())
			{
				GCodeAdd += stream.NextChar;
				stream.Next();
			}
		}

		protected double? ReadVariable(CommandStream stream, char param, bool allowNameOnly)
		{
			stream.Next();
			stream.SkipSpaces();
			if (stream.NextChar == '#')
			{
				stream.Next();
				var paramNr = stream.GetInt();
				AddVariableParam(param,paramNr.ToString());
				return 0;
			}

			stream.SkipSpaces();

			if (stream.IsNumber())
			{
				bool isFloatingPoint;
				var val = stream.GetDouble(out isFloatingPoint);
				AddVariable(param, val, isFloatingPoint);
				return val;
			}
			else if (allowNameOnly)
			{
				AddVariableNoValue(param);
				return null;
			}

			throw new ArgumentOutOfRangeException();
		}

		public virtual void ReadFrom(CommandStream stream)
		{
			Point3D ep = new Point3D();

			if (stream.NextChar == '.')
			{
				stream.Next();
				SubCode = stream.GetInt().ToString();
			}

			if (PositionValid)
			{
				while (true)
				{
					switch (stream.SkipSpacesToUpper())
					{
						case 'X': ep.X = ReadVariable(stream, stream.NextCharToUpper, false); break;
						case 'Y': ep.Y = ReadVariable(stream, stream.NextCharToUpper, false); break;
						case 'Z': ep.Z = ReadVariable(stream, stream.NextCharToUpper, false); break;
						case 'F': ReadVariable(stream, stream.NextCharToUpper, true); break;
						case 'P':
						case 'R':
						case 'I':
						case 'J':
						case 'K': ReadVariable(stream,stream.NextCharToUpper, false); break;
						default:
							{
								ReadFromToEnd(stream);
								return;
							}
					}
				}
			}
			else
			{
				while (true)
				{
					switch (stream.SkipSpacesToUpper())
					{
						case 'P': ReadVariable(stream, stream.NextCharToUpper, false); break;
						default:
						{
							ReadFromToEnd(stream);
							return;
						}
					}
				}
			}
		}

		#endregion
	}
}
