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
            this._offsetX = new System.Windows.Forms.TextBox();
            this._ofsXPlus = new System.Windows.Forms.Button();
            this._ofsXMin = new System.Windows.Forms.Button();
            this._offsetY = new System.Windows.Forms.TextBox();
            this._ofsYPlus = new System.Windows.Forms.Button();
            this._ofsYMin = new System.Windows.Forms.Button();
            this._zoom = new System.Windows.Forms.TextBox();
            this._zoomIn = new System.Windows.Forms.Button();
            this._zoomOut = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._redraw = new System.Windows.Forms.Button();
            this._coord = new System.Windows.Forms.Label();
            this._load = new System.Windows.Forms.Button();
            this._save = new System.Windows.Forms.Button();
            this._sendTo = new System.Windows.Forms.Button();
            this._mainPanel = new System.Windows.Forms.Panel();
            this._gCodeCtrl = new CNCLib.GUI.GCodeUserControl();
            this._headerPanel.SuspendLayout();
            this._mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _headerPanel
            // 
            this._headerPanel.Controls.Add(this._offsetX);
            this._headerPanel.Controls.Add(this._ofsXPlus);
            this._headerPanel.Controls.Add(this._ofsXMin);
            this._headerPanel.Controls.Add(this._offsetY);
            this._headerPanel.Controls.Add(this._ofsYPlus);
            this._headerPanel.Controls.Add(this._ofsYMin);
            this._headerPanel.Controls.Add(this._zoom);
            this._headerPanel.Controls.Add(this._zoomIn);
            this._headerPanel.Controls.Add(this._zoomOut);
            this._headerPanel.Controls.Add(this.label3);
            this._headerPanel.Controls.Add(this.label2);
            this._headerPanel.Controls.Add(this.label1);
            this._headerPanel.Controls.Add(this._redraw);
            this._headerPanel.Controls.Add(this._coord);
            this._headerPanel.Controls.Add(this._load);
            this._headerPanel.Controls.Add(this._save);
            this._headerPanel.Controls.Add(this._sendTo);
            this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerPanel.Location = new System.Drawing.Point(0, 0);
            this._headerPanel.Name = "_headerPanel";
            this._headerPanel.Size = new System.Drawing.Size(687, 59);
            this._headerPanel.TabIndex = 0;
            // 
            // _offsetX
            // 
            this._offsetX.Location = new System.Drawing.Point(366, 8);
            this._offsetX.Name = "_offsetX";
            this._offsetX.Size = new System.Drawing.Size(42, 20);
            this._offsetX.TabIndex = 20;
            this._offsetX.Text = "0";
            // 
            // _ofsXPlus
            // 
            this._ofsXPlus.Location = new System.Drawing.Point(438, 7);
            this._ofsXPlus.Name = "_ofsXPlus";
            this._ofsXPlus.Size = new System.Drawing.Size(24, 20);
            this._ofsXPlus.TabIndex = 28;
            this._ofsXPlus.Text = "+";
            this._ofsXPlus.UseVisualStyleBackColor = true;
            this._ofsXPlus.Click += new System.EventHandler(this._ofsXPlus_Click);
            // 
            // _ofsXMin
            // 
            this._ofsXMin.Location = new System.Drawing.Point(412, 7);
            this._ofsXMin.Name = "_ofsXMin";
            this._ofsXMin.Size = new System.Drawing.Size(24, 20);
            this._ofsXMin.TabIndex = 27;
            this._ofsXMin.Text = "-";
            this._ofsXMin.UseVisualStyleBackColor = true;
            this._ofsXMin.Click += new System.EventHandler(this._ofsXMin_Click);
            // 
            // _offsetY
            // 
            this._offsetY.Location = new System.Drawing.Point(366, 29);
            this._offsetY.Name = "_offsetY";
            this._offsetY.Size = new System.Drawing.Size(42, 20);
            this._offsetY.TabIndex = 21;
            this._offsetY.Text = "0";
            // 
            // _ofsYPlus
            // 
            this._ofsYPlus.Location = new System.Drawing.Point(438, 28);
            this._ofsYPlus.Name = "_ofsYPlus";
            this._ofsYPlus.Size = new System.Drawing.Size(24, 20);
            this._ofsYPlus.TabIndex = 30;
            this._ofsYPlus.Text = "+";
            this._ofsYPlus.UseVisualStyleBackColor = true;
            this._ofsYPlus.Click += new System.EventHandler(this._ofsYPlus_Click);
            // 
            // _ofsYMin
            // 
            this._ofsYMin.Location = new System.Drawing.Point(412, 28);
            this._ofsYMin.Name = "_ofsYMin";
            this._ofsYMin.Size = new System.Drawing.Size(24, 20);
            this._ofsYMin.TabIndex = 29;
            this._ofsYMin.Text = "-";
            this._ofsYMin.UseVisualStyleBackColor = true;
            this._ofsYMin.Click += new System.EventHandler(this._ofsYMin_Click);
            // 
            // _zoom
            // 
            this._zoom.Location = new System.Drawing.Point(164, 7);
            this._zoom.Name = "_zoom";
            this._zoom.Size = new System.Drawing.Size(42, 20);
            this._zoom.TabIndex = 19;
            this._zoom.Text = "1";
            // 
            // _zoomIn
            // 
            this._zoomIn.Location = new System.Drawing.Point(232, 7);
            this._zoomIn.Name = "_zoomIn";
            this._zoomIn.Size = new System.Drawing.Size(24, 20);
            this._zoomIn.TabIndex = 26;
            this._zoomIn.Text = "+";
            this._zoomIn.UseVisualStyleBackColor = true;
            this._zoomIn.Click += new System.EventHandler(this._zoomIn_Click);
            // 
            // _zoomOut
            // 
            this._zoomOut.Location = new System.Drawing.Point(207, 7);
            this._zoomOut.Name = "_zoomOut";
            this._zoomOut.Size = new System.Drawing.Size(24, 20);
            this._zoomOut.TabIndex = 25;
            this._zoomOut.Text = "-";
            this._zoomOut.UseVisualStyleBackColor = true;
            this._zoomOut.Click += new System.EventHandler(this._zoomOut_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(327, 32);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 13);
            this.label3.TabIndex = 24;
            this.label3.Text = "Ofs Y";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(326, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Ofs X";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(125, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Zoom";
            // 
            // _redraw
            // 
            this._redraw.Location = new System.Drawing.Point(498, 3);
            this._redraw.Name = "_redraw";
            this._redraw.Size = new System.Drawing.Size(75, 23);
            this._redraw.TabIndex = 20;
            this._redraw.Text = "Redraw";
            this._redraw.UseVisualStyleBackColor = true;
            this._redraw.Click += new System.EventHandler(this._redraw_Click);
            // 
            // _coord
            // 
            this._coord.AutoSize = true;
            this._coord.Location = new System.Drawing.Point(12, 39);
            this._coord.Name = "_coord";
            this._coord.Size = new System.Drawing.Size(22, 13);
            this._coord.TabIndex = 14;
            this._coord.Text = "0:0";
            // 
            // _load
            // 
            this._load.Location = new System.Drawing.Point(600, 29);
            this._load.Name = "_load";
            this._load.Size = new System.Drawing.Size(84, 23);
            this._load.TabIndex = 3;
            this._load.Text = "Load";
            this._load.UseVisualStyleBackColor = true;
            this._load.Click += new System.EventHandler(this._load_Click);
            // 
            // _save
            // 
            this._save.Location = new System.Drawing.Point(600, 5);
            this._save.Name = "_save";
            this._save.Size = new System.Drawing.Size(84, 23);
            this._save.TabIndex = 2;
            this._save.Text = "Save GCode";
            this._save.UseVisualStyleBackColor = true;
            this._save.Click += new System.EventHandler(this._save_Click);
            // 
            // _sendTo
            // 
            this._sendTo.Location = new System.Drawing.Point(6, 6);
            this._sendTo.Name = "_sendTo";
            this._sendTo.Size = new System.Drawing.Size(75, 23);
            this._sendTo.TabIndex = 0;
            this._sendTo.Text = "Send To ...";
            this._sendTo.UseVisualStyleBackColor = true;
            this._sendTo.Click += new System.EventHandler(this._sendTo_Click);
            // 
            // _mainPanel
            // 
            this._mainPanel.Controls.Add(this._gCodeCtrl);
            this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._mainPanel.Location = new System.Drawing.Point(0, 59);
            this._mainPanel.Name = "_mainPanel";
            this._mainPanel.Size = new System.Drawing.Size(687, 451);
            this._mainPanel.TabIndex = 2;
            // 
            // _gCodeCtrl
            // 
            this._gCodeCtrl.AutoScroll = true;
            this._gCodeCtrl.BackColor = System.Drawing.Color.Black;
            this._gCodeCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gCodeCtrl.Location = new System.Drawing.Point(0, 0);
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
            this._gCodeCtrl.Size = new System.Drawing.Size(687, 451);
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
            this._gCodeCtrl.Zoom = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._gCodeCtrl.GCodeMousePosition += new CNCLib.GUI.GCodeUserControl.GCodeEventHandler(this._plotterCtrl_GCodeMousePosition);
            this._gCodeCtrl.ZoomOffsetChanged += new CNCLib.GUI.GCodeUserControl.GCodeEventHandler(this._gCodeCtrl_ZoomOffsetChanged);
            // 
            // PaintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 510);
            this.Controls.Add(this._mainPanel);
            this.Controls.Add(this._headerPanel);
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
        private System.Windows.Forms.Button _save;
        private System.Windows.Forms.Button _load;
        private System.Windows.Forms.Label _coord;
		private System.Windows.Forms.TextBox _zoom;
		private System.Windows.Forms.Button _redraw;
		private System.Windows.Forms.TextBox _offsetY;
		private System.Windows.Forms.TextBox _offsetX;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _zoomOut;
        private System.Windows.Forms.Button _zoomIn;
        private System.Windows.Forms.Button _ofsXPlus;
        private System.Windows.Forms.Button _ofsXMin;
        private System.Windows.Forms.Button _ofsYPlus;
        private System.Windows.Forms.Button _ofsYMin;
    }
}