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
		public void ReadEeprom()
		{
			Task.Run(async () =>
			{
				UInt32[] values = await new MachineGCodeHelper().GetEpromValuesAsync();
				if (values != null)
				{
					var eeprom = new Eeprom() { Values = values };

					if (values[0] == 0x21436587)
					{
						//	struct SCNCEeprom
						//		{
						//			uint32_t signature;

						//			uint8_t refmove[EEPROM_NUM_AXIS];

						//			uint32_t maxsteprate;
						//			uint32_t acc;
						//			uint32_t dec;
						//			uint32_t refmovesteprate;

						//			float ScaleMm1000ToMachine;

						//			struct SAxisDefinitions
						//			{
						//				mm1000_t size;

						//				uint8_t referenceType;  // EReverenceType
						//				uint8_t dummy1;
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
						//$1 = 4294901761(FFFF0001)
						//$2 = 27000(6978)
						//$3 = 350(15E)
						//$4 = 400(190)
						//$5 = 4000(FA0)
						//$6 = 1091960832(41160000)
						//$7 = 400000(61A80)
						//$8 = 1(1)
						//$9 = 380000(5CC60)
						//$10 = 1(1)
						//$11 = 100000(186A0)
						//$12 = 0(0)
						//$13 = 50000(C350)
						//$14 = 0(0)

						eeprom.SizeX = values[7];
						eeprom.SizeY = values[9];
						eeprom.SizeZ = values[11];
						eeprom.SizeA = values[13];

						eeprom.RefMoveX = (Eeprom.EReverenceType)(values[8] & 0xff);
						eeprom.RefMoveY = (Eeprom.EReverenceType)(values[10] & 0xff);
						eeprom.RefMoveZ = (Eeprom.EReverenceType)(values[12] & 0xff);
						eeprom.RefMoveA = (Eeprom.EReverenceType)(values[14] & 0xff);

						eeprom.RefSeqence0 = (Eeprom.EReverenceSequence)(values[1] & 0xff);
						eeprom.RefSeqence1 = (Eeprom.EReverenceSequence)((values[1]>>8) & 0xff);
						eeprom.RefSeqence2 = (Eeprom.EReverenceSequence)((values[1] >> 16) & 0xff);
						eeprom.RefSeqence3 = (Eeprom.EReverenceSequence)((values[1] >> 24) & 0xff);
						eeprom.RefMoveSteprate = values[5];

						eeprom.MaxStepRate = values[2];
						eeprom.Acc = values[3];
						eeprom.Dec = values[4];
						eeprom.ScaleMMToMachine = BitConverter.ToSingle(BitConverter.GetBytes(values[6]), 0);
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
