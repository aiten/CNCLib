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

namespace CNCLib.GCode.Machine
{
    using System;
    using System.Collections.Generic;

    public class EepromV1
    {
        #region Properties

        public const uint SIGNATURE        = 0x21436502;
        public const uint SIGNATUREPLOTTER = 0x21438702;

        uint[] _values;

        public uint[] Values
        {
            get => _values;
            set
            {
                _values = value;
                Analyse();
            }
        }

        public bool IsValid => Values?.Length > 0 && (Values[0] == SIGNATURE || Values[0] == SIGNATUREPLOTTER);

        public uint OfsAxis      => _ofsAxis;
        public uint DWSizeAxis   => _sizeAxis;
        public uint OfsAfterAxis => _ofsAfterAxis;

        public const uint SIZEOFAXIS_EX = ((uint)EAxisOffsets32.InitPosition) + 1;

        public enum EValueOffsets32
        {
            Signature = 0,
            InfoOffset1,
            Info1,
            Info2,
            Values8Bit1,
            Values16Bit1,
            MaxStepRate,
            OffsetAccDec,
            RefMoveStepRate,
            MoveAwayFromReference,
            StepsPerMm1000
        }

        public enum EValueOffsets8
        {
            NumAxis    = (EValueOffsets32.InfoOffset1 << 8) + 00,
            UseAxis    = (EValueOffsets32.InfoOffset1 << 8) + 01,
            OfsOfAxis  = (EValueOffsets32.InfoOffset1 << 8) + 02,
            SizeOfAxis = (EValueOffsets32.InfoOffset1 << 8) + 03,

            StepperDirection = (EValueOffsets32.Values8Bit1 << 8) + 0,
            SpindleFadeTime  = (EValueOffsets32.Values8Bit1 << 8) + 3
        }

        public enum EValueOffsets16
        {
            MaxSpindleSpeed = (EValueOffsets32.Values16Bit1 << 8) + 00,
            JerkSpeed       = (EValueOffsets32.Values16Bit1 << 8) + 01,

            Acc = (EValueOffsets32.OffsetAccDec << 8) + 00,
            Dec = (EValueOffsets32.OffsetAccDec << 8) + 01
        }

        public enum EAxisOffsets32
        {
            Size = 0,
            Offset1,
            InitPosition,
            MaxStepRate,
            OffsetAccDec,
            RefMoveStepRate,
            StepsPerMm1000,
            ProbeSize
        }

        public enum EAxisOffsets16
        {
            Acc = (EAxisOffsets32.OffsetAccDec << 8) + 00,
            Dec = (EAxisOffsets32.OffsetAccDec << 8) + 01
        }

        public enum EAxisOffsets8
        {
            EReverenceType        = (EAxisOffsets32.Offset1 << 8) + 00,
            EReverenceSequence    = (EAxisOffsets32.Offset1 << 8) + 1,
            EReverenceHitValueMin = (EAxisOffsets32.Offset1 << 8) + 2,
            EReverenceHitValueMax = (EAxisOffsets32.Offset1 << 8) + 3
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

            EPenChangeServoClampPos,
            EPenChangeServoClampDelay
        }

        public enum EValueOffsets16Plotter
        {
            EPenChangeServoClampOpenPos  = (EValueOffsets32Plotter.EPenChangeServoClampPos << 8) + 00,
            EPenChangeServoClampClosePos = (EValueOffsets32Plotter.EPenChangeServoClampPos << 8) + 01,

            EPenChangeServoClampOpenDelay  = (EValueOffsets32Plotter.EPenChangeServoClampDelay << 8) + 00,
            EPenChangeServoClampCloseDelay = (EValueOffsets32Plotter.EPenChangeServoClampDelay << 8) + 01
        }

        [Flags]
        public enum EInfo1
        {
            EEPROM_INFO_SPINDLE        = (1 << 0),
            EEPROM_INFO_SPINDLE_ANALOG = (1 << 1),
            EEPROM_INFO_SPINDLE_DIR    = (1 << 2),
            EEPROM_INFO_COOLANT        = (1 << 3),
            EEPROM_INFO_PROBE          = (1 << 4),
            EEPROM_INFO_LASER          = (1 << 5),

            EEPROM_INFO_COMMANDSYNTAX = (1 << 6), // (3bits)

            EEPROM_INFO_EEPROM = (1 << 9),
            EEPROM_INFO_SD     = (1 << 10),
            EEPROM_INFO_ROTATE = (1 << 11),

            EEPROM_INFO_HOLDRESUME = (1 << 12),
            EEPROM_INFO_HOLD       = (1 << 13),
            EEPROM_INFO_RESUME     = (1 << 14),
            EEPROM_INFO_KILL       = (1 << 15),

            EEPROM_INFO_NEED_EEPROM_FLUSH = (1 << 16),
            EEPROM_INFO_NEED_DTR          = (1 << 17), // deprecated
            EEPROM_INFO_DTR_IS_RESET      = (1 << 18),

            EEPROM_INFO_WORKOFFSETCOUT        = (1 << 19), // 4 bits
        }

        public static int GetCommandSyntax(uint info1)
        {
            return (int)((info1 >> 6) & 7);
        }

        public static uint GetWorkOffsetCount(uint info1)
        {
            return (uint)((info1 >> 19) & 15);
        }

        #endregion

        #region Get/Set

        public uint this[EValueOffsets32 ofs] { get => GetValue32(ofs); set => SetValue32(ofs, value); }

        public ushort this[EValueOffsets16 ofs] { get => GetValue16(ofs); set => SetValue16(ofs, value); }

        public byte this[EValueOffsets8 ofs] { get => GetValue8(ofs); set => SetValue8(ofs, value); }

        public uint this[int axis, EAxisOffsets32 ofs] { get => GetAxisValue32(axis, ofs); set => SetAxisValue32(axis, ofs, value); }

        public ushort this[int axis, EAxisOffsets16 ofs] { get => GetAxisValue16(axis, ofs); set => SetAxisValue16(axis, ofs, value); }

        public byte this[int axis, EAxisOffsets8 ofs] { get => GetAxisValue8(axis, ofs); set => SetAxisValue8(axis, ofs, value); }

        private EValueOffsets32 AddPlotterOfs(EValueOffsets32Plotter ofs)
        {
            return (EValueOffsets32)((uint)ofs + _ofsAfterAxis);
        }

        private EValueOffsets16 AddPlotterOfs(EValueOffsets16Plotter ofs)
        {
            return (EValueOffsets16)((uint)ofs + (_ofsAfterAxis << 8));
        }

        public uint this[EValueOffsets32Plotter ofs] { get => GetValue32(AddPlotterOfs(ofs)); set => SetValue32(AddPlotterOfs(ofs), value); }

        public ushort this[EValueOffsets16Plotter ofs] { get => GetValue16(AddPlotterOfs(ofs)); set => SetValue16(AddPlotterOfs(ofs), value); }

        public List<string> ToGCode()
        {
            var list = new List<string>();
            for (int slot = 2; slot < Values.Length; slot++)
            {
                list.Add($"${slot}={Values[slot]}");
            }

            return list;
        }

        Tuple<EValueOffsets32, int> GetIndex(EValueOffsets16 ofsIdx)
        {
            return new Tuple<EValueOffsets32, int>((EValueOffsets32)(((int)ofsIdx >> 8) & 0xff), (int)ofsIdx & 0xff);
        }

        Tuple<EValueOffsets32, int> GetIndex(EValueOffsets8 ofsIdx)
        {
            return new Tuple<EValueOffsets32, int>((EValueOffsets32)(((int)ofsIdx >> 8) & 0xff), (int)ofsIdx & 0xff);
        }

        Tuple<EAxisOffsets32, int> GetIndex(EAxisOffsets16 ofsIdx)
        {
            return new Tuple<EAxisOffsets32, int>((EAxisOffsets32)(((int)ofsIdx >> 8) & 0xff), (int)ofsIdx & 0xff);
        }

        Tuple<EAxisOffsets32, int> GetIndex(EAxisOffsets8 ofsIdx)
        {
            return new Tuple<EAxisOffsets32, int>((EAxisOffsets32)(((int)ofsIdx >> 8) & 0xff), (int)ofsIdx & 0xff);
        }

        #endregion

        #region Read

        public uint GetValue32(EValueOffsets32 ofs)
        {
            return Values[(int)ofs];
        }

        public ushort GetValue16(EValueOffsets16 ofsIdx)
        {
            var offsets = GetIndex(ofsIdx);

            uint val = GetValue32(offsets.Item1);
            return (ushort)((val >> (offsets.Item2 * 16)) & 0xffff);
        }

        public byte GetValue8(EValueOffsets8 ofsIdx)
        {
            var offsets = GetIndex(ofsIdx);

            uint val = GetValue32(offsets.Item1);
            return (byte)((val >> (offsets.Item2 * 8)) & 0xff);
        }

        public uint GetAxisValue32(int axis, EAxisOffsets32 ofs)
        {
            return Values[_ofsAxis + axis * _sizeAxis + (int)ofs];
        }

        public ushort GetAxisValue16(int axis, EAxisOffsets16 ofsIdx)
        {
            var offsets = GetIndex(ofsIdx);

            uint val = GetAxisValue32(axis, offsets.Item1);
            return (ushort)((val >> (offsets.Item2 * 16)) & 0xffff);
        }

        public byte GetAxisValue8(int axis, EAxisOffsets8 ofsIdx)
        {
            var offsets = GetIndex(ofsIdx);

            uint val = GetAxisValue32(axis, offsets.Item1);
            return (byte)((val >> (offsets.Item2 * 8)) & 0xff);
        }

        #endregion

        #region Write

        public void SetValue32(EValueOffsets32 ofs, uint value)
        {
            Values[(int)ofs] = value;
        }

        public void SetValue8(EValueOffsets8 ofsIdx, byte value)
        {
            var offsets = GetIndex(ofsIdx);

            uint mask = ((uint)0xff) << (offsets.Item2 * 8);
            uint val  = GetValue32(offsets.Item1) & (~mask);

            SetValue32(offsets.Item1, val + ((uint)value << (offsets.Item2 * 8)));
        }

        public void SetValue16(EValueOffsets16 ofsIdx, ushort value)
        {
            var offsets = GetIndex(ofsIdx);

            uint mask = ((uint)0xffff) << (offsets.Item2 * 16);
            uint val  = GetValue32(offsets.Item1) & (~mask);

            SetValue32(offsets.Item1, val + ((uint)value << (offsets.Item2 * 16)));
        }

        public void SetAxisValue32(int axis, EAxisOffsets32 ofs, uint value)
        {
            Values[_ofsAxis + axis * _sizeAxis + (int)ofs] = value;
        }

        public void SetAxisValue16(int axis, EAxisOffsets16 ofsIdx, ushort value)
        {
            var offsets = GetIndex(ofsIdx);

            uint mask = ((uint)0xffff) << (offsets.Item2 * 16);
            uint val  = GetAxisValue32(axis, offsets.Item1) & (~mask);

            SetAxisValue32(axis, offsets.Item1, val + ((uint)value << (offsets.Item2 * 16)));
        }

        public void SetAxisValue8(int axis, EAxisOffsets8 ofsIdx, byte value)
        {
            var offsets = GetIndex(ofsIdx);

            uint mask = ((uint)0xff) << (offsets.Item2 * 8);
            uint val  = GetAxisValue32(axis, offsets.Item1) & (~mask);
            SetAxisValue32(axis, offsets.Item1, val + ((uint)value << (offsets.Item2 * 8)));
        }

        #endregion

        #region Analyse

        uint _numAxis;
        uint _usedAxis;
        uint _ofsAxis;
        uint _sizeAxis;
        uint _ofsAfterAxis;

        private void Analyse()
        {
            if (Values?.Length > 0)
            {
                _numAxis      = (Values[1] >> 0) & 0xff;
                _usedAxis     = (Values[1] >> 8) & 0xff;
                _ofsAxis      = ((Values[1] >> 16) & 0xff) / sizeof(uint);
                _sizeAxis     = ((Values[1] >> 24) & 0xff) / sizeof(uint);
                _ofsAfterAxis = _ofsAxis + _sizeAxis * _numAxis;
            }
        }

        #endregion
    }
}