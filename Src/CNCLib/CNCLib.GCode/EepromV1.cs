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

namespace CNCLib.GCode
{
    public class EepromV1
    {
        #region Properties

        public const uint SIGNATURE = 0x21436501;
        public const uint SIGNATUREPLOTTER = 0x21438701;

        UInt32[] _values;
        public UInt32[] Values
        {
            get => _values;
            set { _values = value; Analyse(); }
        }

        public bool IsValid => Values?.Length > 0 && (Values[0] == SIGNATURE || Values[0] == SIGNATUREPLOTTER);

        public uint OfsAxis => _ofsaxis;
        public uint DWSizeAxis => _sizeaxis;
        public uint OfsAfterAxis => _ofsAfterAxis;

        public const uint SIZEOFAXIX_EX = 2;

        public enum EValueOffsets32
		{
			Signatrue = 0,
			InfoOffset1,
			Info1,
			Info2,
			Values8Bit1,
			Values16Bit1,
			MaxStepRate,
			OffsetAccDec,
			RefMoveStepRate,
			MoveAwayFromRefernece,
			StepsPerMm1000
		}
		public enum EValueOffsets8
		{
			NumAxis = (EValueOffsets32.InfoOffset1 << 8) + 00,
			UseAxis = (EValueOffsets32.InfoOffset1 << 8) + 01,
            OfsOfAxis = (EValueOffsets32.InfoOffset1 << 8) + 02,
            SizeOfAxis = (EValueOffsets32.InfoOffset1 << 8) + 03,

            StepperDirection = (EValueOffsets32.Values8Bit1 << 8) + 0,
			SpindleFadeTime  = (EValueOffsets32.Values8Bit1 << 8) + 3
		}

		public enum EValueOffsets16
		{
			MaxSpindleSpeed = (EValueOffsets32.Values16Bit1 << 8) + 00,
			JerkSpeed		= (EValueOffsets32.Values16Bit1 << 8) + 01,

			Acc = (EValueOffsets32.OffsetAccDec << 8) + 00,
			Dec = (EValueOffsets32.OffsetAccDec << 8) + 01,
		}

		public enum EAxisOffsets32
		{
			Size = 0,
			Ofsett1,
            MaxStepRate,
            OffsetAccDec,
            RefMoveStepRate,
            StepsPerMm1000,
            ProbeSize,
        }

        public enum EAxisOffsets16
        {
            Acc = (EAxisOffsets32.OffsetAccDec << 8) + 00,
            Dec = (EAxisOffsets32.OffsetAccDec << 8) + 01,
        }

        public enum EAxisOffsets8
		{
			EReverenceType = (EAxisOffsets32.Ofsett1 << 8) + 00,
			EReverenceSeqence = (EAxisOffsets32.Ofsett1 << 8) + 1,
			EReverenceHitValueMin = (EAxisOffsets32.Ofsett1 << 8) + 2,
			EReverenceHitValueMax = (EAxisOffsets32.Ofsett1 << 8) + 3,
		}

        public enum EValueOffsets32Plotter
        { 
            EPenDownFeedrate,
            EPenUpFeedrate,

            EMovePenDownFeedrate,
            EMovePenUpFeedrate,
            EMovePenChangeFeedrate,

            EPenDownPos,
            EPenUpPos,

            EPenChangePosX,
            EPenChangePosY,
            EPenChangePosZ,

            EPenChangePosXOfs,
            EPenChangePosYOfs,

            EPenchangeServoClampPos,
            EPenchangeServoClampDelay
        }
        public enum EValueOffsets16Plotter
        {
            EPenchangeServoClampOpenPos = (EValueOffsets32Plotter.EPenchangeServoClampPos << 8) + 00,
            EPenchangeServoClampClosePos = (EValueOffsets32Plotter.EPenchangeServoClampPos << 8) + 01,

            EPenchangeServoClampOpenDelay  = (EValueOffsets32Plotter.EPenchangeServoClampDelay << 8) + 00,
            EPenchangeServoClampCloseDelay = (EValueOffsets32Plotter.EPenchangeServoClampDelay << 8) + 01,
        }

        [Flags]
		public enum EInfo1
		{
			EEPROM_INFO_SPINDLE = (1 << 0),
			EEPROM_INFO_SPINDLE_ANALOG = (1 << 1),
			EEPROM_INFO_SPINDLE_DIR = (1 << 2),
			EEPROM_INFO_COOLANT = (1 << 3),
			EEPROM_INFO_PROBE = (1 << 4),
			EEPROM_INFO_LASER = (1 << 5),

            EEPROM_INFO_COMMANDSYNTAX = (1 << 6), // (3bits)

            EEPROM_INFO_EEPROM = (1 << 9),
			EEPROM_INFO_SD = (1 << 10),
			EEPROM_INFO_ROTATE = (1 << 11),

			EEPROM_INFO_HOLDRESUME = (1 << 12),
			EEPROM_INFO_HOLD = (1 << 13),
			EEPROM_INFO_RESUME = (1 << 14),
			EEPROM_INFO_KILL = (1 << 15),

            EEPROM_INFO_NEED_EEPROM_FLUSH = (1 << 16),
            EEPROM_INFO_NEED_DTR = (1 << 17)
        }

        public static Logic.Contracts.DTO.CommandSyntax GetCommandSyntax(uint info1)
        {
            return (Logic.Contracts.DTO.CommandSyntax)((info1 >> 6) & 7);
        }
        #endregion

        #region Get/Set

        public UInt32 this[EValueOffsets32 ofs] { get => GetValue32(ofs);set => SetValue32(ofs, value);}
		public UInt16 this[EValueOffsets16 ofs] { get => GetValue16(ofs);set => SetValue16(ofs, value);}
		public byte this[EValueOffsets8 ofs] { get => GetValue8(ofs);set => SetValue8(ofs, value);}
		public UInt32 this[int axis, EAxisOffsets32 ofs] { get => GetAxisValue32(axis,ofs);set => SetAxisValue32(axis, ofs, value);}
        public UInt16 this[int axis, EAxisOffsets16 ofs] { get => GetAxisValue16(axis, ofs);set => SetAxisValue16(axis, ofs, value);}
        public byte this[int axis, EAxisOffsets8 ofs] { get => GetAxisValue8(axis,ofs);set => SetAxisValue8(axis,ofs, value);}

        private EValueOffsets32 AddPlotterOfs(EValueOffsets32Plotter ofs)
        {
            return (EValueOffsets32) ((uint) ofs + _ofsAfterAxis);
        }
        private EValueOffsets16 AddPlotterOfs(EValueOffsets16Plotter ofs)
        {
            return (EValueOffsets16)((uint)ofs + (_ofsAfterAxis<<8));
        }
        public UInt32 this[EValueOffsets32Plotter ofs] { get => GetValue32(AddPlotterOfs(ofs)); set => SetValue32(AddPlotterOfs(ofs), value);}
        public UInt16 this[EValueOffsets16Plotter ofs] { get => GetValue16(AddPlotterOfs(ofs)); set => SetValue16(AddPlotterOfs(ofs), value);}

        public List<string> ToGCode()
		{
			var list = new List<string>();
			for (int slot = 2;slot<Values.Length;slot++)
			{
				list.Add($"${slot}={Values[slot]}");
			}
			return list;
		}

		Tuple<EValueOffsets32,int> GetIndex(EValueOffsets16 ofsidx)
		{
			return new Tuple<EValueOffsets32, int>((EValueOffsets32)(((int)ofsidx >> 8) & 0xff), (int)ofsidx & 0xff);
		}
		Tuple<EValueOffsets32, int> GetIndex(EValueOffsets8 ofsidx)
		{
			return new Tuple<EValueOffsets32, int>((EValueOffsets32)(((int)ofsidx >> 8) & 0xff), (int)ofsidx & 0xff);
		}
        Tuple<EAxisOffsets32, int> GetIndex(EAxisOffsets16 ofsidx)
        {
            return new Tuple<EAxisOffsets32, int>((EAxisOffsets32)(((int)ofsidx >> 8) & 0xff), (int)ofsidx & 0xff);
        }

        Tuple<EAxisOffsets32, int> GetIndex(EAxisOffsets8 ofsidx)
		{
			return new Tuple<EAxisOffsets32, int>((EAxisOffsets32)(((int)ofsidx >> 8) & 0xff), (int)ofsidx & 0xff);
		}

		#endregion

		#region Read

		public UInt32 GetValue32(EValueOffsets32 ofs)
		{
			return Values[(int)ofs];
		}

		public ushort GetValue16(EValueOffsets16 ofsidx)
		{
			var offsets = GetIndex(ofsidx);

			UInt32 val = GetValue32(offsets.Item1);
			return (ushort)((val >> (offsets.Item2 * 16)) & 0xffff);
		}

		public byte GetValue8(EValueOffsets8 ofsidx)
		{
			var offsets = GetIndex(ofsidx);

			UInt32 val = GetValue32(offsets.Item1);
			return (byte)((val >> (offsets.Item2 * 8)) & 0xff);
		}

		public UInt32 GetAxisValue32(int axis, EAxisOffsets32 ofs)
		{
			return Values[_ofsaxis + axis * _sizeaxis + (int)ofs];
		}
        public ushort GetAxisValue16(int axis, EAxisOffsets16 ofsidx)
        {
            var offsets = GetIndex(ofsidx);

            UInt32 val = GetAxisValue32(axis,offsets.Item1);
            return (ushort)((val >> (offsets.Item2 * 16)) & 0xffff);
        }

        public byte GetAxisValue8(int axis, EAxisOffsets8 ofsidx)
		{
			var offsets = GetIndex(ofsidx);

			UInt32 val = GetAxisValue32(axis, offsets.Item1);
			return (byte)((val >> (offsets.Item2 * 8)) & 0xff);
		}

		#endregion

		#region Write

		public void SetValue32(EValueOffsets32 ofs, UInt32 value)
		{
			Values[(int)ofs] = value;
		}

		public void SetValue8(EValueOffsets8 ofsidx, byte value)
		{
			var offsets = GetIndex(ofsidx);

			UInt32 mask = ((UInt32)0xff) << (offsets.Item2 * 8);
			UInt32 val = GetValue32(offsets.Item1) & (~mask);

			SetValue32(offsets.Item1, val + ((UInt32)value << (offsets.Item2 * 8)));
		}

		public void SetValue16(EValueOffsets16 ofsidx, ushort value)
		{
			var offsets = GetIndex(ofsidx);

			UInt32 mask = ((UInt32)0xffff) << (offsets.Item2 * 16);
			UInt32 val = GetValue32(offsets.Item1) & (~mask);

			SetValue32(offsets.Item1, val + ((UInt32)value << (offsets.Item2 * 16)));
		}

		public void SetAxisValue32(int axis, EAxisOffsets32 ofs, UInt32 value)
		{
			Values[_ofsaxis + axis * _sizeaxis + (int)ofs] = value;
		}

        public void SetAxisValue16(int axis, EAxisOffsets16 ofsidx, ushort value)
        {
            var offsets = GetIndex(ofsidx);

            UInt32 mask = ((UInt32)0xffff) << (offsets.Item2 * 16);
            UInt32 val = GetAxisValue32(axis, offsets.Item1) & (~mask);

            SetAxisValue32(axis, offsets.Item1, val + ((UInt32)value << (offsets.Item2 * 16)));
        }

        public void SetAxisValue8(int axis, EAxisOffsets8 ofsidx, byte value)
		{
			var offsets = GetIndex(ofsidx);

			UInt32 mask = ((UInt32)0xff) << (offsets.Item2 * 8);
			UInt32 val = GetAxisValue32(axis, offsets.Item1) & (~mask);
			SetAxisValue32(axis, offsets.Item1, val + ((UInt32)value << (offsets.Item2 * 8)));
		}

		#endregion

		#region Analyse

		uint _num_axis;
		uint _used_axis;
		uint _ofsaxis;
		uint _sizeaxis;
        uint _ofsAfterAxis;

		private void Analyse()
		{
			if (Values?.Length > 0)
			{
				_num_axis = (Values[1] >> 0) & 0xff;
				_used_axis = (Values[1] >> 8) & 0xff;
				_ofsaxis = ((Values[1] >> 16) & 0xff) / sizeof(UInt32);
				_sizeaxis = ((Values[1] >> 24) & 0xff) / sizeof(UInt32);
                _ofsAfterAxis = _ofsaxis + _sizeaxis * _num_axis;
            }
		}

		#endregion
	}
}
