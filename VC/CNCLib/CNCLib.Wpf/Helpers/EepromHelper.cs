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

using CNCLib.GCode;
using CNCLib.Wpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNCLib.Wpf.Helpers
{
	class EepromHelper
	{
		public async Task<Eeprom> ReadEepromAsync()
		{
			UInt32[] values = await new MachineGCodeHelper().GetEpromValuesAsync();
			if (values != null)
			{
				var ee = new EepromV1() { Values = values };

				if (ee.IsValid)
				{
					File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromRead.nc"), ee.ToGCode());
					var numaxis = ee[EepromV1.EValueOffsets8.NumAxis];

					var eeprom = Eeprom.Create(numaxis);
					eeprom.Values = values;

					eeprom.NumAxis = ee[EepromV1.EValueOffsets8.NumAxis];
					eeprom.UseAxis = ee[EepromV1.EValueOffsets8.UseAxis];

					eeprom.Info1 = ee[EepromV1.EValueOffsets32.Info1];
					eeprom.Info2 = ee[EepromV1.EValueOffsets32.Info2];

					for (int i = 0; i < numaxis; i++)
					{
						eeprom.GetAxis(i).Size = ee[i, EepromV1.EAxisOffsets32.Size];
						eeprom.GetAxis(i).RefMove = (Eeprom.EReverenceType)ee[i, EepromV1.EAxisOffsets8.EReverenceType];
						eeprom.GetAxis(i).RefHitValue = ee[i, EepromV1.EAxisOffsets8.EReverenceHitValue];

						eeprom.GetAxis(i).StepperDirection = (ee[EepromV1.EValueOffsets8.StepperDirection] & (1 << i)) != 0;

						eeprom[i] = (Eeprom.EReverenceSequence)ee[i, EepromV1.EAxisOffsets8.EReverenceSeqence];
					}

					eeprom.MaxSpindleSpeed = ee[EepromV1.EValueOffsets16.MaxSpindleSpeed];

					eeprom.RefMoveSteprate = ee[EepromV1.EValueOffsets32.RefMoveStepRate];
					eeprom.MoveAwayFromRefernece = ee[EepromV1.EValueOffsets32.MoveAwayFromRefernece];

					eeprom.MaxStepRate = ee[EepromV1.EValueOffsets32.MaxStepRate];
					eeprom.Acc = ee[EepromV1.EValueOffsets16.Acc];
					eeprom.Dec = ee[EepromV1.EValueOffsets16.Dec];
					eeprom.StepsPerMm1000 = BitConverter.ToSingle(BitConverter.GetBytes(ee[EepromV1.EValueOffsets32.StepsPerMm1000]), 0);

					return eeprom;
				}
			}
			return null;
		}

		public async Task<bool> WriteEepromAsync(Eeprom EepromValue)
		{
			var ee = new EepromV1() { Values = EepromValue.Values };

			if (ee.IsValid)
			{
				var numaxis = ee[EepromV1.EValueOffsets8.NumAxis];

				for (int i = 0; i < numaxis; i++)
				{
					ee[i, EepromV1.EAxisOffsets32.Size] = EepromValue.GetAxis(i).Size;
					ee[i, EepromV1.EAxisOffsets8.EReverenceType] = (byte)EepromValue.GetAxis(i).RefMove;
					ee[i, EepromV1.EAxisOffsets8.EReverenceSeqence] = (byte)(Eeprom.EReverenceSequence)EepromValue[i];
					ee[i, EepromV1.EAxisOffsets8.EReverenceHitValue] = EepromValue.GetAxis(i).RefHitValue;

					var direction = ee[EepromV1.EValueOffsets8.StepperDirection] & (~(1 << i));
					if (EepromValue.GetAxis(i).StepperDirection)
					{
						direction += 1 << i;
					}
					ee[EepromV1.EValueOffsets8.StepperDirection] = (byte)direction;
				}

				ee[EepromV1.EValueOffsets16.MaxSpindleSpeed] = EepromValue.MaxSpindleSpeed;
				ee[EepromV1.EValueOffsets32.RefMoveStepRate] = EepromValue.RefMoveSteprate;
				ee[EepromV1.EValueOffsets32.MoveAwayFromRefernece] = EepromValue.MoveAwayFromRefernece;

				ee[EepromV1.EValueOffsets32.MaxStepRate] = EepromValue.MaxStepRate;
				ee[EepromV1.EValueOffsets16.Acc] = EepromValue.Acc;
				ee[EepromV1.EValueOffsets16.Dec] = EepromValue.Dec;
				ee[EepromV1.EValueOffsets32.StepsPerMm1000] = BitConverter.ToUInt32(BitConverter.GetBytes(EepromValue.StepsPerMm1000), 0);

				File.WriteAllLines(Environment.ExpandEnvironmentVariables(@"%TEMP%\EepromWrite.nc"), ee.ToGCode());

				await new MachineGCodeHelper().WriteEepromValuesAsync(ee);
				return true;
			}
			return false;
		}

		public async Task<bool> EraseEepromAsync()
		{
			await new MachineGCodeHelper().EraseEepromValuesAsync();
			return true;
		}
	}
}
