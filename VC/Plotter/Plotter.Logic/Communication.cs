using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.IO.Ports;
using Framework.Logic;

namespace Plotter.Logic
{
    public class Communication : Framework.Logic.HPGLCommunication
    {
        public Communication()
        {
        }

        public void SendFile(string filename,bool singleStep)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                Abort = false;
                String line;
                List<String> lines = new List<string>();
                while ((line = sr.ReadLine()) != null && !Abort)
                {
                    lines.Add(line);
                }          
                SendCommands(lines.ToArray(),true);
            }
        }
    }
}
