namespace GCode.GUI.Load
{
    partial class LoadOptionForm
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
			this.label2 = new System.Windows.Forms.Label();
			this._ofsX = new System.Windows.Forms.TextBox();
			this._ofsY = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this._scaleX = new System.Windows.Forms.TextBox();
			this._scaleY = new System.Windows.Forms.TextBox();
			this._swapXY = new System.Windows.Forms.CheckBox();
			this._filename = new System.Windows.Forms.TextBox();
			this._fileopen = new System.Windows.Forms.Button();
			this._load = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this._AutoScaleSizeX = new System.Windows.Forms.TextBox();
			this._AutoScaleSizeY = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this._autoScaleGb = new System.Windows.Forms.GroupBox();
			this._AutoScaleKeepRatio = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this._AutoScaleBorderDistY = new System.Windows.Forms.TextBox();
			this._AutoScaleBorderDistX = new System.Windows.Forms.TextBox();
			this._autoScale = new System.Windows.Forms.CheckBox();
			this._loadGCode = new System.Windows.Forms.Button();
			this._autoScaleGb.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(10, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(23, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Ofs";
			// 
			// _ofsX
			// 
			this._ofsX.Location = new System.Drawing.Point(48, 84);
			this._ofsX.Name = "_ofsX";
			this._ofsX.Size = new System.Drawing.Size(99, 20);
			this._ofsX.TabIndex = 7;
			this._ofsX.Text = "0";
			// 
			// _ofsY
			// 
			this._ofsY.Location = new System.Drawing.Point(153, 81);
			this._ofsY.Name = "_ofsY";
			this._ofsY.Size = new System.Drawing.Size(85, 20);
			this._ofsY.TabIndex = 8;
			this._ofsY.Text = "0";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(10, 65);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Scale";
			// 
			// _scaleX
			// 
			this._scaleX.Location = new System.Drawing.Point(48, 61);
			this._scaleX.Name = "_scaleX";
			this._scaleX.Size = new System.Drawing.Size(99, 20);
			this._scaleX.TabIndex = 4;
			this._scaleX.Text = "1";
			// 
			// _scaleY
			// 
			this._scaleY.Location = new System.Drawing.Point(153, 60);
			this._scaleY.Name = "_scaleY";
			this._scaleY.Size = new System.Drawing.Size(85, 20);
			this._scaleY.TabIndex = 5;
			this._scaleY.Text = "1";
			// 
			// _swapXY
			// 
			this._swapXY.AutoSize = true;
			this._swapXY.Location = new System.Drawing.Point(48, 39);
			this._swapXY.Name = "_swapXY";
			this._swapXY.Size = new System.Drawing.Size(70, 17);
			this._swapXY.TabIndex = 2;
			this._swapXY.Text = "Swap XY";
			this._swapXY.UseVisualStyleBackColor = true;
			// 
			// _filename
			// 
			this._filename.Location = new System.Drawing.Point(12, 12);
			this._filename.Name = "_filename";
			this._filename.Size = new System.Drawing.Size(313, 20);
			this._filename.TabIndex = 0;
			// 
			// _fileopen
			// 
			this._fileopen.Location = new System.Drawing.Point(331, 9);
			this._fileopen.Name = "_fileopen";
			this._fileopen.Size = new System.Drawing.Size(31, 23);
			this._fileopen.TabIndex = 1;
			this._fileopen.Text = "?";
			this._fileopen.UseVisualStyleBackColor = true;
			this._fileopen.Click += new System.EventHandler(this._fileopen_Click);
			// 
			// _load
			// 
			this._load.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._load.Location = new System.Drawing.Point(368, 39);
			this._load.Name = "_load";
			this._load.Size = new System.Drawing.Size(75, 23);
			this._load.TabIndex = 9;
			this._load.Text = "Load";
			this._load.UseVisualStyleBackColor = true;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(368, 10);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 10;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// _AutoScaleSizeX
			// 
			this._AutoScaleSizeX.Location = new System.Drawing.Point(85, 41);
			this._AutoScaleSizeX.Name = "_AutoScaleSizeX";
			this._AutoScaleSizeX.Size = new System.Drawing.Size(69, 20);
			this._AutoScaleSizeX.TabIndex = 11;
			// 
			// _AutoScaleSizeY
			// 
			this._AutoScaleSizeY.Location = new System.Drawing.Point(160, 41);
			this._AutoScaleSizeY.Name = "_AutoScaleSizeY";
			this._AutoScaleSizeY.Size = new System.Drawing.Size(59, 20);
			this._AutoScaleSizeY.TabIndex = 12;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(11, 45);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(50, 13);
			this.label3.TabIndex = 13;
			this.label3.Text = "Scale To";
			// 
			// _autoScaleGb
			// 
			this._autoScaleGb.Controls.Add(this._AutoScaleKeepRatio);
			this._autoScaleGb.Controls.Add(this.label4);
			this._autoScaleGb.Controls.Add(this._AutoScaleBorderDistY);
			this._autoScaleGb.Controls.Add(this._AutoScaleBorderDistX);
			this._autoScaleGb.Controls.Add(this._autoScale);
			this._autoScaleGb.Controls.Add(this.label3);
			this._autoScaleGb.Controls.Add(this._AutoScaleSizeY);
			this._autoScaleGb.Controls.Add(this._AutoScaleSizeX);
			this._autoScaleGb.Location = new System.Drawing.Point(12, 120);
			this._autoScaleGb.Name = "_autoScaleGb";
			this._autoScaleGb.Size = new System.Drawing.Size(269, 107);
			this._autoScaleGb.TabIndex = 14;
			this._autoScaleGb.TabStop = false;
			this._autoScaleGb.Text = "AutoScale";
			// 
			// _AutoScaleKeepRatio
			// 
			this._AutoScaleKeepRatio.AutoSize = true;
			this._AutoScaleKeepRatio.Location = new System.Drawing.Point(141, 18);
			this._AutoScaleKeepRatio.Name = "_AutoScaleKeepRatio";
			this._AutoScaleKeepRatio.Size = new System.Drawing.Size(79, 17);
			this._AutoScaleKeepRatio.TabIndex = 17;
			this._AutoScaleKeepRatio.Text = "Keep Ratio";
			this._AutoScaleKeepRatio.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(11, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(25, 13);
			this.label4.TabIndex = 16;
			this.label4.Text = "Dist";
			// 
			// _AutoScaleBorderDistY
			// 
			this._AutoScaleBorderDistY.Location = new System.Drawing.Point(160, 64);
			this._AutoScaleBorderDistY.Name = "_AutoScaleBorderDistY";
			this._AutoScaleBorderDistY.Size = new System.Drawing.Size(59, 20);
			this._AutoScaleBorderDistY.TabIndex = 15;
			// 
			// _AutoScaleBorderDistX
			// 
			this._AutoScaleBorderDistX.Location = new System.Drawing.Point(85, 64);
			this._AutoScaleBorderDistX.Name = "_AutoScaleBorderDistX";
			this._AutoScaleBorderDistX.Size = new System.Drawing.Size(69, 20);
			this._AutoScaleBorderDistX.TabIndex = 14;
			// 
			// _autoScale
			// 
			this._autoScale.AutoSize = true;
			this._autoScale.Location = new System.Drawing.Point(18, 19);
			this._autoScale.Name = "_autoScale";
			this._autoScale.Size = new System.Drawing.Size(78, 17);
			this._autoScale.TabIndex = 3;
			this._autoScale.Text = "Auto Scale";
			this._autoScale.UseVisualStyleBackColor = true;
			this._autoScale.CheckedChanged += new System.EventHandler(this._autoScale_CheckedChanged);
			// 
			// _loadGCode
			// 
			this._loadGCode.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this._loadGCode.Location = new System.Drawing.Point(368, 68);
			this._loadGCode.Name = "_loadGCode";
			this._loadGCode.Size = new System.Drawing.Size(75, 23);
			this._loadGCode.TabIndex = 15;
			this._loadGCode.Text = "Load GCode";
			this._loadGCode.UseVisualStyleBackColor = true;
			// 
			// LoadOptionForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(455, 240);
			this.Controls.Add(this._loadGCode);
			this.Controls.Add(this._autoScaleGb);
			this.Controls.Add(this.button1);
			this.Controls.Add(this._load);
			this.Controls.Add(this._filename);
			this.Controls.Add(this._fileopen);
			this.Controls.Add(this.label2);
			this.Controls.Add(this._ofsX);
			this.Controls.Add(this._ofsY);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._scaleX);
			this.Controls.Add(this._scaleY);
			this.Controls.Add(this._swapXY);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoadOptionForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Load ...";
			this._autoScaleGb.ResumeLayout(false);
			this._autoScaleGb.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _ofsX;
        private System.Windows.Forms.TextBox _ofsY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _scaleX;
        private System.Windows.Forms.TextBox _scaleY;
        private System.Windows.Forms.CheckBox _swapXY;
        private System.Windows.Forms.TextBox _filename;
        private System.Windows.Forms.Button _fileopen;
        private System.Windows.Forms.Button _load;
        private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox _AutoScaleSizeX;
		private System.Windows.Forms.TextBox _AutoScaleSizeY;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox _autoScaleGb;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _AutoScaleBorderDistY;
		private System.Windows.Forms.TextBox _AutoScaleBorderDistX;
		private System.Windows.Forms.CheckBox _autoScale;
		private System.Windows.Forms.CheckBox _AutoScaleKeepRatio;
		private System.Windows.Forms.Button _loadGCode;
    }
}