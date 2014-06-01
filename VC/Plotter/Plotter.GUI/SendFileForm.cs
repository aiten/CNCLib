using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Plotter.Logic;
using System.Threading;
using Framework.Logic;

namespace Plotter.GUI
{
    public partial class SendFileForm : Form
    {
        private enum MyEvent
        {
            SendingCommand,
            SentCommand,
            ReplyReceived
        };

        class MyEventParam
        {
            public MyEventParam(MyEvent myevent, ArduinoSerialCommunicationEventArgs e)
            {
                _event = myevent;
                _e = e;
            }
            public MyEvent _event;
            public ArduinoSerialCommunicationEventArgs _e;
        };

        List<MyEventParam> _eventlist = new List<MyEventParam>();
        Thread _guicopyThread = null;

        const int ResultCol = 2;
        int _stepCount = 0;
        bool _debug_continue=true;
        bool _debug = false;
        bool _run = false;

        public SendFileForm()
        {
            InitializeComponent();
        }

        private void _fileopen_Click(object sender, EventArgs e)
        {
            _openFileDialog.FileName = _filename.Text;
            if (_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _filename.Text = _openFileDialog.FileName;
            }
        }

        private Communication Com
        {
            get { return Framework.Tools.Singleton<Communication>.Instance; }
        }

        private void _startFile_Click(object sender, EventArgs e)
        {
            _results.Items.Clear();
            _stepCount = 1;

            bool singleStep = _singlestep.Checked;
            _debug = singleStep;
            _debug_continue = false;

            new Thread(() =>
            {
                try
                {
                    _run = true;
                    Com.SendFile(_filename.Text, true);
                }
                catch (Exception ee)
                {
                    this.Invoke(new MethodInvoker(() => MessageBox.Show(ee.Message)));
                }
                finally
                {
                    _run = false;
                }
            }
            ).Start();
        }

        private void AddEvent(MyEvent ev, ArduinoSerialCommunicationEventArgs arg)
        {
            MyEventParam param = new MyEventParam(ev, arg);
            lock (_eventlist)
            {
                _eventlist.Add(param);
                if (_guicopyThread.ThreadState == ThreadState.Suspended)
                    _guicopyThread.Resume();
            }
        }

        void CommandSending(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            AddEvent(MyEvent.SendingCommand, arg);
            _debug_continue = false;
         }

        void ReplyReceived(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            AddEvent(MyEvent.ReplyReceived, arg);
        }

        void CommandSent(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            
        }

        void AskSingleStepContinue(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            arg.Continue = !_debug  || _debug_continue;
        }

        private void _file_abort_Click(object sender, EventArgs e)
        {
            Com.AbortCommand();
        }

        private void _debug_step_Click(object sender, EventArgs e)
        {
            _debug_continue = true;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {

            _filename.Enabled = !_run;
            _file_start.Enabled = !_run;
            _file_abort.Enabled = _run;
            _commandBtn.Enabled = !_run;

            _debug_step.Enabled = _run && _debug;
            _debug_run.Enabled = _run && _debug;
            _debug_break.Enabled = _run && !_debug;
        }

        private void _break_Click(object sender, EventArgs e)
        {
            _debug = true;
        }

        private void _debug_Run_Click(object sender, EventArgs e)
        {
            _debug = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Com.CommandSending += new Communication.CommandEventHandler(CommandSending);
            Com.ReplyReceived += new Communication.CommandEventHandler(ReplyReceived);
            Com.CommandSent += new Communication.CommandEventHandler(CommandSent);
            Com.CommandWaitSingleStepContinue += new Communication.CommandEventHandler(AskSingleStepContinue);
 
            (_guicopyThread=new Thread(() =>
            {
                while (_eventlist != null)
                {
                    MyEventParam eventparam = null;
                    lock (_eventlist)
                    {
                        if (_eventlist.Count > 0)
                        {
                            eventparam = _eventlist[0];
                            _eventlist.RemoveAt(0);
                        }
                    }
                    if (eventparam == null)
                    {
                        Thread.CurrentThread.Suspend();
                    }
                    else
                    {
                        switch (eventparam._event)
                        {
                            case MyEvent.SendingCommand:
                                {
                                    this.Invoke(new MethodInvoker(() =>
                                    {
                                        _results.Items.Add(_stepCount.ToString());
                                        _results.Items[_results.Items.Count - 1].SubItems.Add(eventparam._e.Info);
                                        _results.EnsureVisible(_results.Items.Count - 1);
                                        _stepCount++;
                                    }
                                    ));
                                    break;
                                }
                            case MyEvent.ReplyReceived:
                                {
                                    this.Invoke(new MethodInvoker(() => { if (_results.Items.Count == 0) _results.Items.Add("");  if (_results.Items[_results.Items.Count - 1].SubItems.Count == ResultCol) _results.Items[_results.Items.Count - 1].SubItems.Add(eventparam._e.Info); else _results.Items[_results.Items.Count - 1].SubItems[ResultCol].Text += " " + eventparam._e.Info; }));
                                    break;
                                }
                            case MyEvent.SentCommand:
                                {
                                    break;
                                }
                        }
                    }
                }
            }
            )).Start();

        }

        private void _commandBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_sendcommand.Text))
            {
                new Thread(() =>
                {
                    Com.SendCommand(_sendcommand.Text);;
                }
                ).Start();
            }
        }

        private void _sendcommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                _commandBtn_Click(this, null);
                _sendcommand.SelectAll();
            }
        }

        private void SendFileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Com.CommandSending -= new Communication.CommandEventHandler(CommandSending);
            Com.ReplyReceived -= new Communication.CommandEventHandler(ReplyReceived);
            Com.CommandSent -= new Communication.CommandEventHandler(CommandSent);
            Com.CommandWaitSingleStepContinue -= new Communication.CommandEventHandler(AskSingleStepContinue);

        }
   }
}
