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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using FluentAssertions;
using Framework.Arduino.SerialCommunication;
using System.IO;
using System.Threading.Tasks;

namespace Framework.Test
{
    [TestClass]
    public class SerialTest : UnitTestBase
    {
        private TInterface CreateMock<TInterface>() where TInterface : class, IDisposable
        {
            TInterface rep = Substitute.For<TInterface>();
            Framework.Tools.Dependency.Dependency.Container.RegisterInstance(rep);
            return rep;
        }

        [TestMethod]
        public void ConnectSerialTest()
        {
            var serialport = Substitute.For<Arduino.SerialCommunication.ISerialPort>();
            var basestream = Substitute.For<MemoryStream>();
            serialport.BaseStream.ReturnsForAnyArgs(basestream);
            var encoding = System.Text.Encoding.GetEncoding(12000);
            serialport.Encoding.ReturnsForAnyArgs(encoding);

            Framework.Tools.Dependency.Dependency.Container.RegisterInstance(serialport);

            var serial = new Serial();
            serial.Connect("com2");
            serial.CommandsInQueue.Should().Be(0);
            serial.Disconnect();
        }

        [TestMethod]
        public async Task WriteCommandSerialTest()
        {
            var serialport = Substitute.For<Arduino.SerialCommunication.ISerialPort>();
            var basestream = Substitute.For<MemoryStream>();
            serialport.BaseStream.ReturnsForAnyArgs(basestream);
            var encoding = System.Text.Encoding.GetEncoding(12000);
            serialport.Encoding.ReturnsForAnyArgs(encoding);

            Framework.Tools.Dependency.Dependency.Container.RegisterInstance(serialport);

            var serial = new Serial();
            serial.Connect("com2");

            string[] results = new string[]
            {
                "ok\n\r",
                "ok\n\r"
            };
            int resultidx = 0;

            basestream.ReadAsync(Arg.Any<Byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.Threading.CancellationToken>()).
                ReturnsForAnyArgs(async (x) =>
                {
                    if (serial.CommandsInQueue == 1)
                    {
                        byte[] encodedStr = encoding.GetBytes(results[resultidx++]);
                        for (int i = 0; i < encodedStr.Length; i++)
                        {
                            ((Byte[])x[0])[i] = encodedStr[i];
                        }
                        return encodedStr.Length;
                    }
                    return 0;
                });

            await serial.SendCommandAsync("?");
            await serial.SendCommandAsync("?");

            serial.Disconnect();

            basestream.Received().WriteAsync(Arg.Is<Byte[]>(e => (char) e[0] == '?'), 0,  4, Arg.Any<System.Threading.CancellationToken>());
        }
    }
}