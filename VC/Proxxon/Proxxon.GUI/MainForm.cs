using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Roboter.Logic;
using System.Threading;
using Framework.Logic;

namespace Roboter.GUI
{
    public partial class MainForm : Form
    {
        private enum MyEvent
        {
            SendCommand,
            ErrorReceived,
            DoneReceived,
            InfoReceived,
            UnknownReceived
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


        public MainForm()
        {
            InitializeComponent();
            _com.SelectedItem = "COM4";
        }

        private void _fileopen_Click(object sender, EventArgs e)
        {
            _openFileDialog.FileName = _filename.Text;
            if (_openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _filename.Text = _openFileDialog.FileName;
            }
        }

        Communication _communication = null;

        private void _run_Click(object sender, EventArgs e)
        {
            _results.Items.Clear();
            _stepCount = 1;

            _communication = new Communication();
            _communication.SingleStep = _singlestep.Checked;
            string com = _com.SelectedItem.ToString();

            new Thread(() =>
            {
                try
                {
                    _communication.CommandSending += new Communication.CommandEventHandler(CommandSend);
                    _communication.ReplyError += new Communication.CommandEventHandler(CommandError);
                    _communication.ReplyOK += new Communication.CommandEventHandler(CommandDone);
                    _communication.ReplyInfo += new Communication.CommandEventHandler(CommandInfo);
                    _communication.SendFile(_filename.Text);
                }
                catch (Exception ee)
                {
                    this.Invoke(new MethodInvoker(() => MessageBox.Show(ee.Message)));
                }
                finally
                {
                    _communication.Dispose();
                    _communication = null;
                }
            }
            ).Start();
        }
        private void AddToEventList(MyEventParam param)
        {
            lock (_eventlist)
            {
                _eventlist.Add(param);
                if (_guicopyThread.ThreadState == ThreadState.Suspended)
                    _guicopyThread.Resume();
            }
        }

        void CommandSend(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            MyEventParam param = new MyEventParam(MyEvent.SendCommand,arg);
            AddToEventList(param);
         }


        void CommandError(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            MyEventParam param = new MyEventParam(MyEvent.ErrorReceived, arg);
            AddToEventList(param);
        }

        void CommandDone(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            MyEventParam param = new MyEventParam(MyEvent.DoneReceived, arg);
            AddToEventList(param);
        }
        void CommandInfo(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            MyEventParam param = new MyEventParam(MyEvent.InfoReceived, arg);
            AddToEventList(param);
        }
        void CommandUnknown(object communication, ArduinoSerialCommunicationEventArgs arg)
        {
            MyEventParam param = new MyEventParam(MyEvent.UnknownReceived, arg);
            AddToEventList(param);
        }

        private void _abort_Click(object sender, EventArgs e)
        {
            if (_communication != null)
                _communication.Abort = true;
        }

        private void _executesingleStep_Click(object sender, EventArgs e)
        {
            if (_communication != null)
                _communication.SingleStepContinue = true;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_communication == null)
            {
                _filename.Enabled = true;
                _run.Enabled = true;
                _abort.Enabled = false;
                _executesingleStep.Enabled = false;
                _executeRun.Enabled = false;
                _break.Enabled = false;
            }
            else
            {
                _filename.Enabled = false;
                _run.Enabled = false;
                _abort.Enabled = true;
                _executesingleStep.Enabled = _communication.SingleStep;
                _executeRun.Enabled = _communication.SingleStep;
                _break.Enabled = !_communication.SingleStep;
            }
        }

        private void _break_Click(object sender, EventArgs e)
        {
            if (_communication != null)
            {
                _communication.SingleStep = true;
                _communication.SingleStepContinue = false;
            }
        }

        private void _executeRun_Click(object sender, EventArgs e)
        {
            if (_communication != null)
            {
                _communication.SingleStep = false;
                _communication.SingleStepContinue = true;
            }
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
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
                            case MyEvent.SendCommand:
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
                            case MyEvent.ErrorReceived:
                                {
                                    this.Invoke(new MethodInvoker(() => { if (_results.Items[_results.Items.Count - 1].SubItems.Count == ResultCol) _results.Items[_results.Items.Count - 1].SubItems.Add(eventparam._e.Info); else _results.Items[_results.Items.Count - 1].SubItems[ResultCol].Text += " " + eventparam._e.Info; }));
                                    break;
                                }
                            case MyEvent.DoneReceived:
                                {
                                    this.Invoke(new MethodInvoker(() => { if (_results.Items[_results.Items.Count - 1].SubItems.Count == ResultCol) _results.Items[_results.Items.Count - 1].SubItems.Add("done"); else _results.Items[_results.Items.Count - 1].SubItems[ResultCol].Text += " done"; }));
                                    break;
                                }
                            case MyEvent.InfoReceived:
                                {
                                    this.Invoke(new MethodInvoker(() => { if (_results.Items[_results.Items.Count - 1].SubItems.Count == ResultCol) _results.Items[_results.Items.Count - 1].SubItems.Add(eventparam._e.Info); else _results.Items[_results.Items.Count - 1].SubItems[ResultCol].Text += " " + eventparam._e.Info; }));
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
                if (_communication != null)
                {
                    new Thread(() =>
                    {
                        _communication.SendCommand(_sendcommand.Text);;
                    }
                    ).Start();
                }
                else
                {
                    _communication = new Communication();
                    _communication.SingleStep = _singlestep.Checked;
                    string com = _com.SelectedItem.ToString();

                    _communication.SendCommand(_sendcommand.Text);

                    _communication.Dispose();
                    _communication = null;
                }
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
    }
}
