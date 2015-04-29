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
		}

		#region AxisVM

		public class AxisViewModel : BaseViewModel
		{
			public ManualControlViewModel Vm { get; private set; }
			public AxisViewModel(ManualControlViewModel vm)
			{
				Vm = vm;
			}
			public Framework.Logic.ArduinoSerialCommunication Com
			{
				get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
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
			private void SendMoveCommand(string dist) { Vm.AsyncRunCommand(() => { Com.SendCommand("g91 g0" + AxisName + dist + " g90"); }); }

			private void SendProbeCommand(string axisname, decimal probesize)
			{
				Vm.AsyncRunCommand(() =>
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

			public void SendRefMove() { Vm.AsyncRunCommand(() => { Com.SendCommand("g28 " + AxisName + "0"); }); }
			public void SendG92() { Vm.AsyncRunCommand(() => { Com.SendCommand("g92 " + AxisName + ParamDec.ToString(CultureInfo.InvariantCulture)); }); }
			public void SendG31() { SendProbeCommand(AxisName, ProbeSize); }
			public void SendHome() 
			{ 
				Vm.AsyncRunCommand(() => 
				{ 
					if (HomeIsMax) 
						Com.SendCommand("g53 g0"+ AxisName+"#" + (5161+AxisIndex).ToString()); 
					else
						Com.SendCommand("g53 g0" + AxisName + "0");
				}); 
			}

			public bool CanSendCommand()
			{
				return Vm.CanSendCommand() && Enabled;
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

		public class CommandHistoryViewModel : BaseViewModel
		{
			public ManualControlViewModel Vm { get; private set; }
			public CommandHistoryViewModel(ManualControlViewModel vm)
			{
				Vm = vm;
			}
			public Framework.Logic.ArduinoSerialCommunication Com
			{
				get { return Framework.Tools.Singleton<Framework.Logic.ArduinoSerialCommunication>.Instance; }
			}

			public const string CommandHistoryFile = @"c:\tmp\Command.txt";

			#region ProxxonCommandCollection

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

			public ICommand RefreshHistoryCommand { get { return new DelegateCommand(RefreshCommandHistory, Vm.CanSendCommand); } }
			public ICommand ClearHistoryCommand { get { return new DelegateCommand(ClearCommandHistory, Vm.CanSendCommand); } }

			#endregion
		}

		private CommandHistoryViewModel _CommandHistory;
		public CommandHistoryViewModel CommandHistory { get { return _CommandHistory; } }

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

        #region SDFileName

		private string _SDFileName = @"auto0.g";
		public string SDFileName
        {
			get { return _SDFileName; }
            set { SetProperty(ref _SDFileName, value);  }
        }

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

		#endregion

		#region FileName

		private string _fileName = @"c:\tmp\test.GCode";
		public string FileName
		{
			get { return _fileName; }
            set { SetProperty(ref _fileName, value);  }
		}

		#endregion

		#region Operations

		protected delegate void ExecuteCommands();

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

		#region Rotation

		public void SendG69()							{ AsyncRunCommand(() => { Com.SendCommand("g69"); }); }
		public void SendG68X0Y0R90()					{ AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r90"); }); }
        public void SendG68X0Y0R270()                   { AsyncRunCommand(() => { Com.SendCommand("g68 x0y0r270"); }); }
        
        #endregion

		public void SendInfo()							{ AsyncRunCommand(() => { Com.SendCommand("?"); });  }
        public void SendAbort()                         { AsyncRunCommand(() => { Com.AbortCommands(); Com.ResumAfterAbort(); Com.SendCommand("!"); }); }
		public void SendDirect()						{ AsyncRunCommand(() => { Com.SendCommand(DirectCommand); }); AddDirectCommandHistory(DirectCommand); }
		public void SendFileDirect()					{ AsyncRunCommand(() => { Com.SendFile(FileName); }); }
		public void SendProxxonCommand(string command)	{ AsyncRunCommand(() => { Com.SendCommand(command); }); }
		public void SendM20File()						{ AsyncRunCommand(() => { Com.SendCommand("m20"); }); }
		public void SendM24File()						{ SendM24File(SDFileName);		}
		public void SendM24File(string filename)
		{
			AsyncRunCommand(() =>
			{
				Com.SendCommand("m23 " + filename);
				Com.SendCommand("m24");
			});
		}

		public void SendM28File()						{	SendM28File(FileName,SDFileName);	}
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
		public void SendM03SpindelOn()	{ AsyncRunCommand(() => { Com.SendCommand("m3"); }); }
		public void SendM05SpindelOff() { AsyncRunCommand(() => { Com.SendCommand("m5"); }); }
		public void SendM07CoolandOn()	{ AsyncRunCommand(() => { Com.SendCommand("m7"); }); }
		public void SendM09CoolandOff() { AsyncRunCommand(() => { Com.SendCommand("m9"); }); }
		public void SendM114PrintPos()	
		{
			AsyncRunCommand(() => 
			{
				string message=null;
				var checkresponse = new Framework.Logic.ArduinoSerialCommunication.CommandEventHandler((obj, e) => 
				{
					message = e.Info;
				});
				Com.ReplyOK += checkresponse;
				Com.SendCommand("m114");
				Com.ReplyOK -= checkresponse;
				if (!string.IsNullOrEmpty(message))
				{
					message = message.Replace("ok","");
					message = message.Replace(" ","");
					string[] positions = message.Split(':');
					if (positions.Length >= 1) AxisX.Pos = positions[0];
					if (positions.Length >= 2) AxisY.Pos = positions[1];
					if (positions.Length >= 3) AxisZ.Pos = positions[2];
					if (positions.Length >= 4) AxisA.Pos = positions[3];
					if (positions.Length >= 5) AxisB.Pos = positions[4];
					if (positions.Length >= 6) AxisC.Pos = positions[5];
				}
			});
		}

		public void SendG53() { AsyncRunCommand(() => { Com.SendCommand("g53"); }); }
		public void SendG54() { AsyncRunCommand(() => { Com.SendCommand("g54"); }); }
		public void SendG55() { AsyncRunCommand(() => { Com.SendCommand("g55"); }); }
		public void SendG56() { AsyncRunCommand(() => { Com.SendCommand("g56"); }); }
		public void SendG57() { AsyncRunCommand(() => { Com.SendCommand("g57"); }); }
		public void SendG58() { AsyncRunCommand(() => { Com.SendCommand("g58"); }); }
		public void SendG59() { AsyncRunCommand(() => { Com.SendCommand("g59"); }); }
		public void SendG92() { AsyncRunCommand(() => { Com.SendCommand("g92"); }); }

		#region CanCommand

		public bool CanSendCommand()
        {
            return Connected;
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
		public bool CanSendDirectCommand()
		{
			return Connected && !string.IsNullOrEmpty(DirectCommand);
		}

		#endregion

		#endregion

		#region Commands

		#region Rotation

		public ICommand SendG69Command { get { return new DelegateCommand(SendG69, CanSendCommand); } }
		public ICommand SendG68X0Y0R90Command { get { return new DelegateCommand(SendG68X0Y0R90, CanSendCommand); } }
        public ICommand SendG68X0Y0R270Command { get { return new DelegateCommand(SendG68X0Y0R270, CanSendCommand); } }

		#endregion

		public ICommand SendDirectCommand		{ get { return new DelegateCommand(SendDirect, CanSendDirectCommand); } }
		public ICommand SendFileDirectCommand	{ get { return new DelegateCommand(SendFileDirect, CanSendFileNameCommand); } }
        public ICommand SendInfoCommand			{ get { return new DelegateCommand(SendInfo, CanSendCommand); } }
        public ICommand SendAbortCommand		{ get { return new DelegateCommand(SendAbort, CanSendCommand); } }
		public ICommand SendM03SpindelOnCommand { get { return new DelegateCommand(SendM03SpindelOn, CanSendCommand); } }
		public ICommand SendM05SpindelOffCommand { get { return new DelegateCommand(SendM05SpindelOff, CanSendCommand); } }
		public ICommand SendM07CoolandOnCommand { get { return new DelegateCommand(SendM07CoolandOn, CanSendCommand); } }
		public ICommand SendM09CoolandOffCommand { get { return new DelegateCommand(SendM09CoolandOff, CanSendCommand); } }
		public ICommand SendM20FileCommand { get { return new DelegateCommand(SendM20File, CanSendCommand); } }
		public ICommand SendM24FileCommand { get { return new DelegateCommand(SendM24File, CanSendSDFileNameCommand); } }
		public ICommand SendM28FileCommand { get { return new DelegateCommand(SendM28File, CanSendFileNameAndSDFileNameCommand); } }
		public ICommand SendM30FileCommand { get { return new DelegateCommand(SendM30File, CanSendSDFileNameCommand); } }
		public ICommand SendM114Command { get { return new DelegateCommand(SendM114PrintPos, CanSendCommand); } }
		public ICommand SendG53Command { get { return new DelegateCommand(SendG53, CanSendCommand); } }
		public ICommand SendG54Command { get { return new DelegateCommand(SendG54, CanSendCommand); } }
		public ICommand SendG55Command { get { return new DelegateCommand(SendG55, CanSendCommand); } }
		public ICommand SendG56Command { get { return new DelegateCommand(SendG56, CanSendCommand); } }
		public ICommand SendG57Command { get { return new DelegateCommand(SendG57, CanSendCommand); } }
		public ICommand SendG58Command { get { return new DelegateCommand(SendG58, CanSendCommand); } }
		public ICommand SendG59Command { get { return new DelegateCommand(SendG59, CanSendCommand); } }
		public ICommand SendG92Command			{ get { return new DelegateCommand(SendG92, CanSendCommand); } }

        #endregion
    }
}
