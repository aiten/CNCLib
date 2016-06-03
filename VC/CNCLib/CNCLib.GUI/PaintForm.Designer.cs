namespace CNCLib.GUI
{
    partial class PaintForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this._headerPanel = new System.Windows.Forms.Panel();
			this._laserColor = new CNCLib.GUI.ColorComboBox();
			this._zoomLbl = new System.Windows.Forms.Label();
			this._zoom = new System.Windows.Forms.TextBox();
			this._zoomIn = new System.Windows.Forms.Button();
			this._zoomOut = new System.Windows.Forms.Button();
			this._colorCB = new CNCLib.GUI.ColorComboBox();
			this._ofsXLbl = new System.Windows.Forms.Label();
			this._offsetX = new System.Windows.Forms.TextBox();
			this._ofsXPlus = new System.Windows.Forms.Button();
			this._ofsXMin = new System.Windows.Forms.Button();
			this._ofsYLbl = new System.Windows.Forms.Label();
			this._offsetY = new System.Windows.Forms.TextBox();
			this._ofsYPlus = new System.Windows.Forms.Button();
			this._ofsYMin = new System.Windows.Forms.Button();
			this._laserLbL = new System.Windows.Forms.Label();
			this._laserSize = new System.Windows.Forms.TextBox();
			this._cutterLbl = new System.Windows.Forms.Label();
			this._cutterSize = new System.Windows.Forms.TextBox();
			this._redraw = new System.Windows.Forms.Button();
			this._coord = new System.Windows.Forms.Label();
			this._load = new System.Windows.Forms.Button();
			this._sendTo = new System.Windows.Forms.Button();
			this._mainPanel = new System.Windows.Forms.Panel();
			this._gCodeCtrl = new CNCLib.GUI.GCodeUserControl();
			this._useAzure = new System.Windows.Forms.CheckBox();
			this._headerPanel.SuspendLayout();
			this._mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _headerPanel
			// 
			this._headerPanel.Controls.Add(this._useAzure);
			this._headerPanel.Controls.Add(this._laserColor);
			this._headerPanel.Controls.Add(this._zoomLbl);
			this._headerPanel.Controls.Add(this._zoom);
			this._headerPanel.Controls.Add(this._zoomIn);
			this._headerPanel.Controls.Add(this._zoomOut);
			this._headerPanel.Controls.Add(this._colorCB);
			this._headerPanel.Controls.Add(this._ofsXLbl);
			this._headerPanel.Controls.Add(this._offsetX);
			this._headerPanel.Controls.Add(this._ofsXPlus);
			this._headerPanel.Controls.Add(this._ofsXMin);
			this._headerPanel.Controls.Add(this._ofsYLbl);
			this._headerPanel.Controls.Add(this._offsetY);
			this._headerPanel.Controls.Add(this._ofsYPlus);
			this._headerPanel.Controls.Add(this._ofsYMin);
			this._headerPanel.Controls.Add(this._laserLbL);
			this._headerPanel.Controls.Add(this._laserSize);
			this._headerPanel.Controls.Add(this._cutterLbl);
			this._headerPanel.Controls.Add(this._cutterSize);
			this._headerPanel.Controls.Add(this._redraw);
			this._headerPanel.Controls.Add(this._coord);
			this._headerPanel.Controls.Add(this._load);
			this._headerPanel.Controls.Add(this._sendTo);
			this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._headerPanel.Location = new System.Drawing.Point(0, 0);
			this._headerPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._headerPanel.Name = "_headerPanel";
			this._headerPanel.Size = new System.Drawing.Size(1568, 113);
			this._headerPanel.TabIndex = 0;
			// 
			// _laserColor
			// 
			this._laserColor.Color = System.Drawing.Color.Black;
			this._laserColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._laserColor.DropDownHeight = 400;
			this._laserColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._laserColor.DropDownWidth = 200;
			this._laserColor.FormattingEnabled = true;
			this._laserColor.IntegralHeight = false;
			this._laserColor.Location = new System.Drawing.Point(1002, 17);
			this._laserColor.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._laserColor.MaxDropDownItems = 20;
			this._laserColor.Name = "_laserColor";
			this._laserColor.Size = new System.Drawing.Size(264, 32);
			this._laserColor.TabIndex = 36;
			this._laserColor.SelectedIndexChanged += new System.EventHandler(this._laserColor_SelectedIndexChanged);
			// 
			// _zoomLbl
			// 
			this._zoomLbl.AutoSize = true;
			this._zoomLbl.Location = new System.Drawing.Point(210, 21);
			this._zoomLbl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._zoomLbl.Name = "_zoomLbl";
			this._zoomLbl.Size = new System.Drawing.Size(66, 25);
			this._zoomLbl.TabIndex = 22;
			this._zoomLbl.Text = "Zoom";
			// 
			// _zoom
			// 
			this._zoom.Location = new System.Drawing.Point(288, 13);
			this._zoom.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._zoom.Name = "_zoom";
			this._zoom.Size = new System.Drawing.Size(80, 31);
			this._zoom.TabIndex = 19;
			this._zoom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._zoom_KeyPress);
			// 
			// _zoomIn
			// 
			this._zoomIn.Location = new System.Drawing.Point(404, 13);
			this._zoomIn.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._zoomIn.Name = "_zoomIn";
			this._zoomIn.Size = new System.Drawing.Size(30, 38);
			this._zoomIn.TabIndex = 26;
			this._zoomIn.Text = "+";
			this._zoomIn.UseVisualStyleBackColor = true;
			this._zoomIn.Click += new System.EventHandler(this._zoomIn_Click);
			// 
			// _zoomOut
			// 
			this._zoomOut.Location = new System.Drawing.Point(374, 13);
			this._zoomOut.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._zoomOut.Name = "_zoomOut";
			this._zoomOut.Size = new System.Drawing.Size(30, 38);
			this._zoomOut.TabIndex = 25;
			this._zoomOut.Text = "-";
			this._zoomOut.UseVisualStyleBackColor = true;
			this._zoomOut.Click += new System.EventHandler(this._zoomOut_Click);
			// 
			// _colorCB
			// 
			this._colorCB.Color = System.Drawing.Color.Black;
			this._colorCB.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this._colorCB.DropDownHeight = 400;
			this._colorCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._colorCB.DropDownWidth = 200;
			this._colorCB.FormattingEnabled = true;
			this._colorCB.IntegralHeight = false;
			this._colorCB.Location = new System.Drawing.Point(288, 56);
			this._colorCB.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._colorCB.MaxDropDownItems = 20;
			this._colorCB.Name = "_colorCB";
			this._colorCB.Size = new System.Drawing.Size(264, 32);
			this._colorCB.TabIndex = 31;
			this._colorCB.SelectedIndexChanged += new System.EventHandler(this.colorComboBox1_SelectedIndexChanged);
			// 
			// _ofsXLbl
			// 
			this._ofsXLbl.AutoSize = true;
			this._ofsXLbl.Location = new System.Drawing.Point(578, 23);
			this._ofsXLbl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._ofsXLbl.Name = "_ofsXLbl";
			this._ofsXLbl.Size = new System.Drawing.Size(65, 25);
			this._ofsXLbl.TabIndex = 23;
			this._ofsXLbl.Text = "Ofs X";
			// 
			// _offsetX
			// 
			this._offsetX.Location = new System.Drawing.Point(658, 15);
			this._offsetX.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._offsetX.Name = "_offsetX";
			this._offsetX.Size = new System.Drawing.Size(80, 31);
			this._offsetX.TabIndex = 20;
			this._offsetX.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._offsetX_KeyPress);
			// 
			// _ofsXPlus
			// 
			this._ofsXPlus.Location = new System.Drawing.Point(778, 13);
			this._ofsXPlus.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._ofsXPlus.Name = "_ofsXPlus";
			this._ofsXPlus.Size = new System.Drawing.Size(30, 38);
			this._ofsXPlus.TabIndex = 28;
			this._ofsXPlus.Text = "+";
			this._ofsXPlus.UseVisualStyleBackColor = true;
			this._ofsXPlus.Click += new System.EventHandler(this._ofsXPlus_Click);
			// 
			// _ofsXMin
			// 
			this._ofsXMin.Location = new System.Drawing.Point(746, 13);
			this._ofsXMin.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._ofsXMin.Name = "_ofsXMin";
			this._ofsXMin.Size = new System.Drawing.Size(30, 38);
			this._ofsXMin.TabIndex = 27;
			this._ofsXMin.Text = "-";
			this._ofsXMin.UseVisualStyleBackColor = true;
			this._ofsXMin.Click += new System.EventHandler(this._ofsXMin_Click);
			// 
			// _ofsYLbl
			// 
			this._ofsYLbl.AutoSize = true;
			this._ofsYLbl.Location = new System.Drawing.Point(580, 62);
			this._ofsYLbl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._ofsYLbl.Name = "_ofsYLbl";
			this._ofsYLbl.Size = new System.Drawing.Size(66, 25);
			this._ofsYLbl.TabIndex = 24;
			this._ofsYLbl.Text = "Ofs Y";
			// 
			// _offsetY
			// 
			this._offsetY.Location = new System.Drawing.Point(658, 54);
			this._offsetY.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._offsetY.Name = "_offsetY";
			this._offsetY.Size = new System.Drawing.Size(80, 31);
			this._offsetY.TabIndex = 21;
			this._offsetY.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._offsetY_KeyPress);
			// 
			// _ofsYPlus
			// 
			this._ofsYPlus.Location = new System.Drawing.Point(778, 54);
			this._ofsYPlus.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._ofsYPlus.Name = "_ofsYPlus";
			this._ofsYPlus.Size = new System.Drawing.Size(30, 38);
			this._ofsYPlus.TabIndex = 30;
			this._ofsYPlus.Text = "+";
			this._ofsYPlus.UseVisualStyleBackColor = true;
			this._ofsYPlus.Click += new System.EventHandler(this._ofsYPlus_Click);
			// 
			// _ofsYMin
			// 
			this._ofsYMin.Location = new System.Drawing.Point(746, 54);
			this._ofsYMin.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._ofsYMin.Name = "_ofsYMin";
			this._ofsYMin.Size = new System.Drawing.Size(30, 38);
			this._ofsYMin.TabIndex = 29;
			this._ofsYMin.Text = "-";
			this._ofsYMin.UseVisualStyleBackColor = true;
			this._ofsYMin.Click += new System.EventHandler(this._ofsYMin_Click);
			// 
			// _laserLbL
			// 
			this._laserLbL.AutoSize = true;
			this._laserLbL.Location = new System.Drawing.Point(828, 23);
			this._laserLbL.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._laserLbL.Name = "_laserLbL";
			this._laserLbL.Size = new System.Drawing.Size(66, 25);
			this._laserLbL.TabIndex = 35;
			this._laserLbL.Text = "Laser";
			// 
			// _laserSize
			// 
			this._laserSize.Location = new System.Drawing.Point(906, 15);
			this._laserSize.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._laserSize.Name = "_laserSize";
			this._laserSize.Size = new System.Drawing.Size(80, 31);
			this._laserSize.TabIndex = 34;
			this._laserSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._laserSize_KeyPress);
			// 
			// _cutterLbl
			// 
			this._cutterLbl.AutoSize = true;
			this._cutterLbl.Location = new System.Drawing.Point(828, 62);
			this._cutterLbl.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._cutterLbl.Name = "_cutterLbl";
			this._cutterLbl.Size = new System.Drawing.Size(70, 25);
			this._cutterLbl.TabIndex = 33;
			this._cutterLbl.Text = "Cutter";
			// 
			// _cutterSize
			// 
			this._cutterSize.Location = new System.Drawing.Point(906, 54);
			this._cutterSize.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._cutterSize.Name = "_cutterSize";
			this._cutterSize.Size = new System.Drawing.Size(80, 31);
			this._cutterSize.TabIndex = 32;
			this._cutterSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._cutterSize_KeyPress);
			// 
			// _redraw
			// 
			this._redraw.Location = new System.Drawing.Point(448, 10);
			this._redraw.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._redraw.Name = "_redraw";
			this._redraw.Size = new System.Drawing.Size(108, 44);
			this._redraw.TabIndex = 20;
			this._redraw.Text = "Redraw";
			this._redraw.UseVisualStyleBackColor = true;
			this._redraw.Click += new System.EventHandler(this._redraw_Click);
			// 
			// _coord
			// 
			this._coord.AutoSize = true;
			this._coord.Location = new System.Drawing.Point(24, 75);
			this._coord.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this._coord.Name = "_coord";
			this._coord.Size = new System.Drawing.Size(42, 25);
			this._coord.TabIndex = 14;
			this._coord.Text = "0:0";
			// 
			// _load
			// 
			this._load.Location = new System.Drawing.Point(1376, 62);
			this._load.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._load.Name = "_load";
			this._load.Size = new System.Drawing.Size(168, 44);
			this._load.TabIndex = 3;
			this._load.Text = "Load";
			this._load.UseVisualStyleBackColor = true;
			this._load.Click += new System.EventHandler(this._load_Click);
			// 
			// _sendTo
			// 
			this._sendTo.Location = new System.Drawing.Point(12, 12);
			this._sendTo.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._sendTo.Name = "_sendTo";
			this._sendTo.Size = new System.Drawing.Size(150, 44);
			this._sendTo.TabIndex = 0;
			this._sendTo.Text = "Send To ...";
			this._sendTo.UseVisualStyleBackColor = true;
			this._sendTo.Click += new System.EventHandler(this._sendTo_Click);
			// 
			// _mainPanel
			// 
			this._mainPanel.Controls.Add(this._gCodeCtrl);
			this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mainPanel.Location = new System.Drawing.Point(0, 113);
			this._mainPanel.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this._mainPanel.Name = "_mainPanel";
			this._mainPanel.Size = new System.Drawing.Size(1568, 966);
			this._mainPanel.TabIndex = 2;
			// 
			// _gCodeCtrl
			// 
			this._gCodeCtrl.AutoScroll = true;
			this._gCodeCtrl.CutterSize = 0D;
			this._gCodeCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._gCodeCtrl.KeepRatio = true;
			this._gCodeCtrl.LaserColor = System.Drawing.Color.Red;
			this._gCodeCtrl.LaserSize = 0.254D;
			this._gCodeCtrl.Location = new System.Drawing.Point(0, 0);
			this._gCodeCtrl.MachineColor = System.Drawing.Color.Black;
			this._gCodeCtrl.Margin = new System.Windows.Forms.Padding(12, 12, 12, 12);
			this._gCodeCtrl.Name = "_gCodeCtrl";
			this._gCodeCtrl.OffsetX = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._gCodeCtrl.OffsetY = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._gCodeCtrl.Size = new System.Drawing.Size(1568, 966);
			this._gCodeCtrl.SizeX = new decimal(new int[] {
            130000,
            0,
            0,
            196608});
			this._gCodeCtrl.SizeY = new decimal(new int[] {
            45000,
            0,
            0,
            196608});
			this._gCodeCtrl.TabIndex = 1;
			this._gCodeCtrl.Zoom = 1D;
			this._gCodeCtrl.GCodeMousePosition += new CNCLib.GUI.GCodeUserControl.GCodeEventHandler(this._plotterCtrl_GCodeMousePosition);
			this._gCodeCtrl.ZoomOffsetChanged += new CNCLib.GUI.GCodeUserControl.GCodeEventHandler(this._gCodeCtrl_ZoomOffsetChanged);
			// 
			// _useAzure
			// 
			this._useAzure.AutoSize = true;
			this._useAzure.Location = new System.Drawing.Point(1376, 21);
			this._useAzure.Name = "_useAzure";
			this._useAzure.Size = new System.Drawing.Size(141, 29);
			this._useAzure.TabIndex = 37;
			this._useAzure.Text = "use Azure";
			this._useAzure.UseVisualStyleBackColor = true;
			// 
			// PaintForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1568, 1079);
			this.Controls.Add(this._mainPanel);
			this.Controls.Add(this._headerPanel);
			this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
			this.Name = "PaintForm";
			this.Text = "PaintForm";
			this.Load += new System.EventHandler(this.PaintForm_Load);
			this._headerPanel.ResumeLayout(false);
			this._headerPanel.PerformLayout();
			this._mainPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private GCodeUserControl _gCodeCtrl;
        private System.Windows.Forms.Panel _headerPanel;
        private System.Windows.Forms.Panel _mainPanel;
		private System.Windows.Forms.Button _sendTo;
        private System.Windows.Forms.Button _load;
        private System.Windows.Forms.Label _coord;
		private System.Windows.Forms.TextBox _zoom;
		private System.Windows.Forms.Button _redraw;
		private System.Windows.Forms.TextBox _offsetY;
		private System.Windows.Forms.TextBox _offsetX;
        private System.Windows.Forms.Label _ofsYLbl;
        private System.Windows.Forms.Label _ofsXLbl;
        private System.Windows.Forms.Label _zoomLbl;
        private System.Windows.Forms.Button _zoomOut;
        private System.Windows.Forms.Button _zoomIn;
        private System.Windows.Forms.Button _ofsXPlus;
        private System.Windows.Forms.Button _ofsXMin;
        private System.Windows.Forms.Button _ofsYPlus;
        private System.Windows.Forms.Button _ofsYMin;
		private ColorComboBox _colorCB;
        private System.Windows.Forms.TextBox _cutterSize;
        private System.Windows.Forms.Label _cutterLbl;
        private System.Windows.Forms.TextBox _laserSize;
        private System.Windows.Forms.Label _laserLbL;
        private ColorComboBox _laserColor;
		private System.Windows.Forms.CheckBox _useAzure;
	}
}