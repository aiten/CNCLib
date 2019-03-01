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

using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using FluentAssertions;

using Framework.Arduino.SerialCommunication;
using Framework.Arduino.SerialCommunication.Abstraction;
using Framework.Dependency;
using Framework.Logging;
using Framework.Logging.Abstraction;

using NSubstitute;

using Xunit;

namespace Framework.UnitTest.SerialCommunication
{
    public class SerialTest : UnitTestBase
    {
        int  _resultIdx;
        bool _sendReply;

        public SerialTest()
        {
            InitializeDependencies();
        }

        private ISerialPort CreateSerialPortMock(string[] responseStrings)
        {
            var serialPort = Substitute.For<ISerialPort>();
            var baseStream = Substitute.For<MemoryStream>();

            serialPort.BaseStream.ReturnsForAnyArgs(baseStream);

            Encoding encoding = Encoding.GetEncoding(1200);
            serialPort.Encoding.ReturnsForAnyArgs(encoding);

            Framework.Dependency.Dependency.Container.ResetContainer();
            Framework.Dependency.Dependency.Container.RegisterInstance(serialPort);

            _resultIdx = 0;
            _sendReply = false;

            baseStream.WriteAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.Threading.CancellationToken>()).ReturnsForAnyArgs(
                async x =>
                {
                    _sendReply = true;
                    var cancellationToken = (CancellationToken)x[3];
                    await Task.Delay(0, cancellationToken);
                });

            baseStream.ReadAsync(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<System.Threading.CancellationToken>()).ReturnsForAnyArgs(
                async x =>
                {
                    var cancellationToken = (CancellationToken)x[3];
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        if (_sendReply)
                        {
                            _sendReply = false;
                            byte[] encodedStr = encoding.GetBytes(responseStrings[_resultIdx++]);
                            for (int i = 0; i < encodedStr.Length; i++)
                            {
                                ((byte[])x[0])[i] = encodedStr[i];
                            }

                            return encodedStr.Length;
                        }

                        await Task.Delay(1, cancellationToken);
                    }

                    return 0;
                });

            return serialPort;
        }

        private ILogger<Serial> CreateLogger()
        {
            return new Logger<Serial>();
        }

        [Fact]
        public async Task ConnectSerialTest()
        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(new string[0]))
            {
                await serial.ConnectAsync("com2");
                serial.CommandsInQueue.Should().Be(0);

                await serial.DisconnectAsync();
            }
        }

        [Fact]
        public async Task WriteOneCommandSerialTest()
        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(
                new[]
                {
                    serial.OkTag + "\n\r"
                }))
            {
                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?", 1000);

                await serial.DisconnectAsync();

                await serialPort.BaseStream.Received(1).WriteAsync(Arg.Is<byte[]>(e => (char)e[0] == '?'), 0, 2, Arg.Any<System.Threading.CancellationToken>());
                await Task.FromResult(0);
            }
        }

        [Fact]
        public async Task WriteTwoCommandSerialTest()
        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(
                new[]
                {
                    serial.OkTag + "\n\r", serial.OkTag + "\n\r"
                }))
            {
                await serial.ConnectAsync("com2");

                await serial.SendCommandAsync("?");
                await serial.SendCommandAsync("?");

                await serial.DisconnectAsync();
                await serialPort.BaseStream.Received(2).WriteAsync(Arg.Is<byte[]>(e => (char)e[0] == '?'), 0, 2, Arg.Any<System.Threading.CancellationToken>());
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
            var eventCounts = new EventCalls();
            serial.WaitForSend         += (sender, e) => eventCounts.EventWaitForSend++;
            serial.CommandSending      += (sender, e) => eventCounts.EventCommandSending++;
            serial.CommandSent         += (sender, e) => eventCounts.EventCommandSent++;
            serial.WaitCommandSent     += (sender, e) => eventCounts.EventWaitCommandSent++;
            serial.ReplyReceived       += (sender, e) => eventCounts.EventReplyReceived++;
            serial.ReplyOK             += (sender, e) => eventCounts.EventReplyOK++;
            serial.ReplyError          += (sender, e) => eventCounts.EventReplyError++;
            serial.ReplyInfo           += (sender, e) => eventCounts.EventReplyInfo++;
            serial.ReplyUnknown        += (sender, e) => eventCounts.EventReplyUnknown++;
            serial.CommandQueueChanged += (sender, e) => eventCounts.EventCommandQueueChanged++;
            serial.CommandQueueEmpty   += (sender, e) => eventCounts.EventCommandQueueEmpty++;
            return eventCounts;
        }

        [Fact]
        public async Task OkEventSerialTest()

        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(
                new[]
                {
                    serial.OkTag + "\n\r"
                }))
            {
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

        [Fact]
        public async Task InfoEventSerialTest()
        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(
                new[]
                {
                    serial.InfoTag + "\n\r" + serial.OkTag + "\n\r"
                }))
            {
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

        [Fact]
        public async Task ErrorEventWithOkSerialTest()
        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(
                new[]
                {
                    serial.ErrorTag + "\n\r" + serial.OkTag + "\n\r"
                }))
            {
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

        [Fact]
        public async Task ErrorEventWithOutOkSerialTest()
        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(
                new[]
                {
                    serial.ErrorTag + "\n\r"
                }))
            {
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

        [Fact]
        public async Task UnknownEventSerialTest()
        {
            using (var serial = new Serial(CreateLogger()))
            using (var serialPort = CreateSerialPortMock(
                new[]
                {
                    "Hallo\n\r" + serial.OkTag + "\n\r"
                }))
            {
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