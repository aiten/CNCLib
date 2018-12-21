////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

namespace Framework.Arduino.SerialCommunication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Threading.Tasks;

    using Abstraction;

    public static class SerialExtension
    {
        const int DefaultTimeout = 10 * 60 * 1000;

        /// <summary>
        /// Send command and wait until the command is transferred and we got a reply (no command pending)
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="line">command line to send</param>
        public static IEnumerable<SerialCommand> SendCommand(this ISerial serial, string line)
        {
            return serial.SendCommandsAsync(new[] { line }, DefaultTimeout).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send command and wait until the command is transferred and we got a reply (no command pending)
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="line">command line to send</param>
        /// <param name="waitForMilliseconds"></param>
        public static async Task<IEnumerable<SerialCommand>> SendCommandAsync(this ISerial serial, string line, int waitForMilliseconds = DefaultTimeout)
        {
            return await serial.SendCommandsAsync(new[] { line }, waitForMilliseconds);
        }

        /// <summary>
        /// Send multiple command lines to the arduino. Wait until the commands are transferred and we got a reply (no command pending)
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="commands"></param>
        public static IEnumerable<SerialCommand> SendCommands(this ISerial serial, IEnumerable<string> commands)
        {
            return serial.SendCommandsAsync(commands, DefaultTimeout).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Queue command - do not wait - not for transfer and not for replay
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="line">command line to send</param>
        public static IEnumerable<SerialCommand> QueueCommand(this ISerial serial, string line)
        {
            return serial.QueueCommandsAsync(new[] { line }).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send commands stored in a file. Wait until the commands are transferred and we got a reply (no command pending)
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="filename">used for a StreamReader</param>
        /// <param name="waitForMilliseconds"></param>
        public static async Task<IEnumerable<SerialCommand>> SendFileAsync(this ISerial serial, string filename, int waitForMilliseconds = DefaultTimeout)
        {
            var list = await serial.QueueFileAsync(filename);
            await serial.WaitUntilQueueEmptyAsync(waitForMilliseconds);
            return list;
        }

        /// <summary>
        /// Send commands stored in a file. Wait until the commands are transferred and we got a reply (no command pending)
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="filename">used for a StreamReader</param>
        public static async Task<IEnumerable<SerialCommand>> QueueFileAsync(this ISerial serial, string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                string line;
                var    lines = new List<string>();
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                return await serial.QueueCommandsAsync(lines.ToArray());
            }
        }

        /// <summary>
        /// Send a command to the arduino and wait until a (OK) reply
        /// Queue must be empty
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="line">command line</param>
        /// <param name="waitForMilliseconds"></param>
        /// <returns>ok result from arduino or empty(if error)</returns>
        public static async Task<string> SendCommandAndReadOKReplyAsync(this ISerial serial, string line, int waitForMilliseconds)
        {
            var ret = await serial.SendCommandAsync(line, waitForMilliseconds);
            if (ret.Any())
            {
                var last = ret.Last();
                if (last.ReplyType.HasFlag(EReplyType.ReplyOK))
                {
                    return last.ResultText;
                }
            }

            return null;
        }

        /// <summary>
        /// write all pending (command with no reply) to file
        /// Intended to be used if user abort queue because of an error
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="filename"></param>
        public static void WritePendingCommandsToFile(this ISerial serial, string filename)
        {
            using (var sw = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
            {
                foreach (SerialCommand cmd in serial.PendingCommands)
                {
                    sw.WriteLine(cmd.CommandText);
                }
            }
        }

        public static void WriteCommandHistory(this ISerial serial, string filename)
        {
            using (var sr = new StreamWriter(Environment.ExpandEnvironmentVariables(filename)))
            {
                foreach (SerialCommand cmds in serial.CommandHistoryCopy)
                {
                    sr.Write(cmds.SentTime);
                    sr.Write(":");
                    sr.Write(cmds.CommandText);
                    sr.Write(" => ");
                    sr.WriteLine(cmds.ResultText);
                }
            }
        }
    }
}