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
        #region Properties

		const string _probeFeedrate = "F100";
		const string _probeDist = "-10";
		const string _probeUp   = "3";

		const string _commandHistoryFile = @"c:\tmp\Command.txt";

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
            set { SetProperty(ref _directCommand, value);  }
        }

        #endregion

		#region XYZName

		private string _XParam;
		public decimal XParamDec { get { return decimal.Parse(XParam); } }
		public string XParam
		{
			get { return _XParam; }
            set { SetProperty(ref _XParam, value);  }
		}

		private string _XPos;
		public string XPos
		{
			get { return _XPos; }
			private set { SetProperty(ref _XPos, value); }
		}

		private string _YParam;
		public decimal YParamDec { get { return decimal.Parse(YParam); } }
		public string YParam
		{
			get { return _YParam; }
            set { SetProperty(ref _YParam, value);  }
		}
		private string _YPos;
		public string YPos
		{
			get { return _YPos; }
			private set { SetProperty(ref _YPos, value); }
		}

		private string _ZParam;
		public decimal ZParamDec { get { return decimal.Parse(ZParam); } }
		public string ZParam
		{
			get { return _ZParam; }
            set { SetProperty(ref _ZParam, value);  }
		}
		private string _ZPos;
		public string ZPos
		{
			get { return _ZPos; }
			private set { SetProperty(ref _ZPos, value); }
		}
		private string _AParam;
		public decimal AParamDec { get { return decimal.Parse(AParam); } }
		public string AParam
		{
			get { return _AParam; }
            set { SetProperty(ref _AParam, value);  }
		}
		private string _APos;
		public string APos
		{
			get { return _APos; }
			private set { SetProperty(ref _APos, value); }
		}
		private string _BParam;
		public decimal BParamDec { get { return decimal.Parse(BParam); } }
		public string BParam
		{
			get { return _BParam; }
            set { SetProperty(ref _BParam, value);  }
		}
		private string _BPos;
		public string BPos
		{
			get { return _BPos; }
			private set { SetProperty(ref _BPos, value); }
		}
		private string _CParam;
		public decimal CParamDec { get { return decimal.Parse(CParam); } }
		public string CParam
		{
			get { return _CParam; }
            set { SetProperty(ref _CParam, value);  }
		}
		private string _CPos;
		public string CPos
		{
			get { return _CPos; }
			private set { SetProperty(ref _CPos, value); }
		}

		#endregion

		#region Probe

		private decimal _ProbeSize = 25.00m;
		public decimal ProbeSize
		{
			get { return _ProbeSize; }
            set { SetProperty(ref _ProbeSize, value);  }
		}

		#endregion

		#region FileName

		private string _fileName = @"c:\tmp\test.GCode";
		public string FileName
		{
			get { return _fileName; }
            set { SetProperty(ref _fileName, value);  }
		}

		#endregion

        #region ProxxonCommandCollection

        private ObservableCollection<ProxxonCommand> _ProxxonCommandCollection;
        public ObservableCollection<ProxxonCommand> ProxxonCommandCollection
        {
            get { return _ProxxonCommandCollection; }
			set { AssignProperty(ref _ProxxonCommandCollection, value); }
        }

        #endregion

		#endregion

		#region Operations

		delegate void ExecuteCommands();

		void AsyncRunCommand(ExecuteCommands todo)
		{
			new Thread(() =>
			{
				try
				{
					todo();
					Com.WriteCommandHistory(_commandHistoryFile);
				}
				finally
				{
					RefreshAfterCommand();
				}
			}
			).Start();
		}

		#region XYZ Operations

		private void SendMoveCommand(string axisvalue)	{	AsyncRunCommand(() => { Com.SendCommand("g91 g0" + axisvalue + " g90"); });	}

		private void SendProbeCommand(string axisname) 
		{	
			AsyncRunCommand(() => 
			{ 
				Com.SendCommand("g91 g31 " + axisname + _probeDist + " " + _probeFeedrate + " g90"); 
				if ((Com.CommandHistory.Last().ReplyType & ArduinoSerialCommunication.EReplyType.ReplyError) == 0)
				{
					Com.SendCommand("g92" + axisname + (-ProbeSize).ToString(CultureInfo.InvariantCulture));
					Com.SendCommand("g91 g0" + axisname + _probeUp + " g90");
				}
			}); 
		}

		#region X

		public void SendXPlus10()		{ SendMoveCommand("X10"); }
		public void SendXPlus1()		{ SendMoveCommand("X1"); }
		public void SendXPlus01()		{ SendMoveCommand("X0.1"); }
		public void SendXPlus001()		{ SendMoveCommand("X0.01"); }
		public void SendXMinus10()		{ SendMoveCommand("X-10"); }
		public void SendXMinus1()		{ SendMoveCommand("X-1"); }
		public void SendXMinus01()		{ SendMoveCommand("X-0.1"); }
		public void SendXMinus001()		{ SendMoveCommand("X-0.01"); }
		public void SendXRefMove()		{ AsyncRunCommand(() => { Com.SendCommand("g28 X0"); }); }
		public void SendXG92()			{ AsyncRunCommand(() => { Com.SendCommand("g92 X" + XParamDec.ToString(CultureInfo.InvariantCulture)); }); }
		public void SendXG31()			{ SendProbeCommand("X"); }

		#endregion

		#region Y

		public void SendYPlus10()		{ SendMoveCommand("Y10"); }
		public void SendYPlus1()		{ SendMoveCommand("Y1"); }
		public void SendYPlus01()		{ SendMoveCommand("Y0.1"); }
		public void SendYPlus001()		{ SendMoveCommand("Y0.01"); }
		public void SendYMinus10()		{ SendMoveCommand("Y-10"); }
		public void SendYMinus1()		{ SendMoveCommand("Y-1"); }
		public void SendYMinus01()		{ SendMoveCommand("Y-0.1"); }
		public void SendYMinus001()		{ SendMoveCommand("Y-0.01"); }
		public void SendYRefMove()		{ AsyncRunCommand(() => { Com.SendCommand("g28 Y0"); }); }
		public void SendYG92()			{ AsyncRunCommand(() => { Com.SendCommand("g92 Y" + YParamDec.ToString(CultureInfo.InvariantCulture)); }); }
		public void SendYG31()			{ SendProbeCommand("Y"); }

		#endregion

		#region Z

		public void SendZPlus10()		{ SendMoveCommand("Z10"); }
		public void SendZPlus1()		{ SendMoveCommand("Z1"); }
		public void SendZPlus01()		{ SendMoveCommand("Z0.1"); }
		public void SendZPlus001()		{ SendMoveCommand("Z0.01"); }
		public void SendZMinus10()		{ SendMoveCommand("Z-10"); }
		public void SendZMinus1()		{ SendMoveCommand("Z-1"); }
		public void SendZMinus01()		{ SendMoveCommand("Z-0.1"); }
		public void SendZMinus001()		{ SendMoveCommand("Z-0.01"); }
		public void SendZRefMove()		{ AsyncRunCommand(() => { Com.SendCommand("g28 Z0"); }); }
		public void SendZG92()			{ AsyncRunCommand(() => { Com.SendCommand("g92 Z" + ZParamDec.ToString(CultureInfo.InvariantCulture)); }); }
		public void SendZG31()			{ SendProbeCommand("Z"); }

		#endregion

		#region A

		public void SendAPlus10()		{ SendMoveCommand("A10"); }
		public void SendAPlus1()		{ SendMoveCommand("A1"); }
		public void SendAPlus01()		{ SendMoveCommand("A0.1"); }
		public void SendAPlus001()		{ SendMoveCommand("A0.01"); }
		public void SendAMinus10()		{ SendMoveCommand("A-10"); }
		public void SendAMinus1()		{ SendMoveCommand("A-1"); }
		public void SendAMinus01()		{ SendMoveCommand("A-0.1"); }
		public void SendAMinus001()		{ SendMoveCommand("A-0.01"); }
		public void SendARefMove()		{ AsyncRunCommand(() => { Com.SendCommand("g28 A0"); }); }
		public void SendAG92()			{ AsyncRunCommand(() => { Com.SendCommand("g92 A" + AParamDec.ToString(CultureInfo.InvariantCulture)); }); }

		#endregion

		#region B

		public void SendBPlus10()		{ SendMoveCommand("B10"); }
		public void SendBPlus1()		{ SendMoveCommand("B1"); }
		public void SendBPlus01()		{ SendMoveCommand("B0.1"); }
		public void SendBPlus001()		{ SendMoveCommand("B0.01"); }
		public void SendBMinus10()		{ SendMoveCommand("B-10"); }
		public void SendBMinus1()		{ SendMoveCommand("B-1"); }
		public void SendBMinus01()		{ SendMoveCommand("B-0.1"); }
		public void SendBMinus001()		{ SendMoveCommand("B-0.01"); }
		public void SendBRefMove()		{ AsyncRunCommand(() => { Com.SendCommand("g28 B0"); }); }
		public void SendBG92()			{ AsyncRunCommand(() => { Com.SendCommand("g92 B" + BParamDec.ToString(CultureInfo.InvariantCulture)); }); }

		#endregion

		#region C

		public void SendCPlus10()		{ SendMoveCommand("C10"); }
		public void SendCPlus1()		{ SendMoveCommand("C1"); }
		public void SendCPlus01()		{ SendMoveCommand("C0.1"); }
		public void SendCPlus001()		{ SendMoveCommand("C0.01"); }
		public void SendCMinus10()		{ SendMoveCommand("C-10"); }
		public void SendCMinus1()		{ SendMoveCommand("C-1"); }
		public void SendCMinus01()		{ SendMoveCommand("C-0.1"); }
		public void SendCMinus001()		{ SendMoveCommand("C-0.01"); }
		public void SendCRefMove()		{ AsyncRunCommand(() => { Com.SendCommand("g28 C0"); }); }
		public void SendCG92()			{ AsyncRunCommand(() => { Com.SendCommand("g92 C" + CParamDec.ToString(CultureInfo.InvariantCulture)); }); }

		#endregion

		#endregion

		public void SendInfo()							{ AsyncRunCommand(() => { Com.SendCommand("?"); });  }
        public void SendAbort()							{ AsyncRunCommand(() => { Com.SendCommand("!"); });  }
        public void SendDirect()						{ AsyncRunCommand(() => { Com.SendCommand(DirectCommand); }); }
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
					if (positions.Length >= 1) XPos = positions[0];
					if (positions.Length >= 2) YPos = positions[1];
					if (positions.Length >= 3) ZPos = positions[2];
					if (positions.Length >= 4) APos = positions[3];
					if (positions.Length >= 5) BPos = positions[4];
					if (positions.Length >= 6) CPos = positions[5];
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

		#region CommandHistory

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

		public bool CanSendCommandXDecimal()
		{
			decimal dummy;
			return Connected && decimal.TryParse(XParam,out dummy);
		}
		public bool CanSendCommandYDecimal()
		{
			decimal dummy;
			return Connected && decimal.TryParse(YParam, out dummy);
		}
		public bool CanSendCommandZDecimal()
		{
			decimal dummy;
			return Connected && decimal.TryParse(ZParam, out dummy);
		}
		public bool CanSendCommandADecimal()
		{
			decimal dummy;
			return Connected && decimal.TryParse(AParam, out dummy);
		}
		public bool CanSendCommandBDecimal()
		{
			decimal dummy;
			return Connected && decimal.TryParse(BParam, out dummy);
		}
		public bool CanSendCommandCDecimal()
		{
			decimal dummy;
			return Connected && decimal.TryParse(CParam, out dummy);
		}

		#endregion

		#endregion

		#region Commands

		#region XYZ Commands

		#region X

		public ICommand SendXPlus10Command	{ get { return new DelegateCommand(SendXPlus10, CanSendCommand); } }
		public ICommand SendXPlus1Command	{ get { return new DelegateCommand(SendXPlus1, CanSendCommand); } }
		public ICommand SendXPlus01Command	{ get { return new DelegateCommand(SendXPlus01, CanSendCommand); } }
		public ICommand SendXPlus001Command { get { return new DelegateCommand(SendXPlus001, CanSendCommand); } }
		public ICommand SendXMinus10Command { get { return new DelegateCommand(SendXMinus10, CanSendCommand); } }
		public ICommand SendXMinus1Command	{ get { return new DelegateCommand(SendXMinus1, CanSendCommand); } }
		public ICommand SendXMinus01Command { get { return new DelegateCommand(SendXMinus01, CanSendCommand); } }
		public ICommand SendXMinus001Command { get { return new DelegateCommand(SendXMinus001, CanSendCommand); } }
		public ICommand SendXRefMoveCommand	{ get { return new DelegateCommand(SendXRefMove, CanSendCommand); } }
		public ICommand SendXG92Command		{ get { return new DelegateCommand(SendXG92, CanSendCommandXDecimal); } }
		public ICommand SendXG31Command		{ get { return new DelegateCommand(SendXG31, CanSendCommand); } }

		#endregion

		#region Y

		public ICommand SendYPlus10Command { get { return new DelegateCommand(SendYPlus10, CanSendCommand); } }
		public ICommand SendYPlus1Command { get { return new DelegateCommand(SendYPlus1, CanSendCommand); } }
		public ICommand SendYPlus01Command { get { return new DelegateCommand(SendYPlus01, CanSendCommand); } }
		public ICommand SendYPlus001Command { get { return new DelegateCommand(SendYPlus001, CanSendCommand); } }
		public ICommand SendYMinus10Command { get { return new DelegateCommand(SendYMinus10, CanSendCommand); } }
		public ICommand SendYMinus1Command { get { return new DelegateCommand(SendYMinus1, CanSendCommand); } }
		public ICommand SendYMinus01Command { get { return new DelegateCommand(SendYMinus01, CanSendCommand); } }
		public ICommand SendYMinus001Command { get { return new DelegateCommand(SendYMinus001, CanSendCommand); } }
		public ICommand SendYRefMoveCommand { get { return new DelegateCommand(SendYRefMove, CanSendCommand); } }
		public ICommand SendYG92Command { get { return new DelegateCommand(SendYG92, CanSendCommandYDecimal); } }
		public ICommand SendYG31Command { get { return new DelegateCommand(SendYG31, CanSendCommand); } }

		#endregion

		#region Z

		public ICommand SendZPlus10Command { get { return new DelegateCommand(SendZPlus10, CanSendCommand); } }
		public ICommand SendZPlus1Command { get { return new DelegateCommand(SendZPlus1, CanSendCommand); } }
		public ICommand SendZPlus01Command { get { return new DelegateCommand(SendZPlus01, CanSendCommand); } }
		public ICommand SendZPlus001Command { get { return new DelegateCommand(SendZPlus001, CanSendCommand); } }
		public ICommand SendZMinus10Command { get { return new DelegateCommand(SendZMinus10, CanSendCommand); } }
		public ICommand SendZMinus1Command { get { return new DelegateCommand(SendZMinus1, CanSendCommand); } }
		public ICommand SendZMinus01Command { get { return new DelegateCommand(SendZMinus01, CanSendCommand); } }
		public ICommand SendZMinus001Command { get { return new DelegateCommand(SendZMinus001, CanSendCommand); } }
		public ICommand SendZRefMoveCommand { get { return new DelegateCommand(SendZRefMove, CanSendCommand); } }
		public ICommand SendZG92Command { get { return new DelegateCommand(SendZG92, CanSendCommandZDecimal); } }
		public ICommand SendZG31Command { get { return new DelegateCommand(SendZG31, CanSendCommand); } }

		#endregion

		#region A

		public ICommand SendAPlus10Command { get { return new DelegateCommand(SendAPlus10, CanSendCommand); } }
		public ICommand SendAPlus1Command { get { return new DelegateCommand(SendAPlus1, CanSendCommand); } }
		public ICommand SendAPlus01Command { get { return new DelegateCommand(SendAPlus01, CanSendCommand); } }
		public ICommand SendAPlus001Command { get { return new DelegateCommand(SendAPlus001, CanSendCommand); } }
		public ICommand SendAMinus10Command { get { return new DelegateCommand(SendAMinus10, CanSendCommand); } }
		public ICommand SendAMinus1Command { get { return new DelegateCommand(SendAMinus1, CanSendCommand); } }
		public ICommand SendAMinus01Command { get { return new DelegateCommand(SendAMinus01, CanSendCommand); } }
		public ICommand SendAMinus001Command { get { return new DelegateCommand(SendAMinus001, CanSendCommand); } }
		public ICommand SendARefMoveCommand { get { return new DelegateCommand(SendARefMove, CanSendCommand); } }
		public ICommand SendAG92Command { get { return new DelegateCommand(SendAG92, CanSendCommandADecimal); } }

		#endregion

		#region B

		public ICommand SendBPlus10Command { get { return new DelegateCommand(SendBPlus10, CanSendCommand); } }
		public ICommand SendBPlus1Command { get { return new DelegateCommand(SendBPlus1, CanSendCommand); } }
		public ICommand SendBPlus01Command { get { return new DelegateCommand(SendBPlus01, CanSendCommand); } }
		public ICommand SendBPlus001Command { get { return new DelegateCommand(SendBPlus001, CanSendCommand); } }
		public ICommand SendBMinus10Command { get { return new DelegateCommand(SendBMinus10, CanSendCommand); } }
		public ICommand SendBMinus1Command { get { return new DelegateCommand(SendBMinus1, CanSendCommand); } }
		public ICommand SendBMinus01Command { get { return new DelegateCommand(SendBMinus01, CanSendCommand); } }
		public ICommand SendBMinus001Command { get { return new DelegateCommand(SendBMinus001, CanSendCommand); } }
		public ICommand SendBRefMoveCommand { get { return new DelegateCommand(SendBRefMove, CanSendCommand); } }
		public ICommand SendBG92Command { get { return new DelegateCommand(SendBG92, CanSendCommandBDecimal); } }
		#endregion

		#region C

		public ICommand SendCPlus10Command { get { return new DelegateCommand(SendCPlus10, CanSendCommand); } }
		public ICommand SendCPlus1Command { get { return new DelegateCommand(SendCPlus1, CanSendCommand); } }
		public ICommand SendCPlus01Command { get { return new DelegateCommand(SendCPlus01, CanSendCommand); } }
		public ICommand SendCPlus001Command { get { return new DelegateCommand(SendCPlus001, CanSendCommand); } }
		public ICommand SendCMinus10Command { get { return new DelegateCommand(SendCMinus10, CanSendCommand); } }
		public ICommand SendCMinus1Command { get { return new DelegateCommand(SendCMinus1, CanSendCommand); } }
		public ICommand SendCMinus01Command { get { return new DelegateCommand(SendCMinus01, CanSendCommand); } }
		public ICommand SendCMinus001Command { get { return new DelegateCommand(SendCMinus001, CanSendCommand); } }
		public ICommand SendCRefMoveCommand { get { return new DelegateCommand(SendCRefMove, CanSendCommand); } }
		public ICommand SendCG92Command { get { return new DelegateCommand(SendCG92, CanSendCommandCDecimal); } }

		#endregion

		#endregion

		public ICommand SendDirectCommand { get { return new DelegateCommand(SendDirect, CanSendDirectCommand); } }
		public ICommand SendFileDirectCommand { get { return new DelegateCommand(SendFileDirect, CanSendFileNameCommand); } }
		public ICommand RefreshHistoryCommand { get { return new DelegateCommand(RefreshCommandHistory, CanSendCommand); } }
        public ICommand ClearHistoryCommand		{ get { return new DelegateCommand(ClearCommandHistory, CanSendCommand); } }
        public ICommand SendInfoCommand			{ get { return new DelegateCommand(SendInfo, CanSendCommand); } }
        public ICommand SendAbortCommand		{ get { return new DelegateCommand(SendAbort, CanSendCommand); } }
		public ICommand SendM03SpindelOnCommand { get { return new DelegateCommand(SendM03SpindelOn, CanSendCommand); } }
		public ICommand SendM05SpindelOffCommand { get { return new DelegateCommand(SendM05SpindelOff, CanSendCommand); } }
		public ICommand SendM07CoolandOnCommand { get { return new DelegateCommand(SendM07CoolandOn, CanSendCommand); } }
		public ICommand SendM09CoolandOffCommand { get { return new DelegateCommand(SendM09CoolandOff, CanSendCommand); } }
		public ICommand SendM20FileCommand { get { return new DelegateCommand(SendM20File, CanSendCommand); } }
		public ICommand SendM24FileCommand { get { return new DelegateCommand(SendM24File, CanSendFileNameAndSDFileNameCommand); } }
		public ICommand SendM28FileCommand { get { return new DelegateCommand(SendM28File, CanSendFileNameCommand); } }
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
