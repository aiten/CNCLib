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

namespace CNCLib.GCode.Generate.Commands
{
    using System.Collections.Generic;

    public class CommandList : List<Command>
    {
        #region Properties

        public Command Current { get; set; }

        #endregion

        #region Add/Update

        public void AddCommand(Command cmd)
        {
            if (Count > 0)
            {
                this[Count - 1].NextCommand = cmd;
                cmd.PrevCommand             = this[Count - 1];
            }

            cmd.NextCommand = null;
            base.Add(cmd);
        }

        public new void Add(Command cmd)
        {
            AddCommand(cmd);
        }

        public void AddCommands(IEnumerable<Command> cmds)
        {
            foreach (Command cmd in cmds)
            {
                AddCommand(cmd);
            }
        }

        public void UpdateCache()
        {
            var state = new CommandState();
            foreach (Command cmd in this)
            {
                cmd.SetCommandState(state);
                cmd.UpdateCalculatedEndPosition(state);
            }
        }

        #endregion

        #region Paint + Convert

        public void Paint(IOutputCommand output, object param)
        {
            var  commandState    = new CommandState();
            bool haveSeenCurrent = Current == null;

            foreach (Command cmd in this)
            {
                if (!haveSeenCurrent)
                {
                    haveSeenCurrent = cmd == Current;
                }

                commandState.IsSelected = haveSeenCurrent;
                cmd.SetCommandState(commandState);
                cmd.Draw(output, commandState, param);
            }
        }

        public IEnumerable<string> ToStringList()
        {
            var list = new List<string>();

            Command last  = null;
            var     state = new CommandState();

            foreach (Command r in this)
            {
                string[] cmds = r.GetGCodeCommands(last?.CalculatedEndPosition, state);
                if (cmds != null)
                {
                    foreach (string str in cmds)
                    {
                        list.Add(str);
                    }
                }

                last = r;
            }

            return list;
        }

        public CommandList Convert(ConvertOptions options)
        {
            var list  = new CommandList();
            var state = new CommandState();

            foreach (Command r in this)
            {
                r.SetCommandState(state);
                list.AddCommands(r.ConvertCommand(state, options));
            }

            return list;
        }

        #endregion
    }
}