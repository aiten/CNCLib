////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2014 Herbert Aitenbichler

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
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using CNCLib.Wpf.Models;
using Framework.Tools.Dependency;
using CNCLib.ServiceProxy;
using System.Threading.Tasks;
using CNCLib.Wpf.Helpers;

namespace CNCLib.Wpf.ViewModels
{
    public class EepromViewModel : BaseViewModel, IDisposable
	{
		#region crt

		public EepromViewModel()
		{
		}

		#endregion

		#region dispose

		public void Dispose()
		{
		}

		#endregion

		#region Properties

		private Eeprom _eeprom = new Eeprom();
		public Eeprom EepromValue
		{
			get { return _eeprom; }
			set { SetProperty(() => _eeprom == value, () => _eeprom = value); }
		}
		private Framework.Arduino.ArduinoSerialCommunication Com
		{
			get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
		}

		#endregion

		#region Operations


		public void WriteEeprom()
		{
            CloseAction();
        }

		class EpromValues
		{
			UInt32[] _values;
			public UInt32[] Values
			{	get { return _values; }
				set { _values = value; Analyse(); } 
			}

			public bool IsValid(UInt32 signature)
			{
				return Values?.Length > 0 && Values[0] == signature;
			}
			public UInt32 Value(uint ofs)
			{
				return Values[ofs];
			}

			public byte Value(uint ofs, int idx)
			{
				UInt32 val = Value(ofs);
				return (byte)((val >> (idx * 8)) & 0xff);
			}

			public UInt32 AxisValue(uint axis, uint ofs)
			{
				return Values[_ofsaxis + axis * _sizeaxis + ofs];
			}
			public byte AxisValue(uint axis, uint ofs, int idx)
			{
				UInt32 val = AxisValue(axis, ofs);
				return (byte)((val >> (idx*8)) & 0xff);
			}

			uint _num_axis;
			uint _used_axis;
			uint _ofsaxis;
			uint _sizeaxis;

			private void Analyse()
			{
				if (Values?.Length > 0)
				{
					_num_axis = (Values[1] >> 0) & 0xff;
					_used_axis = (Values[1] >> 8) & 0xff;
					_ofsaxis = ((Values[1] >> 16) & 0xff) / sizeof(UInt32);
					_sizeaxis = ((Values[1] >> 24) & 0xff) / sizeof(UInt32);
				}
			}
		}

		public void ReadEeprom()
		{
			Task.Run(async () =>
			{
				UInt32[] values = await new MachineGCodeHelper().GetEpromValuesAsync();
				if (values != null)
				{
					var eeprom = new Eeprom() { Values = values };
					var ee = new EpromValues() { Values = values };

					if (ee.IsValid(0x21436587))
					{
						//	struct SCNCEeprom
						//		{
						//			uint32_t signature;

						//			uint8_t num_axis;
						//			uint8_t used_axis;
						//			uint8_t offsetAxis;
						//			uint8_t sizeofAxis;

						//			uint32_t info;

						//			uint32_t maxsteprate;
						//			uint32_t acc;
						//			uint32_t dec;
						//			uint32_t refmovesteprate;

						//			float ScaleMm1000ToMachine;

						//			struct SAxisDefinitions
						//			{
						//				mm1000_t size;

						//				uint8_t referenceType;      // EReverenceType
						//				uint8_t refmoveSequence;

						//				uint8_t dummy2;
						//				uint8_t dummy3;

						//# ifndef REDUCED_SIZE

						//				uint32_t maxsteprate;
						//				uint32_t acc;
						//				uint32_t dec;
						//				uint32_t refmovesteprate;

						//				float ScaleMm1000ToMachine;
						//#endif

						//			}
						//			axis[EEPROM_NUM_AXIS];
						//	};

						//$0 = 558065031(21436587)
						//$1 = 136315396(8200204)
						//$2 = 0(0)
						//$3 = 20000(4E20)
						//$4 = 500(1F4)
						//$5 = 550(226)
						//$6 = 20000(4E20)
						//$7 = 1091960832(41160000)
						//$8 = 36000(8CA0)
						//$9 = 65280(FF00)
						//$10 = 36000(8CA0)
						//$11 = 65280(FF00)
						//$12 = 10000(2710)
						//$13 = 65280(FF00)
						//$14 = 50000(C350)
						//$15 = 65280(FF00)

						eeprom.NumAxis = ee.Value(1, 0);
						eeprom.UseAxis = ee.Value(1, 1);

						eeprom.Info = ee.Value(2);

						eeprom.SizeX = ee.AxisValue(0, 0);
						eeprom.SizeY = ee.AxisValue(1, 0);
						eeprom.SizeZ = ee.AxisValue(2, 0);
						eeprom.SizeA = ee.AxisValue(3, 0);

						eeprom.RefMoveX = (Eeprom.EReverenceType)ee.AxisValue(0, 1, 0);
						eeprom.RefMoveY = (Eeprom.EReverenceType)ee.AxisValue(1, 1, 0);
						eeprom.RefMoveZ = (Eeprom.EReverenceType)ee.AxisValue(2, 1, 0);
						eeprom.RefMoveA = (Eeprom.EReverenceType)ee.AxisValue(3, 1, 0);

						eeprom.RefSeqence0 = (Eeprom.EReverenceSequence)ee.AxisValue(0, 1, 1);
						eeprom.RefSeqence1 = (Eeprom.EReverenceSequence)ee.AxisValue(1, 1, 1);
						eeprom.RefSeqence2 = (Eeprom.EReverenceSequence)ee.AxisValue(2, 1, 1);
						eeprom.RefSeqence3 = (Eeprom.EReverenceSequence)ee.AxisValue(3, 1, 1);
						eeprom.RefMoveSteprate = ee.Value(6);

						eeprom.MaxStepRate = ee.Value(3);
						eeprom.Acc = ee.Value(4);
						eeprom.Dec = ee.Value(5);
						eeprom.ScaleMMToMachine = BitConverter.ToSingle(BitConverter.GetBytes(ee.Value(7)), 0);
					}

					EepromValue = eeprom;
				}
			});
		}

		public bool CanReadEeprom()
		{
			return Com.IsConnected;
		}

		public bool CanWriteEeprom()
		{
			return Com.IsConnected;
		}

		#endregion

		#region Commands

		public ICommand ReadEepromCommand => new DelegateCommand(ReadEeprom, CanReadEeprom);
        public ICommand WriteEepromCommand => new DelegateCommand(WriteEeprom, CanWriteEeprom);

        #endregion
    }
}
