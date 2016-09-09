////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2016 Herbert Aitenbichler

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
		};

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

		protected class Variable
		{
			public char Name;
			public decimal? Value;
			public string Parameter;

			public string ToGCode() 
			{
				if (Value.HasValue)
					return Name + Value.Value.ToString(CultureInfo.InvariantCulture);

				if (string.IsNullOrEmpty(Parameter))
					return Name.ToString();

				return Name + "#" + Parameter;
			}
		};

		public void AddVariable(char name, decimal value)
		{
			_variables.Add(new Variable() { Name = name, Value = value });
		}
		public void AddVariableNoValue(char name)
		{
			_variables.Add(new Variable() { Name = name });
		}
		public void AddVariableParam(char name, string paramvalue)
		{
			_variables.Add(new Variable() { Name = name, Parameter = paramvalue });
		}

		public decimal GetVariable(char name, decimal defaultvalue)
		{
			decimal ret;
			if (TryGetVariable(name, out ret))
				return ret;
			return defaultvalue;
		}

		public bool TryGetVariable(char name, out decimal val)
		{
			Variable var = _variables.Find( n => n.Name == name);
			if (var!=null && var.Value.HasValue)
			{
				val = var.Value.Value;
				return true;
			}
			val = 0;
			return false;
		}

        #endregion

        #region Draw

        public DrawType Convert(MoveType movetype, DrawState state)
        {
            if (movetype == MoveType.NoMove) return DrawType.NoDraw;

            if (state.UseLaser)
            {
                if (state.LaserOn == false) return DrawType.NoDraw;

                switch (movetype)
                {
                    case MoveType.Fast:   return DrawType.LaserFast;
                    case MoveType.Normal: return DrawType.LaserCut;
                }
            }

            switch (movetype)
            {
                case MoveType.Fast: return DrawType.Fast;
                case MoveType.Normal: return DrawType.Cut;
            }

            return DrawType.NoDraw;
        }

        public virtual void Draw(IOutputCommand output, DrawState state, object param)
		{
			output.DrawLine(this, param, Convert(Movetype, state), CalculatedStartPosition, CalculatedEndPosition);
		}

        #endregion

        #region GCode

        public virtual void SetCode(string code) { }     // allow genieric Gxx & Mxx to set code

		public string GCodeAdd { get; set; }

		protected string GCodeHelper(Point3D current)
        {
			String cmd = Code;
			if (!string.IsNullOrEmpty(SubCode))
				cmd += "." + SubCode;

			foreach (Variable p in _variables )
			{
				cmd += " " + p.ToGCode();
			}

			if (!string.IsNullOrEmpty(GCodeAdd))
			{
				if (!string.IsNullOrEmpty(cmd))
					cmd += " ";

				cmd += GCodeAdd;
			}
			return cmd;
        }

		public virtual string[] GetGCodeCommands(Point3D startfrom)
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
				Point3D sc = new Point3D();
				decimal val;

				if (TryGetVariable('X', out val)) sc.X = val;
				if (TryGetVariable('Y', out val)) sc.Y = val;
				if (TryGetVariable('Z', out val)) sc.Z = val;
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

		protected decimal? ReadVariable(CommandStream stream, char param, bool allow)
		{
			stream.Next();
			stream.SkipSpaces();
			if (stream.NextChar == '#')
			{
				stream.Next();
				int paramNr = stream.GetInt();
				AddVariableParam(param,paramNr.ToString());
				return 0;
			}

			stream.SkipSpaces();

			if (stream.IsInt())
			{
				decimal val = stream.GetDecimal();
				AddVariable(param, val);
				return val;
			}
			else
			{
				AddVariableNoValue(param);
				return null;
			}
		}

		public virtual bool ReadFrom(CommandStream stream)
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
						case 'R':
						case 'I':
						case 'J':
						case 'K': ReadVariable(stream,stream.NextCharToUpper, false); break;
						default:
							{
								ReadFromToEnd(stream);
								return true;
							}
					}
				}
			}
			else
			{
				ReadFromToEnd(stream);
			}

			return true;
		}

		#endregion
	}
}
