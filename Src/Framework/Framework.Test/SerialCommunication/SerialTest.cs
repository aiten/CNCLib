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

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Framework.Arduino.SerialCommunication;
using Framework.Tools.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Framework.Test.SerialCommunication
{
    [TestClass]
    public class SerialTest : UnitTestBase
    {
        private ISerialPort CreateSerialPortMock(ISerial serial, string[] responsstrings)
        {
            var serialport = Substitute.For<ISerialPort>();
            var basestream = Substitute.For<MemoryStream>();
            serialport.BaseStream.ReturnsForAnyArgs(basestream);
            Encoding encoding = Encoding.GetEncoding(1200);
            serialport.Encoding.ReturnsForAnyArgs(encoding);

            Tools.Dependency.Dependency.Container.ResetContainer();
            Tools.Dependency.Dependency.Container.RegisterInstance(serialport);

            int  resultidx = 0;
            bool sendReply = false;

            basestream.WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.Threading.CancellationToken>()).
                ReturnsForAnyArgs(async x =>
                {
                    sendReply = true;
                    await Task.FromResult(0);
                });


            basestream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.Threading.CancellationToken>()).
                ReturnsForAnyArgs(async x =>
                {
                    if (sendReply)
                    {
                        await Task.Delay(10);
                        sendReply = false;
                        byte[] encodedStr = encoding.GetBytes(responsstrings[resultidx++]);
                        for (int i = 0; i < encodedStr.Length; i++)
                        {
                            ((byte[]) x[0])[i] = encodedStr[i];
                        }

                        return encodedStr.Length;
                    }

                    await Task.Delay(10);
                    return 0;
                });

            return serialport;
        }

        [TestMethod]
        public async Task ConnectSerialTest()
        {
            var serialport = Substitute.For<ISerialPort>();
            var basestream = Substitute.For<MemoryStream>();
            serialport.BaseStream.ReturnsForAnyArgs(basestream);
            Encoding encoding = Encoding.GetEncoding(12000);
            serialport.Encoding.ReturnsForAnyArgs(encoding);

            Tools.Dependency.Dependency.Container.ResetContainer();
            Tools.Dependency.Dependency.Container.RegisterInstance(serialport);

            using (var serial = new Serial())
            {
                await serial.ConnectAsync("com2");
                serial.CommandsInQueue.Should().Be(0);

                await serial.DisconnectAsync();
            }
        }

        [TestMethod]
        public async Task WriteOneCommandSerialTest()
        {
            using (var serial = new Serial())
            {
                var serialport = CreateSerialPortMock(serial, new[]
                {
                    serial.OkTag + "\n\r"
                });

                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?", 1000);

                await serial.DisconnectAsync();

                await serialport.BaseStream.Received(1).WriteAsync(Arg.Is<byte[]>(e => (char) e[0] == '?'), 0, 2, Arg.Any<System.Threading.CancellationToken>());
                await Task.FromResult(0);
            }
        }

        [TestMethod]
        public async Task WriteTwoCommandSerialTest()
        {
            using (var serial = new Serial())
            {
                var serialport = CreateSerialPortMock(serial, new[]
                {
                    serial.OkTag + "\n\r", serial.OkTag + "\n\r"
                });

                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?");
                await serial.SendCommandAsync("?");

                await serial.DisconnectAsync();
                await serialport.BaseStream.Received(2).WriteAsync(Arg.Is<byte[]>(e => (char) e[0] == '?'), 0, 2, Arg.Any<System.Threading.CancellationToken>());
                await Task.FromResult(0);
            }
        }

        #region events

        sealed class EventCalls
        {
            public int EventWaitForSend         { get; set; }
            public int EventCommandSending      { get; set; }
            public int EventCommandSent         { get; set; }
            public int EventWaitCommandSent     { get; set; }
            public int EventReplyReceived       { get; set; }
            public int EventReplyOK             { get; set; }
            public int EventReplyError          { get; set; }
            public int EventReplyInfo           { get; set; }
            public int EventReplyUnknown        { get; set; }
            public int EventCommandQueueChanged { get; set; }
            public int EventCommandQueueEmpty   { get; set; }
        }

        private EventCalls SubscribeForEventCall(ISerial serial)
        {
            var eventcounts = new EventCalls();
            serial.WaitForSend         += (sender, e) => eventcounts.EventWaitForSend++;
            serial.CommandSending      += (sender, e) => eventcounts.EventCommandSending++;
            serial.CommandSent         += (sender, e) => eventcounts.EventCommandSent++;
            serial.WaitCommandSent     += (sender, e) => eventcounts.EventWaitCommandSent++;
            serial.ReplyReceived       += (sender, e) => eventcounts.EventReplyReceived++;
            serial.ReplyOK             += (sender, e) => eventcounts.EventReplyOK++;
            serial.ReplyError          += (sender, e) => eventcounts.EventReplyError++;
            serial.ReplyInfo           += (sender, e) => eventcounts.EventReplyInfo++;
            serial.ReplyUnknown        += (sender, e) => eventcounts.EventReplyUnknown++;
            serial.CommandQueueChanged += (sender, e) => eventcounts.EventCommandQueueChanged++;
            serial.CommandQueueEmpty   += (sender, e) => eventcounts.EventCommandQueueEmpty++;
            return eventcounts;
        }

        [TestMethod]
        public async Task OkEventSerialTest()

        {
            using (var serial = new Serial())
            {
                /* var serialport = */
                CreateSerialPortMock(serial, new[]
                {
                    serial.OkTag + "\n\r"
                });

                var eventCalls = SubscribeForEventCall(serial);

                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?");

                await serial.DisconnectAsync();

                eventCalls.EventWaitForSend.Should().Be(0);
                eventCalls.EventCommandSending.Should().Be(1);
                eventCalls.EventCommandSent.Should().Be(1);
                eventCalls.EventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
                eventCalls.EventReplyReceived.Should().Be(1);
                eventCalls.EventReplyOK.Should().Be(1);
                eventCalls.EventReplyError.Should().Be(0);
                eventCalls.EventReplyInfo.Should().Be(0);
                eventCalls.EventReplyUnknown.Should().Be(0);
                eventCalls.EventCommandQueueChanged.Should().Be(2);
                eventCalls.EventCommandQueueEmpty.Should().Be(2);

                await Task.FromResult(0);
            }
        }

        [TestMethod]
        public async Task InfoEventSerialTest()
        {
            using (var serial = new Serial())
            {
                /* var serialport = */
                CreateSerialPortMock(serial, new[]
                {
                    serial.InfoTag + "\n\r" + serial.OkTag + "\n\r"
                });

                var eventCalls = SubscribeForEventCall(serial);

                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?");

                await serial.DisconnectAsync();

                eventCalls.EventWaitForSend.Should().Be(0);
                eventCalls.EventCommandSending.Should().Be(1);
                eventCalls.EventCommandSent.Should().Be(1);
                eventCalls.EventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
                eventCalls.EventReplyReceived.Should().Be(2);
                eventCalls.EventReplyOK.Should().Be(1);
                eventCalls.EventReplyError.Should().Be(0);
                eventCalls.EventReplyInfo.Should().Be(1);
                eventCalls.EventReplyUnknown.Should().Be(0);
                eventCalls.EventCommandQueueChanged.Should().Be(2);
                eventCalls.EventCommandQueueEmpty.Should().Be(2);

                await Task.FromResult(0);
            }
        }

        [TestMethod]
        public async Task ErrorEventWithOkSerialTest()
        {
            using (var serial = new Serial())
            {
                /* var serialport = */
                CreateSerialPortMock(serial, new[]
                {
                    serial.ErrorTag + "\n\r" + serial.OkTag + "\n\r"
                });

                var eventCalls = SubscribeForEventCall(serial);

                serial.ErrorIsReply = false;

                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?");

                await serial.DisconnectAsync();

                eventCalls.EventWaitForSend.Should().Be(0);
                eventCalls.EventCommandSending.Should().Be(1);
                eventCalls.EventCommandSent.Should().Be(1);
                eventCalls.EventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
                eventCalls.EventReplyReceived.Should().Be(2);
                eventCalls.EventReplyOK.Should().Be(1);
                eventCalls.EventReplyError.Should().Be(1);
                eventCalls.EventReplyInfo.Should().Be(0);
                eventCalls.EventReplyUnknown.Should().Be(0);
                eventCalls.EventCommandQueueChanged.Should().Be(2);
                eventCalls.EventCommandQueueEmpty.Should().Be(2);

                await Task.FromResult(0);
            }
        }

        [TestMethod]
        public async Task ErrorEventWithOutOkSerialTest()
        {
            using (var serial = new Serial())
            {
                /* var serialport = */
                CreateSerialPortMock(serial, new[]
                {
                    serial.ErrorTag + "\n\r"
                });

                var eventCalls = SubscribeForEventCall(serial);

                serial.ErrorIsReply = true; // no OK is needed

                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?");

                await serial.DisconnectAsync();

                eventCalls.EventWaitForSend.Should().Be(0);
                eventCalls.EventCommandSending.Should().Be(1);
                eventCalls.EventCommandSent.Should().Be(1);
                eventCalls.EventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
                eventCalls.EventReplyReceived.Should().Be(1);
                eventCalls.EventReplyOK.Should().Be(0);
                eventCalls.EventReplyError.Should().Be(1);
                eventCalls.EventReplyInfo.Should().Be(0);
                eventCalls.EventReplyUnknown.Should().Be(0);
                eventCalls.EventCommandQueueChanged.Should().Be(2);
                eventCalls.EventCommandQueueEmpty.Should().Be(2);
            }
        }

        [TestMethod]
        public async Task UnknownEventSerialTest()
        {
            using (var serial = new Serial())
            {
                /* var serialport = */
                CreateSerialPortMock(serial, new[]
                {
                    "Hallo\n\r" + serial.OkTag + "\n\r"
                });

                var eventCalls = SubscribeForEventCall(serial);

                serial.ErrorIsReply = true; // no OK is needed

                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?");

                await serial.DisconnectAsync();

                eventCalls.EventWaitForSend.Should().Be(0);
                eventCalls.EventCommandSending.Should().Be(1);
                eventCalls.EventCommandSent.Should().Be(1);
                eventCalls.EventWaitCommandSent.Should().BeGreaterOrEqualTo(1);
                eventCalls.EventReplyReceived.Should().Be(2);
                eventCalls.EventReplyOK.Should().Be(1);
                eventCalls.EventReplyError.Should().Be(0);
                eventCalls.EventReplyInfo.Should().Be(0);
                eventCalls.EventReplyUnknown.Should().Be(1);
                eventCalls.EventCommandQueueChanged.Should().Be(2);
                eventCalls.EventCommandQueueEmpty.Should().Be(2);
            }
        }

        #endregion
    }
}