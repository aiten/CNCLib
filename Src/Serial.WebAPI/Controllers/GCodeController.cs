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

namespace CNCLib.Serial.WebAPI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CNCLib.GCode.Machine;
    using CNCLib.GCode.Serial;
    using CNCLib.Serial.WebAPI.SerialPort;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize]
    [Route("api/[controller]")]
    public class GCodeController : Controller
    {
        #region SpecialCommand

        /// <summary>
        /// Get a machine parameter value (#xxxx).
        /// </summary>
        /// <param name="id">Id of the serial port.</param>
        /// <param name="paramNo">Number of the parameter to be read.</param>
        /// <returns></returns>
        [HttpPost("{id:int}/getParameter")]
        public async Task<ActionResult<decimal>> GetParameter(int id, int paramNo)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            var paramValue = await port.Serial.GetParameterValueAsync(paramNo, port.GCodeCommandPrefix);

            return Ok(paramValue);
        }

        /// <summary>
        /// Get the position of all axis.
        /// </summary>
        /// <param name="id">Id of the serial port.</param>
        /// <returns></returns>
        [HttpPost("{id:int}/getPosition")]
        public async Task<ActionResult<IEnumerable<IEnumerable<decimal>>>> GetPosition(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            var position = await port.Serial.GetPosition(port.GCodeCommandPrefix);

            return Ok(position);
        }

        /// <summary>
        /// Send a probe command to the machine.
        /// </summary>
        /// <param name="id">Id of the serial port.</param>
        /// <param name="axisName">Name of the axis, e.g. 'X'</param>
        /// <param name="probeSize"></param>
        /// <param name="probeDist"></param>
        /// <param name="probeDistUp"></param>
        /// <param name="probeFeed"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/probe")]
        public async Task<ActionResult> Probe(int id, string axisName, decimal probeSize, decimal probeDist, decimal probeDistUp, decimal probeFeed)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(axisName) || probeSize == 0 || probeDist == 0 || probeFeed == 0)
            {
                return new BadRequestResult();
            }

            await port.Serial.SendProbeCommandAsync(axisName, probeSize, probeDist, probeDistUp, probeFeed);

            return Ok();
        }

        #endregion

        #region Eeprom

        /// <summary>
        /// Read the content of the eeprom from the machine.
        /// </summary>
        /// <param name="id">Id of the serial port.</param>
        /// <returns>Array of eeprom values.</returns>
        [HttpPost("{id:int}/eeprom")]
        public async Task<ActionResult<IEnumerable<UInt32>>> GetEeprom(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            var eeprom = await port.Serial.GetEpromValues(GCodeSerial.DefaultEpromTimeout);

            return Ok(eeprom);
        }

        /// <summary>
        /// Write the machine eeprom.
        /// </summary>
        /// <param name="id">Id of the serial port.</param>
        /// <param name="eepromValue">Array of all eeprom values (32bit)</param>
        /// <returns></returns>
        [HttpPut("{id:int}/eeprom")]
        public async Task<ActionResult> SaveEeprom(int id, [FromBody] UInt32[] eepromValue)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            var ee = new EepromV1 { Values = eepromValue };

            if (ee.IsValid)
            {
                await port.Serial.WriteEepromValues(ee);
            }

            return Ok();
        }

        /// <summary>
        /// Erase Eprom values.
        /// Please restart machine after this call.
        /// </summary>
        /// <param name="id">Id of the serial port.</param>
        /// <returns></returns>
        [HttpDelete("{id:int}/eeprom")]
        public async Task<ActionResult<IEnumerable<UInt32>>> EraseEeprom(int id)
        {
            var port = await SerialPortList.GetPortAndRescan(id);

            if (port == null)
            {
                return NotFound();
            }

            var eeprom = await port.Serial.EraseEeprom();

            return Ok(eeprom);
        }

        #endregion

        #region eeprom info

        /// <summary>
        /// Convert the eeprom to a readable class.
        /// </summary>
        /// <param name="eepromValue">eeprom values from machine.</param>
        /// <returns>Eeprom as object.</returns>
        [HttpPost("toEeprom")]
        public ActionResult<Eeprom> GetEepromInfo([FromBody] UInt32[] eepromValue)
        {
            var eeprom = EepromExtensions.ConvertEeprom(eepromValue);
            return Ok(eeprom);
        }

        /// <summary>
        /// Convert the eeprom-object to uint32.
        /// </summary>
        /// <param name="eeprom">eeprom values from machine.</param>
        /// <returns>uint32 to be sent to the machine.</returns>
        [HttpPost("fromEeprom")]
        public ActionResult<UInt32[]> GetEepromInfo([FromBody] Eeprom eeprom)
        {
            var eePromV1 = new EepromV1() { Values = eeprom.Values };
            eeprom.WriteTo(eePromV1);
            return Ok(eeprom.Values);
        }

        #endregion
    }
}