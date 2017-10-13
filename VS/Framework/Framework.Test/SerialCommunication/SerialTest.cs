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

        private ISerialPort CreateSerialPortMock(ISerial serial, string[] responsstrings)
        {
            var serialport = Substitute.For<Arduino.SerialCommunication.ISerialPort>();
            var basestream = Substitute.For<MemoryStream>();
            serialport.BaseStream.ReturnsForAnyArgs(basestream);
            var encoding = System.Text.Encoding.GetEncoding(1200);
            serialport.Encoding.ReturnsForAnyArgs(encoding);

            Framework.Tools.Dependency.Dependency.Container.RegisterInstance(serialport);

            int resultidx = 0;

            basestream.ReadAsync(Arg.Any<Byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.Threading.CancellationToken>()).
                ReturnsForAnyArgs(async (x) =>
                {
                    if (serial.CommandsInQueue >= 1)
                    {

                        byte[] encodedStr = encoding.GetBytes(responsstrings[resultidx++]);
                        for (int i = 0; i < encodedStr.Length; i++)
                        {
                            ((Byte[])x[0])[i] = encodedStr[i];
                        }
                        return encodedStr.Length;
/*
                        byte[] encodedStr = encoding.GetBytes(responsstrings[resultidx++]);
                        encodedStr.CopyTo(((Byte[])x[0]), encodedStr.Length);
                        return encodedStr.Length;
*/
                    }
                    return 0;
                });

            return serialport;
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
        public async Task WriteOneCommandSerialTest()
        {
            var serial = new Serial();
            var serialport = CreateSerialPortMock(serial, new string[]
                                    {
                                        serial.OkTag + "\n\r"
                                    });

            serial.Connect("com2");

            await serial.SendCommandAsync("?");

            serial.Disconnect();

            serialport.BaseStream.Received(2).WriteAsync(Arg.Is<Byte[]>(e => (char)e[0] == '?'), 0, 2, Arg.Any<System.Threading.CancellationToken>());
        }

        [TestMethod]
        public async Task WriteTwoCommandSerialTest()
        {
            var serial = new Serial();
            var serialport = CreateSerialPortMock(serial, new string[]
                                    {
                                        serial.OkTag + "\n\r",
                                        serial.OkTag + "\n\r"
                                    });

            serial.Connect("com2");

            await serial.SendCommandAsync("?");
            await serial.SendCommandAsync("?");

            serial.Disconnect();

            serialport.BaseStream.Received(2).WriteAsync(Arg.Is<Byte[]>(e => (char) e[0] == '?'), 0,  2, Arg.Any<System.Threading.CancellationToken>());
        }

        #region events

        class EventCalls
        {
            public int eventWaitForSend = 0;
            public int eventCommandSending = 0;
            public int eventCommandSent = 0;
            public int eventWaitCommandSent = 0;
            public int eventReplyReceived = 0;
            public int eventReplyOK = 0;
            public int eventReplyError = 0;
            public int eventReplyInfo = 0;
            public int eventReplyUnknown = 0;
            public int eventCommandQueueChanged = 0;
            public int eventCommandQueueEmpty = 0;
        }

        private EventCalls SubscribeForEventCall(ISerial serial)
        {
            EventCalls eventcounts = new EventCalls();
            serial.WaitForSend += (object sender, SerialEventArgs e) => eventcounts.eventWaitForSend++;
            serial.CommandSending += (object sender, SerialEventArgs e) => eventcounts.eventCommandSending++;
            serial.CommandSent += (object sender, SerialEventArgs e) => eventcounts.eventCommandSent++;
            serial.WaitCommandSent += (object sender, SerialEventArgs e) => eventcounts.eventWaitCommandSent++;
            serial.ReplyReceived += (object sender, SerialEventArgs e) => eventcounts.eventReplyReceived++;
            serial.ReplyOK += (object sender, SerialEventArgs e) => eventcounts.eventReplyOK++;
            serial.ReplyError += (object sender, SerialEventArgs e) => eventcounts.eventReplyError++;
            serial.ReplyInfo += (object sender, SerialEventArgs e) => eventcounts.eventReplyInfo++;
            serial.ReplyUnknown += (object sender, SerialEventArgs e) => eventcounts.eventReplyUnknown++;
            serial.CommandQueueChanged += (object sender, SerialEventArgs e) => eventcounts.eventCommandQueueChanged++;
            serial.CommandQueueEmpty += (object sender, SerialEventArgs e) => eventcounts.eventCommandQueueEmpty++;
            return eventcounts;
        }

        [TestMethod]
        public async Task OkEventSerialTest()

        {
            var serial = new Serial();
            var serialport = CreateSerialPortMock(serial, new string[]
                                    {
                                        serial.OkTag + "\n\r"
                                    });

            var eventCalls = SubscribeForEventCall(serial);

            serial.Connect("com2");

            await serial.SendCommandAsync("?");

            serial.Disconnect();

            eventCalls.eventWaitForSend.Should().Be(0);
            eventCalls.eventCommandSending.Should().Be(1);
            eventCalls.eventCommandSent.Should().Be(1);
            eventCalls.eventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
            eventCalls.eventReplyReceived.Should().Be(1);
            eventCalls.eventReplyOK.Should().Be(1);
            eventCalls.eventReplyError.Should().Be(0);
            eventCalls.eventReplyInfo.Should().Be(0);
            eventCalls.eventReplyUnknown.Should().Be(0);
            eventCalls.eventCommandQueueChanged.Should().Be(2);
            eventCalls.eventCommandQueueEmpty.Should().Be(2);
        }

        [TestMethod]
        public async Task InfoEventSerialTest()
        {
            var serial = new Serial();
            var serialport = CreateSerialPortMock(serial, new string[]
                                    {
                                        serial.InfoTag + "\n\r",
                                        serial.OkTag + "\n\r"
                                    });

            var eventCalls = SubscribeForEventCall(serial);

            serial.Connect("com2");

            await serial.SendCommandAsync("?");

            serial.Disconnect();

            eventCalls.eventWaitForSend.Should().Be(0);
            eventCalls.eventCommandSending.Should().Be(1);
            eventCalls.eventCommandSent.Should().Be(1);
            eventCalls.eventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
            eventCalls.eventReplyReceived.Should().Be(2);
            eventCalls.eventReplyOK.Should().Be(1);
            eventCalls.eventReplyError.Should().Be(0);
            eventCalls.eventReplyInfo.Should().Be(1);
            eventCalls.eventReplyUnknown.Should().Be(0);
            eventCalls.eventCommandQueueChanged.Should().Be(2);
            eventCalls.eventCommandQueueEmpty.Should().Be(2);
        }

        [TestMethod]
        public async Task ErrorEventWithOkSerialTest()
        {
            var serial = new Serial();
            var serialport = CreateSerialPortMock(serial, new string[]
                                    {
                                        serial.ErrorTag + "\n\r",
                                        serial.OkTag + "\n\r"
                                    });

            var eventCalls = SubscribeForEventCall(serial);

            serial.ErrorIsReply = false;

            serial.Connect("com2");

            await serial.SendCommandAsync("?");

            serial.Disconnect();

            eventCalls.eventWaitForSend.Should().Be(0);
            eventCalls.eventCommandSending.Should().Be(1);
            eventCalls.eventCommandSent.Should().Be(1);
            eventCalls.eventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
            eventCalls.eventReplyReceived.Should().Be(2);
            eventCalls.eventReplyOK.Should().Be(1);
            eventCalls.eventReplyError.Should().Be(1);
            eventCalls.eventReplyInfo.Should().Be(0);
            eventCalls.eventReplyUnknown.Should().Be(0);
            eventCalls.eventCommandQueueChanged.Should().Be(2);
            eventCalls.eventCommandQueueEmpty.Should().Be(2);
        }

        [TestMethod]
        public async Task ErrorEventWithOutOkSerialTest()
        {
            var serial = new Serial();
            var serialport = CreateSerialPortMock(serial, new string[]
                                    {
                                        serial.ErrorTag + "\n\r"
                                    });

            var eventCalls = SubscribeForEventCall(serial);

            serial.ErrorIsReply = true;     // no OK is needed

            serial.Connect("com2");

            await serial.SendCommandAsync("?");

            serial.Disconnect();

            eventCalls.eventWaitForSend.Should().Be(0);
            eventCalls.eventCommandSending.Should().Be(1);
            eventCalls.eventCommandSent.Should().Be(1);
            eventCalls.eventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
            eventCalls.eventReplyReceived.Should().Be(1);
            eventCalls.eventReplyOK.Should().Be(0);
            eventCalls.eventReplyError.Should().Be(1);
            eventCalls.eventReplyInfo.Should().Be(0);
            eventCalls.eventReplyUnknown.Should().Be(0);
            eventCalls.eventCommandQueueChanged.Should().Be(2);
            eventCalls.eventCommandQueueEmpty.Should().Be(2);
        }

        [TestMethod]
        public async Task UnknownEventSerialTest()
        {
            var serial = new Serial();
            var serialport = CreateSerialPortMock(serial, new string[]
                                    {
                                        "Hallo\n\r",
                                        serial.OkTag + "\n\r"
                                    });

            var eventCalls = SubscribeForEventCall(serial);

            serial.ErrorIsReply = true;     // no OK is needed

            serial.Connect("com2");

            await serial.SendCommandAsync("?");

            serial.Disconnect();

            eventCalls.eventWaitForSend.Should().Be(0);
            eventCalls.eventCommandSending.Should().Be(1);
            eventCalls.eventCommandSent.Should().Be(1);
            eventCalls.eventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
            eventCalls.eventReplyReceived.Should().Be(2);
            eventCalls.eventReplyOK.Should().Be(1);
            eventCalls.eventReplyError.Should().Be(0);
            eventCalls.eventReplyInfo.Should().Be(0);
            eventCalls.eventReplyUnknown.Should().Be(1);
            eventCalls.eventCommandQueueChanged.Should().Be(2);
            eventCalls.eventCommandQueueEmpty.Should().Be(2);
        }

        #endregion
    }
}