using CNCLib.Wpf.ViewModels.ManualControl;
using Framework.Arduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CNCLib.Wpf.Helpers
{
    class JoystickArduinoSerialCommunication : ArduinoSerialCommunication
    {
        private Framework.Arduino.ArduinoSerialCommunication Com
        {
            get { return Framework.Tools.Pattern.Singleton<Framework.Arduino.ArduinoSerialCommunication>.Instance; }
        }

        public void AsyncRunCommand(Action todo)
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
                }
            }
            ).Start();
        }

        protected override void OnReplyReceived(ArduinoSerialCommunicationEventArgs info)
        {
            base.OnReplyReceived(info);

            AsyncRunCommand(() => Com.SendCommand(info.Info));
        }
    }
}
