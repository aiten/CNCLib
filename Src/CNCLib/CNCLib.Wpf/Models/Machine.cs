////////////////////////////////////////////////////////
/*
  This file is part of CNCLib - A library for stepper motors.

  Copyright (c) 2013-2018 Herbert Aitenbichler

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

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CNCLib.Wpf.Models
{
    public class Machine
    {
        const string CATEGORY_INTERNAL      = "Internal";
        const string CATEGORY_COMMUNICATION = "Communication";
        const string CATEGORY_SIZE          = "Size";
        const string CATEGORY_FEATURES      = "Features";
        const string CATEGORY_PROBE         = "Probe";
        const string CATEGORY_GENERAL       = "General";
        const string CATEGORY_BEHAVIOR      = "Behavior";

        [ReadOnly(false)]
        [Browsable(false)]
        [Category(CATEGORY_INTERNAL)]
        [DisplayName("Machine ID")]
        [Description("Internal Id of machine")]
        public int MachineID { get; set; }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("SerialServer")]
        [Description(
            "Name of the CNCLib.Serial.Server, e.g. IP-Address, localhost or url - if empty, local port is used")]
        public string SerialServer { get; set; }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("SerialServerPort")]
        [Description("Port of the CNCLib.Serial.Server, default is 5000")]
        public int SerialServerPort { get; set; }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("ComPort")]
        [Description("Com of attached arduino")]
        public string ComPort { get; set; }

        public string GetComPort()
        {
            string comport = ComPort;

            if (string.IsNullOrEmpty(SerialServer) == false)
            {
                if (comport.StartsWith(@"/")) // linux: /dev/tty
                {
                    comport = comport.Substring(1);
                }

                comport = $@"http://{SerialServer}:{SerialServerPort}/{comport}";
            }

            return comport;
        }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("BaudRate")]
        [Description("BaudRate")]
        public int BaudRate { get; set; }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("DtrIsReset")]
        [Description(
            "For Arduino Uno, Mega, ... Dtr cause a reset when connecting. For a Arduino zero Dtr must be set/used to transfer data (no reset)")]
        public bool DtrIsReset { get; set; }

        [Category(CATEGORY_SIZE)]
        [DisplayName("Axis")]
        [Description("Active axis count")]
        public int Axis { get; set; }

        [Category(CATEGORY_GENERAL)]
        [DisplayName("Name")]
        [Description("Name of the machine")]
        public string Name { get; set; }

        [Category(CATEGORY_SIZE)]
        [DisplayName("Size-X")]
        [Description("Size of the X axis (in mm)")]
        public decimal SizeX { get; set; }

        [Category(CATEGORY_SIZE)]
        [DisplayName("Size-Y")]
        [Description("Size of the Y axis (in mm)")]
        public decimal SizeY { get; set; }

        [Category(CATEGORY_SIZE)]
        [DisplayName("Size-Z")]
        [Description("Size of the Z axis (in mm)")]
        public decimal SizeZ { get; set; }

        [Category(CATEGORY_SIZE)]
        [DisplayName("Size-A")]
        [Description("Size of the A axis (in mm/grad)")]
        public decimal SizeA { get; set; }

        [Category(CATEGORY_SIZE)]
        [DisplayName("Size-B")]
        [Description("Size of the B axis (in mm/grad)")]
        public decimal SizeB { get; set; }

        [Category(CATEGORY_SIZE)]
        [DisplayName("Size-C")]
        [Description("Size of the C axis (in mm/grad)")]
        public decimal SizeC { get; set; }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("BufferSize")]
        [Description("Size of arduino stream input buffer, usual 63 bytes")]
        public int BufferSize { get; set; }

        [Category(CATEGORY_COMMUNICATION)]
        [DisplayName("CommandToUpper")]
        [Description("Convert all commands to uppercase before sending to machine")]
        public bool CommandToUpper { get; set; }

        [Category(CATEGORY_PROBE)]
        [DisplayName("ProbeSize-X")]
        [Description("Distance between probe hit an 0 (the size of the probe-tool)")]
        public decimal ProbeSizeX { get; set; }

        [Category(CATEGORY_PROBE)]
        [DisplayName("ProbeSize-Y")]
        [Description("Distance between probe hit an 0 (the size of the probe-tool)")]
        public decimal ProbeSizeY { get; set; }

        [Category(CATEGORY_PROBE)]
        [DisplayName("ProbeSize-Z")]
        [Description("Distance between probe hit an 0 (the size of the probe-tool)")]
        public decimal ProbeSizeZ { get; set; }

        [Category(CATEGORY_PROBE)]
        [DisplayName("ProbeDistUp")]
        [Description("Distance to retract when probe is hit")]
        public decimal ProbeDistUp { get; set; }

        [Category(CATEGORY_PROBE)]
        [DisplayName("ProbeDist")]
        [Description("Distance to move untile probe must hit")]
        public decimal ProbeDist { get; set; }

        [Category(CATEGORY_PROBE)]
        [DisplayName("ProbeFeed")]
        [Description("Feedrate while probe is active")]
        public decimal ProbeFeed { get; set; }

        [Category(CATEGORY_FEATURES)]
        [DisplayName("SDSupport")]
        [Description("Machine has a SD card")]
        public bool SDSupport { get; set; }

        [Category(CATEGORY_FEATURES)]
        [DisplayName("Spindle")]
        [Description("Can switch on/of the spindle")]
        public bool Spindle { get; set; }

        [Category(CATEGORY_FEATURES)]
        [DisplayName("Coolant")]
        [Description("Can switch on/of the coolant")]
        public bool Coolant { get; set; }

        [Category(CATEGORY_FEATURES)]
        [DisplayName("Laser")]
        [Description("This is a laser")]
        public bool Laser { get; set; }

        [Category(CATEGORY_FEATURES)]
        [DisplayName("Rotate")]
        [Description("Machine can rotate the coordinate system")]
        public bool Rotate { get; set; }

        [Category(CATEGORY_FEATURES)]
        [DisplayName("CommandSyntax")]
        [Description("Syntax of machine commands, e.g. GCode, HPGL")]
        public Logic.Contracts.DTO.CommandSyntax CommandSyntax { get; set; }

        private ObservableCollection<MachineCommand> _MachineCommands;

        [Browsable(false)]
        public ObservableCollection<MachineCommand> MachineCommands
        {
            get
            {
                if (_MachineCommands == null)
                {
                    _MachineCommands = new ObservableCollection<MachineCommand>();
                    _MachineCommands.CollectionChanged += ((sender, e) =>
                    {
                        if (e.NewItems != null)
                        {
                            foreach (MachineCommand item in e.NewItems)
                            {
                                item.MachineID = MachineID;
                            }
                        }
                    });
                }

                return _MachineCommands;
            }
        }

        private ObservableCollection<MachineInitCommand> _MachineInitCommands;

        [Browsable(false)]
        public ObservableCollection<MachineInitCommand> MachineInitCommands
        {
            get
            {
                if (_MachineInitCommands == null)
                {
                    _MachineInitCommands = new ObservableCollection<MachineInitCommand>();
                    _MachineInitCommands.CollectionChanged += ((sender, e) =>
                    {
                        if (e.NewItems != null)
                        {
                            foreach (MachineInitCommand item in e.NewItems)
                            {
                                item.MachineID = MachineID;
                            }
                        }
                    });
                }

                return _MachineInitCommands;
            }
        }
    }
}