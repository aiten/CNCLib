namespace Plotter.GUI
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
			this._paintcount = new System.Windows.Forms.TextBox();
			this._paintSelected = new System.Windows.Forms.Button();
			this._paintfrom = new System.Windows.Forms.Button();
			this._selidxlbl = new System.Windows.Forms.Label();
			this._coord = new System.Windows.Forms.Label();
			this._plot = new System.Windows.Forms.CheckBox();
			this._selLast = new System.Windows.Forms.Button();
			this._selFirst = new System.Windows.Forms.Button();
			this._del = new System.Windows.Forms.Button();
			this._selPrev = new System.Windows.Forms.Button();
			this._selNext = new System.Windows.Forms.Button();
			this._undo = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._dot = new System.Windows.Forms.RadioButton();
			this._circle = new System.Windows.Forms.RadioButton();
			this._triangle = new System.Windows.Forms.RadioButton();
			this._line = new System.Windows.Forms.RadioButton();
			this._rectangle = new System.Windows.Forms.RadioButton();
			this._polyLine = new System.Windows.Forms.RadioButton();
			this._home = new System.Windows.Forms.Button();
			this._load = new System.Windows.Forms.Button();
			this._save = new System.Windows.Forms.Button();
			this._clear = new System.Windows.Forms.Button();
			this._paintAgain = new System.Windows.Forms.Button();
			this._mainPanel = new System.Windows.Forms.Panel();
			this._plotterCtrl = new Plotter.GUI.PlotterUserControl();
			this._headerPanel.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this._mainPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _headerPanel
			// 
			this._headerPanel.Controls.Add(this._abort);
			this._headerPanel.Controls.Add(this._paintcount);
			this._headerPanel.Controls.Add(this._paintSelected);
			this._headerPanel.Controls.Add(this._paintfrom);
			this._headerPanel.Controls.Add(this._selidxlbl);
			this._headerPanel.Controls.Add(this._coord);
			this._headerPanel.Controls.Add(this._plot);
			this._headerPanel.Controls.Add(this._selLast);
			this._headerPanel.Controls.Add(this._selFirst);
			this._headerPanel.Controls.Add(this._del);
			this._headerPanel.Controls.Add(this._selPrev);
			this._headerPanel.Controls.Add(this._selNext);
			this._headerPanel.Controls.Add(this._undo);
			this._headerPanel.Controls.Add(this.groupBox1);
			this._headerPanel.Controls.Add(this._home);
			this._headerPanel.Controls.Add(this._load);
			this._headerPanel.Controls.Add(this._save);
			this._headerPanel.Controls.Add(this._clear);
			this._headerPanel.Controls.Add(this._paintAgain);
			this._headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this._headerPanel.Location = new System.Drawing.Point(0, 0);
			this._headerPanel.Name = "_headerPanel";
			this._headerPanel.Size = new System.Drawing.Size(687, 88);
			this._headerPanel.TabIndex = 0;
			// 
			// _abort
			// 
			this._abort.Location = new System.Drawing.Point(600, 57);
			this._abort.Name = "_abort";
			this._abort.Size = new System.Drawing.Size(75, 23);
			this._abort.TabIndex = 19;
			this._abort.Text = "Abort";
			this._abort.UseVisualStyleBackColor = true;
			this._abort.Click += new System.EventHandler(this._abort_Click);
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
			this._coord.Location = new System.Drawing.Point(347, 67);
			this._coord.Name = "_coord";
			this._coord.Size = new System.Drawing.Size(22, 13);
			this._coord.TabIndex = 14;
			this._coord.Text = "0:0";
			// 
			// _plot
			// 
			this._plot.AutoSize = true;
			this._plot.Location = new System.Drawing.Point(87, 56);
			this._plot.Name = "_plot";
			this._plot.Size = new System.Drawing.Size(111, 17);
			this._plot.TabIndex = 13;
			this._plot.Text = "Plot while drawing";
			this._plot.UseVisualStyleBackColor = true;
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
			// _del
			// 
			this._del.Location = new System.Drawing.Point(87, 29);
			this._del.Name = "_del";
			this._del.Size = new System.Drawing.Size(75, 23);
			this._del.TabIndex = 10;
			this._del.Text = "Del";
			this._del.UseVisualStyleBackColor = true;
			this._del.Click += new System.EventHandler(this._del_Click);
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
			// _undo
			// 
			this._undo.Location = new System.Drawing.Point(87, 6);
			this._undo.Name = "_undo";
			this._undo.Size = new System.Drawing.Size(75, 23);
			this._undo.TabIndex = 7;
			this._undo.Text = "Undo";
			this._undo.UseVisualStyleBackColor = true;
			this._undo.Click += new System.EventHandler(this._undo_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._dot);
			this.groupBox1.Controls.Add(this._circle);
			this.groupBox1.Controls.Add(this._triangle);
			this.groupBox1.Controls.Add(this._line);
			this.groupBox1.Controls.Add(this._rectangle);
			this.groupBox1.Controls.Add(this._polyLine);
			this.groupBox1.Location = new System.Drawing.Point(344, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(252, 60);
			this.groupBox1.TabIndex = 6;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Draw";
			// 
			// _dot
			// 
			this._dot.AutoSize = true;
			this._dot.Location = new System.Drawing.Point(84, 37);
			this._dot.Name = "_dot";
			this._dot.Size = new System.Drawing.Size(42, 17);
			this._dot.TabIndex = 10;
			this._dot.TabStop = true;
			this._dot.Text = "Dot";
			this._dot.UseVisualStyleBackColor = true;
			this._dot.CheckedChanged += new System.EventHandler(this._dot_CheckedChanged);
			// 
			// _circle
			// 
			this._circle.AutoSize = true;
			this._circle.Location = new System.Drawing.Point(84, 16);
			this._circle.Name = "_circle";
			this._circle.Size = new System.Drawing.Size(51, 17);
			this._circle.TabIndex = 9;
			this._circle.TabStop = true;
			this._circle.Text = "Circle";
			this._circle.UseVisualStyleBackColor = true;
			this._circle.CheckedChanged += new System.EventHandler(this._circle_CheckedChanged);
			// 
			// _triangle
			// 
			this._triangle.AutoSize = true;
			this._triangle.Location = new System.Drawing.Point(141, 37);
			this._triangle.Name = "_triangle";
			this._triangle.Size = new System.Drawing.Size(63, 17);
			this._triangle.TabIndex = 8;
			this._triangle.TabStop = true;
			this._triangle.Text = "Triangle";
			this._triangle.UseVisualStyleBackColor = true;
			this._triangle.CheckedChanged += new System.EventHandler(this._triangle_CheckedChanged);
			// 
			// _line
			// 
			this._line.AutoSize = true;
			this._line.Location = new System.Drawing.Point(141, 16);
			this._line.Name = "_line";
			this._line.Size = new System.Drawing.Size(45, 17);
			this._line.TabIndex = 7;
			this._line.TabStop = true;
			this._line.Text = "Line";
			this._line.UseVisualStyleBackColor = true;
			this._line.CheckedChanged += new System.EventHandler(this._line_CheckedChanged);
			// 
			// _rectangle
			// 
			this._rectangle.AutoSize = true;
			this._rectangle.Location = new System.Drawing.Point(6, 37);
			this._rectangle.Name = "_rectangle";
			this._rectangle.Size = new System.Drawing.Size(74, 17);
			this._rectangle.TabIndex = 6;
			this._rectangle.TabStop = true;
			this._rectangle.Text = "Rectangle";
			this._rectangle.UseVisualStyleBackColor = true;
			this._rectangle.CheckedChanged += new System.EventHandler(this._rectangle_CheckedChanged);
			// 
			// _polyLine
			// 
			this._polyLine.AutoSize = true;
			this._polyLine.Location = new System.Drawing.Point(6, 16);
			this._polyLine.Name = "_polyLine";
			this._polyLine.Size = new System.Drawing.Size(65, 17);
			this._polyLine.TabIndex = 5;
			this._polyLine.TabStop = true;
			this._polyLine.Text = "PolyLine";
			this._polyLine.UseVisualStyleBackColor = true;
			this._polyLine.CheckedChanged += new System.EventHandler(this._polyLine_CheckedChanged);
			// 
			// _home
			// 
			this._home.Location = new System.Drawing.Point(6, 52);
			this._home.Name = "_home";
			this._home.Size = new System.Drawing.Size(75, 23);
			this._home.TabIndex = 4;
			this._home.Text = "Home";
			this._home.UseVisualStyleBackColor = true;
			this._home.Click += new System.EventHandler(this._home_Click);
			// 
			// _load
			// 
			this._load.Location = new System.Drawing.Point(600, 31);
			this._load.Name = "_load";
			this._load.Size = new System.Drawing.Size(75, 23);
			this._load.TabIndex = 3;
			this._load.Text = "Load";
			this._load.UseVisualStyleBackColor = true;
			this._load.Click += new System.EventHandler(this._load_Click);
			// 
			// _save
			// 
			this._save.Location = new System.Drawing.Point(600, 7);
			this._save.Name = "_save";
			this._save.Size = new System.Drawing.Size(75, 23);
			this._save.TabIndex = 2;
			this._save.Text = "Save";
			this._save.UseVisualStyleBackColor = true;
			this._save.Click += new System.EventHandler(this._save_Click);
			// 
			// _clear
			// 
			this._clear.Location = new System.Drawing.Point(6, 29);
			this._clear.Name = "_clear";
			this._clear.Size = new System.Drawing.Size(75, 23);
			this._clear.TabIndex = 1;
			this._clear.Text = "Clear";
			this._clear.UseVisualStyleBackColor = true;
			this._clear.Click += new System.EventHandler(this._clear_Click);
			// 
			// _paintAgain
			// 
			this._paintAgain.Location = new System.Drawing.Point(6, 6);
			this._paintAgain.Name = "_paintAgain";
			this._paintAgain.Size = new System.Drawing.Size(75, 23);
			this._paintAgain.TabIndex = 0;
			this._paintAgain.Text = "Paint";
			this._paintAgain.UseVisualStyleBackColor = true;
			this._paintAgain.Click += new System.EventHandler(this._paintAgain_Click);
			// 
			// _mainPanel
			// 
			this._mainPanel.Controls.Add(this._plotterCtrl);
			this._mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mainPanel.Location = new System.Drawing.Point(0, 88);
			this._mainPanel.Name = "_mainPanel";
			this._mainPanel.Size = new System.Drawing.Size(687, 422);
			this._mainPanel.TabIndex = 2;
			// 
			// _plotterCtrl
			// 
			this._plotterCtrl.BackColor = System.Drawing.Color.White;
			this._plotterCtrl.CreateNewShape = "PolyLine";
			this._plotterCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._plotterCtrl.Location = new System.Drawing.Point(0, 0);
			this._plotterCtrl.Name = "_plotterCtrl";
			this._plotterCtrl.ReadOnly = false;
			this._plotterCtrl.SelectedColor = System.Drawing.Color.Black;
			this._plotterCtrl.SelectedLinesize = 1;
			this._plotterCtrl.SelectedShape = -1;
			this._plotterCtrl.Size = new System.Drawing.Size(687, 422);
			this._plotterCtrl.SizeXHPGL = 20850;
			this._plotterCtrl.SizeYHPGL = 12000;
			this._plotterCtrl.TabIndex = 1;
			this._plotterCtrl.ShapeCreating += new Plotter.GUI.PlotterUserControl.ShapeEventHandler(this._plotterCtrl_ShapeCreating);
			this._plotterCtrl.ShapeCreated += new Plotter.GUI.PlotterUserControl.ShapeEventHandler(this._plotterCtrl_ShapeCreated);
			this._plotterCtrl.PolylineAddPoint += new Plotter.GUI.PlotterUserControl.ShapeEventHandler(this._plotterCtrl_PolylineAddPoint);
			this._plotterCtrl.HpglMousePosition += new Plotter.GUI.PlotterUserControl.ShapeEventHandler(this._plotterCtrl_HpglMousePosition);
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
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this._mainPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private PlotterUserControl _plotterCtrl;
        private System.Windows.Forms.Panel _headerPanel;
        private System.Windows.Forms.Panel _mainPanel;
        private System.Windows.Forms.Button _paintAgain;
        private System.Windows.Forms.Button _clear;
        private System.Windows.Forms.Button _save;
        private System.Windows.Forms.Button _load;
        private System.Windows.Forms.Button _home;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton _rectangle;
        private System.Windows.Forms.RadioButton _polyLine;
        private System.Windows.Forms.RadioButton _triangle;
        private System.Windows.Forms.RadioButton _line;
        private System.Windows.Forms.Button _undo;
        private System.Windows.Forms.RadioButton _dot;
        private System.Windows.Forms.RadioButton _circle;
        private System.Windows.Forms.Button _selPrev;
        private System.Windows.Forms.Button _selNext;
        private System.Windows.Forms.Button _del;
        private System.Windows.Forms.Button _selLast;
        private System.Windows.Forms.Button _selFirst;
        private System.Windows.Forms.CheckBox _plot;
        private System.Windows.Forms.Label _coord;
        private System.Windows.Forms.Label _selidxlbl;
        private System.Windows.Forms.Button _paintfrom;
        private System.Windows.Forms.Button _paintSelected;
        private System.Windows.Forms.TextBox _paintcount;
		private System.Windows.Forms.Button _abort;
    }
}