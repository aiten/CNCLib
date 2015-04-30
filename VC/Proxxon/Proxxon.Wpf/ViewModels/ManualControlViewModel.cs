////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2015 Herbert Aitenbichler

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Framework.Wpf.ViewModels;
using Framework.Wpf.Helpers;
using Proxxon.Wpf.Models;
using System.Threading;
using System.IO;
using Framework.Logic;
using System.Globalization;

namespace Proxxon.Wpf.ViewModels
{
    public class ManualControlViewModel : BaseViewModel
	{
		public ManualControlViewModel()
		{
			_AxisX = new AxisViewModel(this)
			{
				AxisName = "X",
				AxisIndex = 0,
				Size = Global.Instance.Machine.SizeX,
				ProbeSize = Global.Instance.Machine.ProbeSizeX
			};
			_AxisY = new AxisViewModel(this)
			{
				AxisName = "Y",
				AxisIndex = 1,
				Size = Global.Instance.Machine.SizeY,
				ProbeSize = Global.Instance.Machine.ProbeSizeY
			};
			_AxisZ = new AxisViewModel(this)
			{
				AxisName = "Z",
				AxisIndex = 2,
				HomeIsMax = true,
				Size = Global.Instance.Machine.SizeZ,
				ProbeSize = Global.Instance.Machine.ProbeSizeZ
			};
			_AxisA = new AxisViewModel(this)
			{
				AxisName = "A",
				AxisIndex = 3,
				Size = Global.Instance.Machine.SizeA,
				ProbeSize = 0m
			};
			_AxisB = new AxisViewModel(this)
			{
				AxisName = "B",
				AxisIndex = 4,
				Size = Global.Instance.Machine.SizeB,
				ProbeSize = 0m
			};
			_AxisC = new AxisViewModel(this)
			{
				AxisName = "C",
				AxisIndex = 5,
				Size = Global.Instance.Machine.SizeC,
				ProbeSize = 0m
			};
	
			_CommandHistory = new CommandHistoryViewModel(this) { };

			_sd = new SDViewModel(this) { };

			_directCommand = new DirectCommandViewModel(this) { };

			_shift = new ShiftViewModel(this) { };

			_tool = new ToolViewModel(this) { };

			_rotate = new RotateViewModel(this) { };
		}

		public delegate void ExecuteCommands();

		public class DetailViewModel : BaseViewModel
		{
			public ManualControlViewModel VmX { get; private set; }
			public DetailViewModel(ManualControlViewModel vm)
			{
				VmX = vm;
			}
			public Framework.Logic.ArduinoSerialCommunication Com
			{
				get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
			}
			public bool Connected
			{
				get { return Com.IsConnected; }
			}
			protected void AsyncRunCommand(ExecuteCommands todo)
			{
				VmX.AsyncRunCommand(todo);
			}

			#region Command/CanCommand

			public bool CanSend()
			{
				return Connected;
			}

			#endregion
		}

		#region AxisVM

		public class AxisViewModel : DetailViewModel
		{
			public AxisViewModel(ManualControlViewModel vm) : base(vm)
			{
			}

			#region Properties

			public string AxisName { get; set; }
			public int AxisIndex { get; set; }
			public decimal Size { get; set; }
			public decimal ProbeSize { get; set; }
			public bool HomeIsMax { get; set; }

			private string _param = "0";
			public decimal ParamDec { get { return decimal.Parse(Param); } }
			public string Param
			{
				get { return _param; }
				set { SetProperty(ref _param, value); }
			}

			private string _pos = "";
			public string Pos
			{
				get { return _pos; }
				set { SetProperty(ref _pos, value); }
			}
			public bool Enabled { get { return Global.Instance.Machine.Axis >= AxisIndex && Size > 0m; } }

			#endregion

			#region Commands / CanCommands
			private void SendMoveCommand(string dist) { AsyncRunCommand(() => { Com.SendCommand("g91 g0" + AxisName + dist + " g90"); }); }

			private void SendProbeCommand(string axisname, decimal probesize)
			{
				AsyncRunCommand(() =>
				{
					string probdist = Global.Instance.Machine.ProbeDist.ToString(CultureInfo.InvariantCulture);
					string probdistup = Global.Instance.Machine.ProbeDistUp.ToString(CultureInfo.InvariantCulture);
					string probfeed = Global.Instance.Machine.ProbeFeed.ToString(CultureInfo.InvariantCulture);

					Com.SendCommand("g91 g31 " + axisname + "-" + probdist + " F" + probfeed + " g90");
					if ((Com.CommandHistory.Last().ReplyType & ArduinoSerialCommunication.EReplyType.ReplyError) == 0)
					{
						Com.SendCommand("g92 " + axisname + (-probesize).ToString(CultureInfo.InvariantCulture));
						Com.SendCommand("g91 g0" + axisname + probdistup + " g90");
					}
				});
			}

			public void SendRefMove() { AsyncRunCommand(() => { Com.SendCommand("g28 " + AxisName + "0"); }); }
			public void SendG92() { AsyncRunCommand(() => { Com.SendCommand("g92 " + AxisName + ParamDec.ToString(CultureInfo.InvariantCulture)); }); }
			public void SendG31() { SendProbeCommand(AxisName, ProbeSize); }
			public void SendHome() 
			{ 
				AsyncRunCommand(() => 
				{ 
					if (HomeIsMax) 
						Com.SendCommand("g53 g0"+ AxisName+"#" + (5161+AxisIndex).ToString()); 
					else
						Com.SendCommand("g53 g0" + AxisName + "0");
				}); 
			}

			public bool CanSendCommand()
			{
				return CanSend() && Enabled;
			}

			#endregion

			#region ICommands

			public ICommand SendPlus100Command { get { return new DelegateCommand(() => SendMoveCommand("100"), () => CanSendCommand() && Size >= 100.0m); } }
			public ICommand SendPlus10Command { get { return new DelegateCommand(() => SendMoveCommand("10"), () => CanSendCommand() && Size >= 10.0m); } }
			public ICommand SendPlus1Command { get { return new DelegateCommand(() => SendMoveCommand("1"), () => CanSendCommand() && Size >= 1.0m); } }
			public ICommand SendPlus01Command { get { return new DelegateCommand(() => SendMoveCommand("0.1"), () => CanSendCommand() && Size >= 0.1m); } }
			public ICommand SendPlus001Command { get { return new DelegateCommand(() => SendMoveCommand("0.01"), () => CanSendCommand() && Size >= 0.01m); } }
			public ICommand SendMinus100Command { get { return new DelegateCommand(() => SendMoveCommand("-100"), () => CanSendCommand() && Size >= 100.0m); } }
			public ICommand SendMinus10Command { get { return new DelegateCommand(() => SendMoveCommand("-10"), () => CanSendCommand() && Size >= 10.0m); } }
			public ICommand SendMinus1Command { get { return new DelegateCommand(() => SendMoveCommand("-1"), () => CanSendCommand() && Size >= 1.0m); } }
			public ICommand SendMinus01Command { get { return new DelegateCommand(() => SendMoveCommand("-0.1"), () => CanSendCommand() && Size >= 0.1m); } }
			public ICommand SendMinus001Command { get { return new DelegateCommand(() => SendMoveCommand("-0.01"), () => CanSendCommand() && Size >= 0.01m); } }
			public ICommand SendRefMoveCommand { get { return new DelegateCommand(SendRefMove, CanSendCommand); } }
			public ICommand SendG92Command { get { return new DelegateCommand(SendG92,  () => { decimal dummy; return CanSendCommand() && decimal.TryParse(Param,out dummy); }); } }
			public ICommand SendG31Command { get { return new DelegateCommand(SendG31, CanSendCommand); } }
			public ICommand SendHomeCommand { get { return new DelegateCommand(SendHome, CanSendCommand); } }

			#endregion
		}

		private AxisViewModel _AxisX;
		public AxisViewModel AxisX { get { return _AxisX; } }

		private AxisViewModel _AxisY;
		public AxisViewModel AxisY { get { return _AxisY; } }

		private AxisViewModel _AxisZ;
		public AxisViewModel AxisZ { get { return _AxisZ; } }

		private AxisViewModel _AxisA;
		public AxisViewModel AxisA { get { return _AxisA; } }

		private AxisViewModel _AxisB;
		public AxisViewModel AxisB { get { return _AxisB; } }
		
		private AxisViewModel _AxisC;
		public AxisViewModel AxisC { get { return _AxisC; } }

		#endregion

		#region CommandHistoryVM

		public class CommandHistoryViewModel : DetailViewModel
		{
			public CommandHistoryViewModel(ManualControlViewModel vm) : base(vm)
			{
			}

			public const string CommandHistoryFile = @"c:\tmp\Command.txt";

			#region Properties

			private ObservableCollection<ProxxonCommand> _ProxxonCommandCollection;
			public ObservableCollection<ProxxonCommand> ProxxonCommandCollection
			{
				get { return _ProxxonCommandCollection; }
				set { AssignProperty(ref _ProxxonCommandCollection, value); }
			}

			#endregion

			#region Commands / CanCommands

			public void RefreshAfterCommand()
			{
				RefreshCommandHistory();
			}

			public void RefreshCommandHistory()
			{
				lock (this)
				{
					var results = new ObservableCollection<ProxxonCommand>();

					foreach (ArduinoSerialCommunication.Command rc in Com.CommandHistory)
					{
						results.Add(new ProxxonCommand() { CommandDate = rc.SentTime, CommandText = rc.CommandText, Result = rc.ResultText });

					}
					ProxxonCommandCollection = results;
				}
			}
			public void ClearCommandHistory()
			{
				Com.ClearCommandHistory();
				RefreshCommandHistory();
			}

			#endregion

			#region ICommand

			public ICommand RefreshHistoryCommand { get { return new DelegateCommand(RefreshCommandHistory, CanSend); } }
			public ICommand ClearHistoryCommand { get { return new DelegateCommand(ClearCommandHistory, CanSend); } }

			#endregion
		}

		private CommandHistoryViewModel _CommandHistory;
		public CommandHistoryViewModel CommandHistory { get { return _CommandHistory; } }

		#endregion

		#region SD_VM

		public class SDViewModel : DetailViewModel
		{
			public SDViewModel(ManualControlViewModel vm) : base(vm)
			{
			}

			#region Properties

			private string _fileName = @"c:\tmp\test.GCode";
			public string FileName
			{
				get { return _fileName; }
				set { SetProperty(ref _fileName, value); }
			}

			private string _SDFileName = @"auto0.g";
			public string SDFileName
			{
				get { return _SDFileName; }
				set { SetProperty(ref _SDFileName, value); }
			}

			#endregion

			#region Commands / CanCommands
			public void SendM20File() { AsyncRunCommand(() => { Com.SendCommand("m20"); }); }
			public void SendM24File() { SendM24File(SDFileName); }
			public void SendM24File(string filename)
			{
				AsyncRunCommand(() =>
				{
					Com.SendCommand("m23 " + filename);
					Com.SendCommand("m24");
				});
			}

			public void SendM28File() { SendM28File(FileName, SDFileName); }
			public void SendM28File(string filename, string sDFileName)
			{
				AsyncRunCommand(() =>
				{
					using (StreamReader sr = new StreamReader(filename))
					{
						bool savefileinresponse = false;
						var checkresponse = new Framework.Logic.ArduinoSerialCommunication.CommandEventHandler((obj, e) =>
						{
							savefileinresponse = e.Info.Contains(sDFileName);
						});
						Com.ReplyUnknown += checkresponse;
						Com.SendCommand("m28 " + sDFileName);
						Com.ReplyUnknown -= checkresponse;
						if (savefileinresponse)
						{
							string line;
							while ((line = sr.ReadLine()) != null)
							{
								Com.SendCommand(line);
							}
							bool filesavednresponse = false;
							checkresponse = new Framework.Logic.ArduinoSerialCommunication.CommandEventHandler((obj, e) =>
							{
								filesavednresponse = e.Info.Contains("Done");
							});
							Com.ReplyUnknown += checkresponse;
							Com.SendCommand("m29");
							Com.ReplyUnknown -= checkresponse;
						}
					}
				});
			}
			public void SendM30File() { SendM30File(SDFileName); }
			public void SendM30File(string filename)
			{
				AsyncRunCommand(() =>
				{
					Com.SendCommand("m30 " + filename);
				});
			}
			public void SendFileDirect() { AsyncRunCommand(() => { Com.SendFile(FileName); }); }

			public void AddToFile()
			{
				AsyncRunCommand(() =>
				{
					string message = null;
					var checkresponse = new Framework.Logic.ArduinoSerialCommunication.CommandEventHandler((obj, e) =>
					{
						message = e.Info;
					});
					Com.ReplyOK += checkresponse;
					Com.SendCommand("m114");
					Com.ReplyOK -= checkresponse;
					if (!string.IsNullOrEmpty(message))
					{
						message = message.Replace("ok", "");
						message = message.Replace(" ", "");
						string[] positions = message.Split(':');

						using (StreamWriter sw = new StreamWriter(FileName, true))
						{
							sw.Write("g1");
							if (positions.Length >= 1) sw.Write("X"+positions[0]);
							if (positions.Length >= 2) sw.Write("Y"+positions[1]);
							if (positions.Length >= 3) sw.Write("Z"+positions[2]);
							if (positions.Length >= 4) sw.Write("A"+positions[3]);
							if (positions.Length >= 5) sw.Write("B"+positions[4]);
							if (positions.Length >= 6) sw.Write("C"+positions[5]);
							sw.WriteLine();
						}
					}
				});
			}

			public bool CanSendFileNameCommand()
			{
				return Connected && File.Exists(FileName);
			}
			public bool CanSendSDFileNameCommand()
			{
				return Connected && !string.IsNullOrEmpty(SDFileName);
			}
			public bool CanSendFileNameAndSDFileNameCommand()
			{
				return Connected && CanSendSDFileNameCommand() && CanSendFileNameCommand();
			}


			#endregion

			#region ICommand
			public ICommand SendM20FileCommand { get { return new DelegateCommand(SendM20File, CanSend); } }
			public ICommand SendM24FileCommand { get { return new DelegateCommand(SendM24File, CanSendSDFileNameCommand); } }
			public ICommand SendM28FileCommand { get { return new DelegateCommand(SendM28File, CanSendFileNameAndSDFileNameCommand); } }
			public ICommand SendM30FileCommand { get { return new DelegateCommand(SendM30File, CanSendSDFileNameCommand); } }
			public ICommand SendFileDirectCommand { get { return new DelegateCommand(SendFileDirect, CanSendFileNameCommand); } }
			public ICommand AddToFileCommand { get { return new DelegateCommand(AddToFile, CanSendFileNameCommand); } }

			#endregion
		}

		private SDViewModel _sd;
		public SDViewModel SD { get { return _sd; } }

		#endregion

		#region DirectCommandVM

		public class DirectCommandViewModel : DetailViewModel
		{
			public DirectCommandViewModel(ManualControlViewModel vm) : base(vm)
			{
			}

			#region Properties

			#endregion

			#region DirectCommand

			private string _directCommand;
			public string DirectCommand
			{
				get { return _directCommand; }
				set { SetProperty(ref _directCommand, value); }
			}

			private void AddDirectCommandHistory(string cmd)
			{
				if (_directCommandHistory == null) _directCommandHistory = new ObservableCollection<string>();
				_directCommandHistory.Add(cmd);
				DirectCommandHistory = _directCommandHistory;
			}

			private ObservableCollection<string> _directCommandHistory;
			public ObservableCollection<string> DirectCommandHistory
			{
				get { return _directCommandHistory; }
				set { AssignProperty(ref _directCommandHistory, value); }
			}

			#endregion

			#region Commands / CanCommands

			public void SendDirect() { AsyncRunCommand(() => { Com.SendCommand(DirectCommand); }); AddDirectCommandHistory(DirectCommand); }
			public bool CanSendDirectCommand()
			{
				return Connected && !string.IsNullOrEmpty(DirectCommand);
			}

			#endregion

			#region ICommand
			public ICommand SendDirectCommand { get { return new DelegateCommand(SendDirect, CanSendDirectCommand); } }

			#endregion
		}

		private DirectCommandViewModel _directCommand;
		public DirectCommandViewModel DirectCommand { get { return _directCommand; } }

		#endregion

		#region ShiftVM

		public class ShiftViewModel : DetailViewModel
		{
			public ShiftViewModel(ManualControlViewModel vm)
				: base(vm)
			{
			}

			#region Properties


			#endregion

			#region Commands / CanCommands

			public void SendG53() { AsyncRunCommand(() => { Com.SendCommand("g53"); }); }
			public void SendG54() { AsyncRunCommand(() => { Com.SendCommand("g54"); }); }
			public void SendG55() { AsyncRunCommand(() => { Com.SendCommand("g55"); }); }
			public void SendG56() { AsyncRunCommand(() => { Com.SendCommand("g56"); }); }
			public void SendG57() { AsyncRunCommand(() => { Com.SendCommand("g57"); }); }
			public void SendG58() { AsyncRunCommand(() => { Com.SendCommand("g58"); }); }
			public void SendG59() { AsyncRunCommand(() => { Com.SendCommand("g59"); }); }
			public void SendG92() { AsyncRunCommand(() => { Com.SendCommand("g92"); }); }
			

			#endregion

			#region ICommand
			public ICommand SendG53Command { get { return new DelegateCommand(SendG53, CanSend); } }
			public ICommand SendG54Command { get { return new DelegateCommand(SendG54, CanSend); } }
			public ICommand SendG55Command { get { return new DelegateCommand(SendG55, CanSend); } }
			public ICommand SendG56Command { get { return new DelegateCommand(SendG56, CanSend); } }
			public ICommand SendG57Command { get { return new DelegateCommand(SendG57, CanSend); } }
			public ICommand SendG58Command { get { return new DelegateCommand(SendG58, CanSend); } }
			public ICommand SendG59Command { get { return new DelegateCommand(SendG59, CanSend); } }
			public ICommand SendG92Command { get { return new DelegateCommand(SendG92, CanSend); } }

			#endregion
		}

		private ShiftViewModel _shift;
		public ShiftViewModel Shift { get { return _shift; } }

		#endregion

		#region ToolVM

		public class ToolViewModel : DetailViewModel
		{
			public ToolViewModel(ManualControlViewModel vm)
				: base(vm)
			{
			}

			#region Properties


			#endregion

			#region Commands / CanCommands

			public void SendInfo() { AsyncRunCommand(() => { Com.SendCommand("?"); }); }
			public void SendAbort() { AsyncRunCommand(() => { Com.AbortCommands(); Com.ResumAfterAbort(); Com.SendCommand("!"); }); }
			public void SendProxxonCommand(string command) { AsyncRunCommand(() => { Com.SendCommand(command); }); }
			public void SendM03SpindelOn() { AsyncRunCommand(() => { Com.SendCommand("m3"); }); }
			public void SendM05SpindelOff() { AsyncRunCommand(() => { Com.SendCommand("m5"); }); }
			public void SendM07CoolandOn() { AsyncRunCommand(() => { Com.SendCommand("m7"); }); }
			public void SendM09CoolandOff() { AsyncRunCommand(() => { Com.SendCommand("m9"); }); }
			public void SendM114PrintPos()
			{
				AsyncRunCommand(() =>
				{
					string message = null;
					var checkresponse = new Framework.Logic.ArduinoSerialCommunication.CommandEventHandler((obj, e) =>
					{
						message = e.Info;
					});
					Com.ReplyOK += checkresponse;
					Com.SendCommand("m114");
					Com.ReplyOK -= checkresponse;
					if (!string.IsNullOrEmpty(message))
					{
						message = message.Replace("ok", "");
						message = message.Replace(" ", "");
						string[] positions = message.Split(':');
						if (positions.Length >= 1) VmX.AxisX.Pos = positions[0];
						if (positions.Length >= 2) VmX.AxisY.Pos = positions[1];
						if (positions.Length >= 3) VmX.AxisZ.Pos = positions[2];
						if (positions.Length >= 4) VmX.AxisA.Pos = positions[3];
						if (positions.Length >= 5) VmX.AxisB.Pos = positions[4];
						if (positions.Length >= 6) VmX.AxisC.Pos = positions[5];
					}
				});
			}


			#endregion

			#region ICommand
			public ICommand SendInfoCommand { get { return new DelegateCommand(SendInfo, CanSend); } }
			public ICommand SendAbortCommand { get { return new DelegateCommand(SendAbort, CanSend); } }
			public ICommand SendM03SpindelOnCommand { get { return new DelegateCommand(SendM03SpindelOn, CanSend); } }
			public ICommand SendM05SpindelOffCommand { get { return new DelegateCommand(SendM05SpindelOff, CanSend); } }
			public ICommand SendM07CoolandOnCommand { get { return new DelegateCommand(SendM07CoolandOn, CanSend); } }
			public ICommand SendM09CoolandOffCommand { get { return new DelegateCommand(SendM09CoolandOff, CanSend); } }
			public ICommand SendM114Command { get { return new DelegateCommand(SendM114PrintPos, CanSend); } }

			#endregion
		}

		private ToolViewModel _tool;
		public ToolViewModel Tool { get { return _tool; } }

		#endregion

		#region RotateVM

		public class RotateViewModel : DetailViewModel
		{
			public RotateViewModel(ManualControlViewModel vm)
				: base(vm)
			{
			}

			#region Properties


			#endregion

			#region Commands / CanCommands

			public void SendG69()							{ AsyncRunCommand(() => { Com.SendCommand("g69"); }); }
			public void SendG68X0Y0R90()					{ AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r90"); }); }
			public void SendG68X0Y0R270()                   { AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r270"); }); }

			#endregion

			#region ICommand
			public ICommand SendG69Command { get { return new DelegateCommand(SendG69, CanSend); } }
			public ICommand SendG68X0Y0R90Command { get { return new DelegateCommand(SendG68X0Y0R90, CanSend); } }
			public ICommand SendG68X0Y0R270Command { get { return new DelegateCommand(SendG68X0Y0R270, CanSend); } }

			#endregion
		}

		private RotateViewModel _rotate;
		public RotateViewModel Rotate { get { return _rotate; } }

		#endregion

		#region Properties

		public Framework.Logic.ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
        }

        public bool Connected
        {
            get { return Com.IsConnected; }
        }

		#endregion

		#region Operations

		protected void AsyncRunCommand(ExecuteCommands todo)
		{
			new Thread(() =>
			{
				try
				{
					todo();
					Com.WriteCommandHistory(CommandHistoryViewModel.CommandHistoryFile);
				}
				finally
				{
					CommandHistory.RefreshAfterCommand();
				}
			}
			).Start();
		}

		#endregion
	}
}
