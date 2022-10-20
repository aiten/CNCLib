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

namespace CNCLib.WpfClient.ViewModels.ManualControl;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using CNCLib.WpfClient.Helpers;

using Framework.Arduino.SerialCommunication;
using Framework.Wpf.Helpers;

public class SDViewModel : DetailViewModel
{
    private readonly Global _global;

    public SDViewModel(IManualControlViewModel vm, Global global) : base(vm, global)
    {
        _global = global;
    }

    #region Properties

    private string _fileName = @"%USERPROFILE%\Documents\test.GCode";

    public string FileName
    {
        get => _fileName;
        set => SetProperty(ref _fileName, value);
    }

    private string _SDFileName = @"auto0.g";

    public string SDFileName
    {
        get => _SDFileName;
        set => SetProperty(ref _SDFileName, value);
    }

    #endregion

    #region Commands / CanCommands

    public void SendM20File()
    {
        RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "m20"); });
    }

    public void SendM24File()
    {
        SendM24File(SDFileName);
    }

    public void SendM24File(string filename)
    {
        RunAndUpdate(
            () =>
            {
                _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "m23 " + filename);
                _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "m24");
            });
    }

    public void SendM28File()
    {
        SendM28File(FileName, SDFileName);
    }

    public void SendM28File(string filename, string sDFileName)
    {
        RunInNewTask(
            () =>
            {
                var lines = new List<string>();
                using (var sr = new StreamReader(filename))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lines.Add(_global.Machine.PrepareCommand(line));
                    }
                }

                SendM28(sDFileName, lines.ToArray()).ConfigureAwait(false).GetAwaiter().GetResult();
            });
    }

    public void SendM28PreView()
    {
        SendM28PreView(SDFileName);
    }

    public void SendM28PreView(string sDFileName)
    {
        RunInNewTask(
            () =>
            {
                var lines = _global.Commands.ToStringList();
                SendM28(sDFileName, lines.ToArray()).ConfigureAwait(false).GetAwaiter().GetResult();
            });
    }

    private async Task SendM28(string sDFileName, string[] lines)
    {
        const int SDTimeoutCreate = 10000;
        const int SDTimeoutCopy   = 10 * 60 * 1000;
        const int SDTimeoutSave   = 10000;
        var       result          = await _global.Com.Current.SendCommandAndReadOKReplyAsync(_global.Machine.PrepareCommand("m28 " + sDFileName), SDTimeoutCreate);
        if (!string.IsNullOrEmpty(result) && result.Contains(sDFileName))
        {
            await _global.Com.Current.SendCommandsAsync(lines, SDTimeoutCopy);
            var resultDone = await _global.Com.Current.SendCommandAndReadOKReplyAsync(_global.Machine.PrepareCommand("m29"), SDTimeoutSave);

            if (!string.IsNullOrEmpty(resultDone) && result.Contains("Done"))
            {
            }
        }
    }

    public void SendM30File()
    {
        SendM30File(SDFileName);
    }

    public void SendM30File(string filename)
    {
        RunAndUpdate(() => { _global.Com.Current.PrepareAndQueueCommand(_global.Machine, "m30 " + filename); });
    }

    public void SendFileDirect()
    {
        RunAndUpdate(async () => { await _global.Com.Current.SendFileAsync(FileName); });
    }

    public void AddToFile()
    {
        RunAndUpdate(
            async () =>
            {
                string message = await _global.Com.Current.SendCommandAndReadOKReplyAsync(_global.Machine.PrepareCommand("m114"), 10000);
                if (!string.IsNullOrEmpty(message))
                {
                    message = message.Replace("ok", "");
                    message = message.Replace(" ",  "");
                    string[] positions = message.Split(':');

                    using (var sw = new StreamWriter(Environment.ExpandEnvironmentVariables(FileName), true))
                    {
                        await sw.WriteAsync("g1");
                        if (positions.Length >= 1)
                        {
                            await sw.WriteAsync("X" + positions[0]);
                        }

                        if (positions.Length >= 2)
                        {
                            await sw.WriteAsync("Y" + positions[1]);
                        }

                        if (positions.Length >= 3)
                        {
                            await sw.WriteAsync("Z" + positions[2]);
                        }

                        if (positions.Length >= 4)
                        {
                            await sw.WriteAsync("A" + positions[3]);
                        }

                        if (positions.Length >= 5)
                        {
                            await sw.WriteAsync("B" + positions[4]);
                        }

                        if (positions.Length >= 6)
                        {
                            await sw.WriteAsync("C" + positions[5]);
                        }

                        await sw.WriteLineAsync();
                    }
                }
            });
    }

    public bool CanSendSDCommand()
    {
        return CanSend() && _global.Machine.SDSupport;
    }

    public bool CanSendFileNameCommand()
    {
        return Connected && File.Exists(FileName);
    }

    public bool CanSendSDFileNameCommand()
    {
        return CanSendSDCommand() && !string.IsNullOrEmpty(SDFileName);
    }

    public bool CanSendFileNameAndSDFileNameCommand()
    {
        return CanSendSDCommand() && CanSendSDFileNameCommand() && CanSendFileNameCommand();
    }

    #endregion

    #region ICommand

    public ICommand SendM20FileCommand => new DelegateCommand(SendM20File, CanSendSDCommand);
    public ICommand SendM24FileCommand => new DelegateCommand(SendM24File, CanSendSDFileNameCommand);

    public ICommand SendM28FileCommand =>
        new DelegateCommand(SendM28File, CanSendFileNameAndSDFileNameCommand);

    public ICommand SendM28PreViewCommand => new DelegateCommand(SendM28PreView, CanSendSDFileNameCommand);
    public ICommand SendM30FileCommand    => new DelegateCommand(SendM30File,    CanSendSDFileNameCommand);
    public ICommand SendFileDirectCommand => new DelegateCommand(SendFileDirect, CanSendFileNameCommand);

    public ICommand AddToFileCommand =>
        new DelegateCommand(AddToFile, () => CanSendGCode() && CanSendFileNameCommand());

    public ICommand BrowseForSDFileCommand => new DelegateCommand(
        () =>
        {
            string filename = BrowseFileNameFunc?.Invoke(FileName, false);
            if (filename != null)
            {
                FileName = filename;
            }
        },
        CanSend);

    #endregion
}