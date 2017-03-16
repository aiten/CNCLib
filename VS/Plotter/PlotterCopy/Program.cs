using System;
using System.IO.Ports;
using System.Threading;
using System.IO;
using Plotter.BL;

namespace PlotterCopy
{
    class Program
    {
        //static bool _continue;
        //static SerialPort _serialPort;
        //static bool _replyreceived;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: PlotterCoppy filename [COMx]");
                return;
            }

            string com = "COM6";

            if (args.Length < 2)
                com = args[1];

            Communication communication = new Communication();
            communication.CommandSend += new Communication.CommandEventHandler(CommandSend);
            communication.CommandError += new Communication.CommandEventHandler(CommandError);
            communication.SendFile(args[0],com);
        }

        static string _lastcmd;
        static void CommandSend(object communication, CommunicationEventArgs arg)
        {
            _lastcmd = arg.Info;
        }

        static void CommandError(object communication, CommunicationEventArgs arg)
        {
            Console.WriteLine(_lastcmd + ": " + arg.Info);
        }
    }
}
