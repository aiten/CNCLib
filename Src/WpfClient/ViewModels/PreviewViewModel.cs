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

namespace CNCLib.WpfClient.ViewModels;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using CNCLib.GCode.Generate;
using CNCLib.GCode.Generate.Commands;
using CNCLib.Logic.Abstraction.DTO;

using Framework.Arduino.SerialCommunication;
using Framework.Drawing;
using Framework.Wpf.Helpers;
using Framework.Wpf.ViewModels;

public class PreviewViewModel : BaseViewModel
{
    private readonly Global _global;

    #region crt

    public PreviewViewModel(Global global)
    {
        _global                              =  global;
        _global.Com.LocalCom.CommandSending  += CommandSending;
        _global.Com.RemoteCom.CommandSending += CommandSending;
    }

    #endregion

    #region Properties

    public Global Global => _global;

    private CommandList _commands = new CommandList();

    public CommandList Commands
    {
        get => _commands;
        set
        {
            SetProperty(() => _commands == value, () => _commands = value);
            _global.Commands       = value;
            _currentDrawSeqIdToCmd = null;
        }
    }

    private decimal _offsetX = 0;

    public decimal OffsetX
    {
        get => _offsetX;
        set { SetProperty(() => _offsetX == value, () => _offsetX = value); }
    }

    private decimal _offsetY = 0;

    public decimal OffsetY
    {
        get => _offsetY;
        set { SetProperty(() => _offsetY == value, () => _offsetY = value); }
    }

    private double _zoom = 1;

    public double Zoom
    {
        get => _zoom;
        set { SetProperty(() => Math.Abs(_zoom - value) < double.Epsilon, () => _zoom = value); }
    }

    private double _rotateAngle = 0.0;

    public double RotateAngle
    {
        get => _rotateAngle;
        set { SetProperty(() => Math.Abs(_rotateAngle - value) < double.Epsilon, () => _rotateAngle = value); }
    }

    private double[] _rotateVector = new double[] { 0, 0, 1 };

    public double[] RotateVector
    {
        get => _rotateVector;
        set { SetProperty(() => _rotateVector == value, () => _rotateVector = value); }
    }

    private decimal _laserSize = 0.25m;

    public decimal LaserSize
    {
        get => _laserSize;
        set { SetProperty(() => _laserSize == value, () => _laserSize = value); }
    }

    private decimal _cutterSize = 0;

    public decimal CutterSize
    {
        get => _cutterSize;
        set { SetProperty(() => _cutterSize == value, () => _cutterSize = value); }
    }

    private Color _machineColor = Colors.Black;

    public Color MachineColor
    {
        get => _machineColor;
        set { SetProperty(() => _machineColor == value, () => _machineColor = value); }
    }

    private Color _laserOnColor = Colors.Red;

    public Color LaserOnColor
    {
        get => _laserOnColor;
        set { SetProperty(() => _laserOnColor == value, () => _laserOnColor = value); }
    }

    private Color _laserOffColor = Colors.Orange;

    public Color LaserOffColor
    {
        get => _laserOffColor;
        set { SetProperty(() => _laserOffColor == value, () => _laserOffColor = value); }
    }

    private Color _cutColor = Colors.LightGray;

    public Color CutColor
    {
        get => _cutColor;
        set { SetProperty(() => _cutColor == value, () => _cutColor = value); }
    }

    private Color _cutDotColor = Colors.Blue;

    public Color CutDotColor
    {
        get => _cutDotColor;
        set { SetProperty(() => _cutDotColor == value, () => _cutDotColor = value); }
    }

    private Color _cutEllipseColor = Colors.Cyan;

    public Color CutEllipseColor
    {
        get => _cutEllipseColor;
        set { SetProperty(() => _cutEllipseColor == value, () => _cutEllipseColor = value); }
    }

    private Color _cutArcColor = Colors.Beige;

    public Color CutArcColor
    {
        get => _cutArcColor;
        set { SetProperty(() => _cutArcColor == value, () => _cutArcColor = value); }
    }

    private Color _fastColor = Colors.Green;

    public Color FastColor
    {
        get => _fastColor;
        set { SetProperty(() => _fastColor == value, () => _fastColor = value); }
    }

    private Color _helpLineColor = Colors.LightBlue;

    public Color HelpLineColor
    {
        get => _helpLineColor;
        set { SetProperty(() => _helpLineColor == value, () => _helpLineColor = value); }
    }

    private string _cmdHistoryFileName = @"%USERPROFILE%\Documents\Command.txt";

    public string CmdHistoryFileName
    {
        get => _cmdHistoryFileName;
        set { SetProperty(() => _cmdHistoryFileName == value, () => _cmdHistoryFileName = value); }
    }

    #endregion

    #region GUI-forward

    public class GetLoadInfoArg
    {
        public LoadOptions? LoadOption { get; set; }
        public bool         UseAzure   { get; set; }
    }

    public Func<GetLoadInfoArg, bool>? GetLoadInfo { get; set; }

    public Action? RefreshPreview { get; set; }

    #endregion

    #region private/intern

    LoadOptions _loadInfo         = new LoadOptions();
    bool        _useAzure         = false;
    bool        _loadingOrSending = false;

    Command? _lastCurrentCommand = null;

    private void CommandSending(object sender, SerialEventArgs arg)
    {
        int seqId = arg.SeqId;

        if (_currentDrawSeqIdToCmd != null && _currentDrawSeqIdToCmd.ContainsKey(seqId))
        {
            Commands.Current = _currentDrawSeqIdToCmd[seqId];
            if (Commands.Current != _lastCurrentCommand)
            {
                _lastCurrentCommand = Commands.Current;
                RefreshPreview?.Invoke();
            }
        }
    }

    private bool IsHpglAndPlotter()
    {
        if (_global.Machine?.CommandSyntax != CommandSyntax.Hpgl)
        {
            return true;
        }

        if (Commands == null || Commands.Count == 0)
        {
            return true;
        }

        foreach (var c in Commands)
        {
            if (!string.IsNullOrEmpty(c.ImportInfo))
            {
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Operations

    private async Task SendCommands(List<CommandToIndex> list)
    {
        var cmdList = new List<string>();
        int idx     = 0;
        var cmdDict = new Dictionary<int, CommandToIndex>();

        foreach (var c in list)
        {
            c.IdxFrom =  idx;
            c.IdxTo   =  idx + c.CommandText.Length - 1;
            idx       += c.CommandText.Length;
            cmdList.AddRange(c.CommandText);
            for (idx = c.IdxFrom; idx <= c.IdxTo; idx++)
            {
                cmdDict[idx] = c;
            }
        }

        var serialCommands = await _global.Com.Current.QueueCommandsAsync(cmdList);

        foreach (var serialCmd in serialCommands)
        {
            //var ctx = list.FirstOrDefault((ct) => ct.IdxFrom >= serialCmd.CommandIndex && ct.IdxTo <= serialCmd.CommandIndex);
            if (cmdDict.TryGetValue(serialCmd.CommandIndex, out var ctx))
            {
                ctx.SeqIdFrom = Math.Min(ctx.SeqIdFrom, serialCmd.SeqId);
                ctx.SeqIdTo   = Math.Max(ctx.SeqIdTo, serialCmd.SeqId);
            }
        }

        var dict = new Dictionary<int, Command>();
        foreach (var c in list)
        {
            for (idx = c.SeqIdFrom; idx <= c.SeqIdTo; idx++)
            {
                dict[idx] = c.Cmd;
            }
        }

        _currentDrawSeqIdToCmd = dict;
    }

    class CommandToIndex
    {
        public Command  Cmd         { get; set; } = default!;
        public string[] CommandText { get; set; } = default!;
        public int      SeqIdFrom   { get; set; } = int.MaxValue;
        public int      SeqIdTo     { get; set; } = int.MinValue;
        public int      IdxFrom     { get; set; }
        public int      IdxTo       { get; set; }
    }

    private Dictionary<int, Command>? _currentDrawSeqIdToCmd;

    public void SendTo()
    {
        Task.Run(
            async () =>
            {
                _loadingOrSending = true;

                try
                {
                    _global.Com.Current.ClearCommandHistory();
                    var cmdDefList = new List<CommandToIndex>();

                    if (_global.Machine?.CommandSyntax == CommandSyntax.Hpgl && IsHpglAndPlotter())
                    {
                        Commands.ForEach(
                            cmd =>
                            {
                                var cmdStr = cmd.ImportInfo;
                                if (!string.IsNullOrEmpty(cmdStr))
                                {
                                    cmdDefList.Add(
                                        new CommandToIndex()
                                        {
                                            Cmd         = cmd,
                                            CommandText = new[] { cmdStr }
                                        });
                                }
                            });
                    }
                    else
                    {
                        Command? last  = null;
                        var      state = new CommandState();

                        Commands.ForEach(
                            cmd =>
                            {
                                cmdDefList.Add(
                                    new CommandToIndex()
                                    {
                                        Cmd         = cmd,
                                        CommandText = cmd.GetGCodeCommands(last?.CalculatedEndPosition, state).ToArray()
                                    });
                                last = cmd;
                            });
                    }

                    await SendCommands(cmdDefList);
                }
                finally
                {
                    _loadingOrSending = false;
                }
            });
    }

    public async Task Load()
    {
        if (_loadInfo.AutoScaleSizeX == 0 || _loadInfo.AutoScaleSizeY == 0)
        {
            _loadInfo.AutoScaleSizeX = _global.SizeX;
            _loadInfo.AutoScaleSizeY = _global.SizeY;
        }

        var arg = new GetLoadInfoArg { LoadOption = _loadInfo, UseAzure = _useAzure };
        if ((GetLoadInfo?.Invoke(arg)).GetValueOrDefault(false))
        {
            _loadInfo = arg.LoadOption;
            _useAzure = arg.UseAzure;

            var ld = new GCodeLoad();

            try
            {
                _loadingOrSending = true;
                Commands          = await ld.Load(_loadInfo, _useAzure);
            }
            catch (Exception ex)
            {
                MessageBox?.Invoke("Load failed with error: " + ex.Message, "CNCLib", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            finally
            {
                _loadingOrSending = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public bool CanSendTo()
    {
        return !_loadingOrSending && _global.Com.Current.IsConnected && Commands.Count > 0 && IsHpglAndPlotter();
    }

    public bool CanLoad()
    {
        return _loadingOrSending == false;
    }

    public bool CanResetView()
    {
        return true;
    }

    public void ResetViewXY()
    {
        Zoom = 1;

        // set Zoom first, set Zoom adjusts OffsetX/Y
        OffsetX     = 0;
        OffsetY     = 0;
        RotateAngle = 0;
    }

    public void ResetViewXZ()
    {
        Zoom = 1;

        // set Zoom first, set Zoom adjusts OffsetX/Y
        OffsetX      = 0;
        OffsetY      = 0;
        RotateVector = new double[] { 1, 1, 1 };
        RotateAngle  = Math.PI * 4.0 / 3.0;
    }

    public void ResetViewYZ()
    {
        Zoom = 1;

        // set Zoom first, set Zoom adjusts OffsetX/Y
        OffsetX      = 0;
        OffsetY      = 0;
        RotateVector = new double[] { 1, 0, 0 };
        RotateAngle  = -Math.PI / 2.0;
    }

    public void GotoPos(Point3D pt)
    {
        _global.Com.Current.QueueCommand($@"g0 x{(pt.X0).ToString(CultureInfo.InvariantCulture)} y{(pt.Y0).ToString(CultureInfo.InvariantCulture)}");
    }

    public bool CanGotoPos(Point3D pt)
    {
        return !_loadingOrSending && _global.Com.Current.IsConnected;
    }

    #endregion

    #region Commands

    private ICommand? _loadCommand;

    public ICommand? LoadCommand
    {
        get { return _loadCommand ?? (_loadCommand = new DelegateCommand(async () => await Load(), CanLoad)); }
    }

    public ICommand SendToCommand      => new DelegateCommand(SendTo,      CanSendTo);
    public ICommand ResetViewCommandXY => new DelegateCommand(ResetViewXY, CanResetView);
    public ICommand GotoPosCommand     => new DelegateCommand<Point3D>(GotoPos, CanGotoPos);
    public ICommand ResetViewCommandXZ => new DelegateCommand(ResetViewXZ, CanResetView);
    public ICommand ResetViewCommandYZ => new DelegateCommand(ResetViewYZ, CanResetView);

    #endregion
}