namespace GCode.GUI
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
			this._abort = new System.Windows.Forms.Button();
			this._redraw = new System.Windows.Forms.Button();
			this._offsetY = new System.Windows.Forms.TextBox();
			this._offsetX = new System.Windows.Forms.TextBox();
			this._zoom = new System.Windows.Forms.TextBox();
			this._paintcount = new System.Windows.Forms.TextBox();
			this._paintSelected = new System.Windows.Forms.Button();
			this._paintfrom = new System.Windows.Forms.Button();
			this._selidxlbl = new System.Windows.Forms.Label();
			this._coord = new System.Windows.Forms.Label();
			this._selLast = new System.Windows.Forms.Button();
			this._selFirst = new System.Windows.Forms.Button();
			this._selPrev = new System.Windows.Forms.Button();
			this._selNext = new System.Windows.Forms.Button();
			this._home = new System.Windows.Forms.Button();
			this._load = new System.Windows.Forms.Button();
			this._save = new System.Windows.Forms.Button();
			this._sendTo = new System.Windows.Forms.Button();
			this._mainPanel = new System.Windows.Forms.Panel();
			this._plotterCtrl = new GCode.GUI.GCodeUserControl();
			this._headerPanel.SuspendLayout();
			this._mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _headerPanel
			// 
			this._headerPanel.Controls.Add(this._abort);
			this._headerPanel.Controls.Add(this._redraw);
			this._headerPanel.Controls.Add(this._offsetY);
			this._headerPanel.Controls.Add(this._offsetX);
			this._headerPanel.Controls.Add(this._zoom);
			this._headerPanel.Controls.Add(this._paintcount);
			this._headerPanel.Controls.Add(this._paintSelected);
			this._headerPanel.Controls.Add(this._paintfrom);
			this._headerPanel.Controls.Add(this._selidxlbl);
			this._headerPanel.Controls.Add(this._coord);
			this._headerPanel.Controls.Add(this._selLast);
			this._headerPanel.Controls.Add(this._selFirst);
			this._headerPanel.Controls.Add(this._selPrev);
			this._headerPanel.Controls.Add(this._selNext);
			this._headerPanel.Controls.Add(this._home);
			this._headerPanel.Controls.Add(this._load);
			this._headerPanel.Controls.Add(this._save);
			this._headerPanel.Controls.Add(this._sendTo);
			this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._headerPanel.Location = new System.Drawing.Point(0, 0);
			this._headerPanel.Name = "_headerPanel";
			this._headerPanel.Size = new System.Drawing.Size(687, 89);
			this._headerPanel.TabIndex = 0;
			// 
			// _abort
			// 
			this._abort.Enabled = false;
			this._abort.Location = new System.Drawing.Point(6, 59);
			this._abort.Name = "_abort";
			this._abort.Size = new System.Drawing.Size(75, 23);
			this._abort.TabIndex = 22;
			this._abort.Text = "Abort";
			this._abort.UseVisualStyleBackColor = true;
			this._abort.Click += new System.EventHandler(this._abort_Click);
			// 
			// _redraw
			// 
			this._redraw.Location = new System.Drawing.Point(502, 4);
			this._redraw.Name = "_redraw";
			this._redraw.Size = new System.Drawing.Size(75, 23);
			this._redraw.TabIndex = 20;
			this._redraw.Text = "Redraw";
			this._redraw.UseVisualStyleBackColor = true;
			this._redraw.Click += new System.EventHandler(this._redraw_Click);
			// 
			// _offsetY
			// 
			this._offsetY.Location = new System.Drawing.Point(454, 6);
			this._offsetY.Name = "_offsetY";
			this._offsetY.Size = new System.Drawing.Size(42, 20);
			this._offsetY.TabIndex = 21;
			this._offsetY.Text = "0";
			// 
			// _offsetX
			// 
			this._offsetX.Location = new System.Drawing.Point(406, 6);
			this._offsetX.Name = "_offsetX";
			this._offsetX.Size = new System.Drawing.Size(42, 20);
			this._offsetX.TabIndex = 20;
			this._offsetX.Text = "0";
			// 
			// _zoom
			// 
			this._zoom.Location = new System.Drawing.Point(358, 6);
			this._zoom.Name = "_zoom";
			this._zoom.Size = new System.Drawing.Size(42, 20);
			this._zoom.TabIndex = 19;
			this._zoom.Text = "1";
			// 
			// _paintcount
			// 
			this._paintcount.Location = new System.Drawing.Point(235, 31);
			this._paintcount.Name = "_paintcount";
			this._paintcount.Size = new System.Drawing.Size(23, 20);
			this._paintcount.TabIndex = 18;
			this._paintcount.Text = "1";
			// 
			// _paintSelected
			// 
			this._paintSelected.Location = new System.Drawing.Point(261, 29);
			this._paintSelected.Name = "_paintSelected";
			this._paintSelected.Size = new System.Drawing.Size(75, 23);
			this._paintSelected.TabIndex = 17;
			this._paintSelected.Text = "Paint Sel";
			this._paintSelected.UseVisualStyleBackColor = true;
			this._paintSelected.Click += new System.EventHandler(this._paintSelected_Click);
			// 
			// _paintfrom
			// 
			this._paintfrom.Location = new System.Drawing.Point(166, 30);
			this._paintfrom.Name = "_paintfrom";
			this._paintfrom.Size = new System.Drawing.Size(68, 23);
			this._paintfrom.TabIndex = 16;
			this._paintfrom.Text = "Paint From";
			this._paintfrom.UseVisualStyleBackColor = true;
			this._paintfrom.Click += new System.EventHandler(this._paintfrom_Click);
			// 
			// _selidxlbl
			// 
			this._selidxlbl.Location = new System.Drawing.Point(231, 10);
			this._selidxlbl.Name = "_selidxlbl";
			this._selidxlbl.Size = new System.Drawing.Size(31, 18);
			this._selidxlbl.TabIndex = 15;
			this._selidxlbl.Text = "0";
			this._selidxlbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _coord
			// 
			this._coord.AutoSize = true;
			this._coord.Location = new System.Drawing.Point(355, 38);
			this._coord.Name = "_coord";
			this._coord.Size = new System.Drawing.Size(22, 13);
			this._coord.TabIndex = 14;
			this._coord.Text = "0:0";
			// 
			// _selLast
			// 
			this._selLast.Location = new System.Drawing.Point(291, 6);
			this._selLast.Name = "_selLast";
			this._selLast.Size = new System.Drawing.Size(35, 23);
			this._selLast.TabIndex = 12;
			this._selLast.Text = ">>";
			this._selLast.UseVisualStyleBackColor = true;
			this._selLast.Click += new System.EventHandler(this._selLast_Click);
			// 
			// _selFirst
			// 
			this._selFirst.Location = new System.Drawing.Point(167, 6);
			this._selFirst.Name = "_selFirst";
			this._selFirst.Size = new System.Drawing.Size(35, 23);
			this._selFirst.TabIndex = 11;
			this._selFirst.Text = "<<";
			this._selFirst.UseVisualStyleBackColor = true;
			this._selFirst.Click += new System.EventHandler(this._selFirst_Click);
			// 
			// _selPrev
			// 
			this._selPrev.Location = new System.Drawing.Point(204, 6);
			this._selPrev.Name = "_selPrev";
			this._selPrev.Size = new System.Drawing.Size(22, 23);
			this._selPrev.TabIndex = 9;
			this._selPrev.Text = "&<";
			this._selPrev.UseVisualStyleBackColor = true;
			this._selPrev.Click += new System.EventHandler(this._selPrev_Click);
			// 
			// _selNext
			// 
			this._selNext.Location = new System.Drawing.Point(264, 6);
			this._selNext.Name = "_selNext";
			this._selNext.Size = new System.Drawing.Size(25, 23);
			this._selNext.TabIndex = 8;
			this._selNext.Text = "&>";
			this._selNext.UseVisualStyleBackColor = true;
			this._selNext.Click += new System.EventHandler(this._selNext_Click);
			// 
			// _home
			// 
			this._home.Location = new System.Drawing.Point(6, 30);
			this._home.Name = "_home";
			this._home.Size = new System.Drawing.Size(75, 23);
			this._home.TabIndex = 4;
			this._home.Text = "Home";
			this._home.UseVisualStyleBackColor = true;
			this._home.Click += new System.EventHandler(this._home_Click);
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
			this._mainPanel.Controls.Add(this._plotterCtrl);
			this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mainPanel.Location = new System.Drawing.Point(0, 89);
			this._mainPanel.Name = "_mainPanel";
			this._mainPanel.Size = new System.Drawing.Size(687, 421);
			this._mainPanel.TabIndex = 2;
			// 
			// _plotterCtrl
			// 
			this._plotterCtrl.AutoScroll = true;
			this._plotterCtrl.BackColor = System.Drawing.Color.White;
			this._plotterCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._plotterCtrl.Location = new System.Drawing.Point(0, 0);
			this._plotterCtrl.Name = "_plotterCtrl";
			this._plotterCtrl.OffsetX = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._plotterCtrl.OffsetY = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this._plotterCtrl.SelectedColor = System.Drawing.Color.Black;
			this._plotterCtrl.SelectedCommand = -1;
			this._plotterCtrl.SelectedLinesize = 1;
			this._plotterCtrl.Size = new System.Drawing.Size(687, 421);
			this._plotterCtrl.TabIndex = 1;
			this._plotterCtrl.Zoom = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._plotterCtrl.GCodeMousePosition += new GCode.GUI.GCodeUserControl.GCodeEventHandler(this._plotterCtrl_GCodeMousePosition);
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

        private GCodeUserControl _plotterCtrl;
        private System.Windows.Forms.Panel _headerPanel;
        private System.Windows.Forms.Panel _mainPanel;
		private System.Windows.Forms.Button _sendTo;
        private System.Windows.Forms.Button _save;
        private System.Windows.Forms.Button _load;
		private System.Windows.Forms.Button _home;
        private System.Windows.Forms.Button _selPrev;
		private System.Windows.Forms.Button _selNext;
        private System.Windows.Forms.Button _selLast;
        private System.Windows.Forms.Button _selFirst;
        private System.Windows.Forms.Label _coord;
        private System.Windows.Forms.Label _selidxlbl;
        private System.Windows.Forms.Button _paintfrom;
        private System.Windows.Forms.Button _paintSelected;
		private System.Windows.Forms.TextBox _paintcount;
		private System.Windows.Forms.TextBox _zoom;
		private System.Windows.Forms.Button _redraw;
		private System.Windows.Forms.TextBox _offsetY;
		private System.Windows.Forms.TextBox _offsetX;
		private System.Windows.Forms.Button _abort;
    }
}