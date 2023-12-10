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

namespace CNCLib.GCode.Machine;

using System;
using System.Collections.Generic;

public abstract class EepromV0
{
    #region Properties

    uint[] _values = Array.Empty<uint>();

    public uint[] Values
    {
        get => _values;
        set
        {
            _values = value;
            Analyse();
        }
    }

    public abstract void ReadFrom(Eeprom eeprom);

    public void ReadFromDefault(Eeprom eeprom)
    {
        eeprom.Signature = GetValue32((uint)EepromV0.EValueOffsets32.Signature);

        eeprom.Info1 = GetValue32((uint)EValueOffsets32.Info1);
        eeprom.Info2 = GetValue32((uint)EValueOffsets32.Info2);
    }

    public abstract void WriteTo(Eeprom eeprom);

    public uint Signature => Values.Length > 0 ? Values[0] : 0;

    public abstract uint VersionSignature { get; }

    public bool IsValid => Values.Length > 0 && (Values[0] == Signature || Values[0] == VersionSignature);

    public uint OfsAxis      => _ofsAxis;
    public uint DWSizeAxis   => _sizeAxis;
    public uint OfsAfterAxis => _ofsAfterAxis;

    public enum EValueOffsets32
    {
        Signature = 0,
        InfoOffset1,
        Info1,
        Info2,
        FirstCustomV0
    }

    public enum EValueOffsets8
    {
        NumAxis    = (EValueOffsets32.InfoOffset1 << 8) + 00,
        UseAxis    = (EValueOffsets32.InfoOffset1 << 8) + 01,
        OfsOfAxis  = (EValueOffsets32.InfoOffset1 << 8) + 02,
        SizeOfAxis = (EValueOffsets32.InfoOffset1 << 8) + 03,
    }

    public enum EAxisOffsets32
    {
        Size = 0,
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

        EEPROM_INFO_WORKOFFSETCOUT = (1 << 19), // 4 bits
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

    public List<string> ToGCode()
    {
        var list = new List<string>();
        for (int slot = 2; slot < Values.Length; slot++)
        {
            list.Add($"${slot}={Values[slot]}");
        }

        return list;
    }

    Tuple<uint, uint> GetIndex(uint ofsIdx)
    {
        return new Tuple<uint, uint>((uint)(((int)ofsIdx >> 8) & 0xff), (uint)ofsIdx & 0xff);
    }

    #endregion

    #region Read

    public uint GetValue32(uint ofs)
    {
        return Values[(int)ofs];
    }

    public ushort GetValue16(uint ofsIdx)
    {
        var offsets = GetIndex(ofsIdx);

        uint val = GetValue32(offsets.Item1);
        return (ushort)((val >> (int)(offsets.Item2 * 16)) & 0xffff);
    }

    public byte GetValue8(uint ofsIdx)
    {
        var offsets = GetIndex(ofsIdx);

        uint val = GetValue32(offsets.Item1);
        return (byte)((val >> (int)(offsets.Item2 * 8)) & 0xff);
    }

    public uint GetAxisValue32(int axis, uint ofs)
    {
        return Values[_ofsAxis + axis * _sizeAxis + (int)ofs];
    }

    public ushort GetAxisValue16(int axis, uint ofsIdx)
    {
        var offsets = GetIndex(ofsIdx);

        uint val = GetAxisValue32(axis, offsets.Item1);
        return (ushort)((val >> (int)(offsets.Item2 * 16)) & 0xffff);
    }

    public byte GetAxisValue8(int axis, uint ofsIdx)
    {
        var offsets = GetIndex(ofsIdx);

        uint val = GetAxisValue32(axis, offsets.Item1);
        return (byte)((val >> (int)(offsets.Item2 * 8)) & 0xff);
    }

    #endregion

    #region Write

    public void SetValue32(uint ofs, uint value)
    {
        Values[(int)ofs] = value;
    }

    public void SetValue8(uint ofsIdx, byte value)
    {
        var offsets = GetIndex(ofsIdx);

        uint mask = ((uint)0xff) << (int)(offsets.Item2 * 8);
        uint val  = GetValue32(offsets.Item1) & (~mask);

        SetValue32(offsets.Item1, val + ((uint)value << (int)(offsets.Item2 * 8)));
    }

    public void SetValue16(uint ofsIdx, ushort value)
    {
        var offsets = GetIndex(ofsIdx);

        uint mask = ((uint)0xffff) << (int)(offsets.Item2 * 16);
        uint val  = GetValue32(offsets.Item1) & (~mask);

        SetValue32(offsets.Item1, val + ((uint)value << (int)(offsets.Item2 * 16)));
    }

    public void SetAxisValue32(int axis, uint ofs, uint value)
    {
        Values[_ofsAxis + axis * _sizeAxis + (int)ofs] = value;
    }

    public void SetAxisValue16(int axis, uint ofsIdx, ushort value)
    {
        var offsets = GetIndex(ofsIdx);

        uint mask = ((uint)0xffff) << (int)(offsets.Item2 * 16);
        uint val  = GetAxisValue32(axis, offsets.Item1) & (~mask);

        SetAxisValue32(axis, offsets.Item1, val + ((uint)value << (int)(offsets.Item2 * 16)));
    }

    public void SetAxisValue8(int axis, uint ofsIdx, byte value)
    {
        var offsets = GetIndex(ofsIdx);

        uint mask = ((uint)0xff) << (int)(offsets.Item2 * 8);
        uint val  = GetAxisValue32(axis, offsets.Item1) & (~mask);
        SetAxisValue32(axis, offsets.Item1, val + ((uint)value << (int)(offsets.Item2 * 8)));
    }

    #endregion

    #region Analyse

    uint _numAxis;
    uint _usedAxis;
    uint _ofsAxis;
    uint _sizeAxis;
    uint _ofsAfterAxis;

    protected virtual void Analyse()
    {
        if (Values?.Length > 0)
        {
            _numAxis      = (Values[(int)EValueOffsets32.InfoOffset1] >> 0) & 0xff;
            _usedAxis     = (Values[(int)EValueOffsets32.InfoOffset1] >> 8) & 0xff;
            _ofsAxis      = ((Values[(int)EValueOffsets32.InfoOffset1] >> 16) & 0xff) / sizeof(uint);
            _sizeAxis     = ((Values[(int)EValueOffsets32.InfoOffset1] >> 24) & 0xff) / sizeof(uint);
            _ofsAfterAxis = _ofsAxis + _sizeAxis * _numAxis;
        }
    }

    #endregion
}