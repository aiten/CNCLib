using Framework.Logic;
using GCode.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCode.GUI
{
    public partial class MainForm : Form
    {
		private ArduinoSerialCommunication Com
        {
			get { return Framework.Tools.Singleton<ArduinoSerialCommunication>.Instance; }
        }

        public MainForm()
        {
            InitializeComponent();
            _com.SelectedItem = "COM8";
            UpdateButtons();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
			Framework.Tools.Singleton<ArduinoSerialCommunication>.Free();
        }

        private void UpdateButtons()
        {
			if (Framework.Tools.Singleton<ArduinoSerialCommunication>.Allocated && Com.IsConnected)
            {
                _paint.Enabled = true;
            }
            else
            {
//               _paint.Enabled = false;
            }
        }

        private void _connect_Click(object sender, EventArgs e)
        {
            if (Com.IsConnected)
            {
				Framework.Tools.Singleton<ArduinoSerialCommunication>.Free();
            }
            else 
            {
				Com.ResetOnConnect = _reset.Checked;
				Com.Connect(_com.SelectedItem.ToString());
            }
            UpdateButtons();
        }

        private void _paint_Click(object sender, EventArgs e)
        {
            using (PaintForm form = new PaintForm())
            {
                form.ShowDialog();
            }
        }
    }
}
