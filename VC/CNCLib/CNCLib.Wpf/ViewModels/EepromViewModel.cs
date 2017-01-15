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
using CNCLib.GCode;
using System.IO;
using System.Windows;

namespace CNCLib.Wpf.ViewModels
{
    public class EepromViewModel : BaseViewModel, IDisposable
	{
		#region crt

		public EepromViewModel()
		{
		}

		bool _validReadEeprom;

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
			if (MessageBox?.Invoke("Send 'Write EEprom commands' to machine?", "CNCLib", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				var ee = new EepromV1() { Values = EepromValue.Values };

				if (ee.IsValid)
				{
					//ee[EepromV1.EValueOffsets8.NumAxis] = EepromValue.NumAxis;
					//ee[EepromV1.EValueOffsets8.UseAxis] = EepromValue.UseAxis;

					//eeprom.Info = ee[EepromV1.EValueOffsets32.Info];

					ee[0, EepromV1.EAxisOffsets32.Size] = EepromValue.SizeX;
					ee[1, EepromV1.EAxisOffsets32.Size] = EepromValue.SizeY;
					ee[2, EepromV1.EAxisOffsets32.Size] = EepromValue.SizeZ;
					ee[3, EepromV1.EAxisOffsets32.Size] = EepromValue.SizeA;

					ee[0, EepromV1.EAxisOffsets8.EReverenceType] = (byte)EepromValue.RefMoveX;
					ee[1, EepromV1.EAxisOffsets8.EReverenceType] = (byte)EepromValue.RefMoveY;
					ee[2, EepromV1.EAxisOffsets8.EReverenceType] = (byte)EepromValue.RefMoveZ;
					ee[3, EepromV1.EAxisOffsets8.EReverenceType] = (byte)EepromValue.RefMoveA;

					ee[0, EepromV1.EAxisOffsets8.EReverenceSeqence] = (byte)EepromValue.RefSeqence0;
					ee[1, EepromV1.EAxisOffsets8.EReverenceSeqence] = (byte)EepromValue.RefSeqence1;
					ee[2, EepromV1.EAxisOffsets8.EReverenceSeqence] = (byte)EepromValue.RefSeqence2;
					ee[3, EepromV1.EAxisOffsets8.EReverenceSeqence] = (byte)EepromValue.RefSeqence3;
					ee[EepromV1.EValueOffsets32.RefMoveStepRate] = EepromValue.RefMoveSteprate;
					ee[EepromV1.EValueOffsets32.MoveAwayFromRefernece] = EepromValue.MoveAwayFromRefernece;

					ee[EepromV1.EValueOffsets32.MaxstepRate] = EepromValue.MaxStepRate;
					ee[EepromV1.EValueOffsets32.Acc] = EepromValue.Acc;
					ee[EepromV1.EValueOffsets32.Dec] = EepromValue.Dec;
					ee[EepromV1.EValueOffsets32.StepsPerMm1000] = BitConverter.ToUInt32(BitConverter.GetBytes(EepromValue.StepsPerMm1000), 0);

					File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EEpromWrite.nc"), ee.ToGCode());

					Task.Run(async () =>
					{
						await Com.SendCommandAsync(@"$!");
						await Com.SendCommandsAsync(ee.ToGCode());
					});
				}

				CloseAction();
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
					var ee = new EepromV1() { Values = values };

					if (ee.IsValid)
					{

						File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EEpromRead.nc"), ee.ToGCode());

						eeprom.NumAxis = ee[EepromV1.EValueOffsets8.NumAxis];
						eeprom.UseAxis = ee[EepromV1.EValueOffsets8.UseAxis];

						eeprom.Info1	= ee[EepromV1.EValueOffsets32.Info1];
						eeprom.Info2 = ee[EepromV1.EValueOffsets32.Info2];

						eeprom.SizeX = ee[0, EepromV1.EAxisOffsets32.Size];
						eeprom.SizeY = ee[1, EepromV1.EAxisOffsets32.Size];
						eeprom.SizeZ = ee[2, EepromV1.EAxisOffsets32.Size];
						eeprom.SizeA = ee[3, EepromV1.EAxisOffsets32.Size];

						eeprom.RefMoveX = (Eeprom.EReverenceType)ee[0, EepromV1.EAxisOffsets8.EReverenceType];
						eeprom.RefMoveY = (Eeprom.EReverenceType)ee[1, EepromV1.EAxisOffsets8.EReverenceType];
						eeprom.RefMoveZ = (Eeprom.EReverenceType)ee[2, EepromV1.EAxisOffsets8.EReverenceType];
						eeprom.RefMoveA = (Eeprom.EReverenceType)ee[3, EepromV1.EAxisOffsets8.EReverenceType];

						eeprom.RefSeqence0 = (Eeprom.EReverenceSequence)ee[0, EepromV1.EAxisOffsets8.EReverenceSeqence];
						eeprom.RefSeqence1 = (Eeprom.EReverenceSequence)ee[1, EepromV1.EAxisOffsets8.EReverenceSeqence];
						eeprom.RefSeqence2 = (Eeprom.EReverenceSequence)ee[2, EepromV1.EAxisOffsets8.EReverenceSeqence];
						eeprom.RefSeqence3 = (Eeprom.EReverenceSequence)ee[3, EepromV1.EAxisOffsets8.EReverenceSeqence];
						eeprom.RefMoveSteprate = ee[EepromV1.EValueOffsets32.RefMoveStepRate];
						eeprom.MoveAwayFromRefernece = ee[EepromV1.EValueOffsets32.MoveAwayFromRefernece];

						eeprom.MaxStepRate	= ee[EepromV1.EValueOffsets32.MaxstepRate];
						eeprom.Acc			= ee[EepromV1.EValueOffsets32.Acc];
						eeprom.Dec			= ee[EepromV1.EValueOffsets32.Dec];
						eeprom.StepsPerMm1000 = BitConverter.ToSingle(BitConverter.GetBytes(ee[EepromV1.EValueOffsets32.StepsPerMm1000]), 0);

						_validReadEeprom = true;
					}

					EepromValue = eeprom;
				}
			});
		}

		public void EraseEeprom()
		{
			if (MessageBox?.Invoke("Send 'Erase EEprom command' to machine?", "CNCLib", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
			{
				Task.Run(async () =>
				{
					await Com.SendCommandAsync(@"$!");
					await Com.SendCommandAsync(@"$0=0");
				});

				CloseAction();
			}
        }

		public bool CanReadEeprom()
		{
			return Com.IsConnected;
		}

		public bool CanWriteEeprom()
		{
			return Com.IsConnected && _validReadEeprom;
		}
		public bool CanEraseEeprom()
		{
			return Com.IsConnected;
		}

		#endregion

		#region Commands

		public ICommand ReadEepromCommand => new DelegateCommand(ReadEeprom, CanReadEeprom);
        public ICommand WriteEepromCommand => new DelegateCommand(WriteEeprom, CanWriteEeprom);
		public ICommand EraseEepromCommand => new DelegateCommand(EraseEeprom, CanEraseEeprom);

		#endregion
	}
}
