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
            var memorystream = new MemoryStream();
            serialport.BaseStream.ReturnsForAnyArgs(memorystream);
            var encoding = System.Text.Encoding.GetEncoding(12000);
            serialport.Encoding.ReturnsForAnyArgs(encoding);

            Framework.Tools.Dependency.Dependency.Container.RegisterInstance(serialport);

            var serial = new Serial();
            serial.Connect("com2");
            serial.CommandsInQueue.Should().Be(0);
            serial.Disconnect();
        }

        [TestMethod]
        public void WriteCommandSerialTest()
        {
            var serialport = Substitute.For<Arduino.SerialCommunication.ISerialPort>();
            var memorystream = new MemoryStream();
            serialport.BaseStream.ReturnsForAnyArgs(memorystream);
            var encoding = System.Text.Encoding.GetEncoding(12000);
            serialport.Encoding.ReturnsForAnyArgs(encoding);

            Framework.Tools.Dependency.Dependency.Container.RegisterInstance(serialport);

            var serial = new Serial();
            serial.Connect("com2");
            serial.QueueCommand("?");
            serial.CommandsInQueue.Should().Be(1);
            serial.Disconnect();
        }

    }
}