namespace CNCLib.GUI.Load
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
            this._generateForEngrageGroup = new System.Windows.Forms.GroupBox();
            this._engraveUseParameter = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this._engraveZUp = new System.Windows.Forms.TextBox();
            this._engraveZDown = new System.Windows.Forms.TextBox();
            this._generateForEngrave = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this._generateForLaserGroup = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this._laserOff = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this._laserOn = new System.Windows.Forms.TextBox();
            this._generateForLaser = new System.Windows.Forms.RadioButton();
            this._generalGroup = new System.Windows.Forms.GroupBox();
            this._autoScaleGb.SuspendLayout();
            this._generateForEngrageGroup.SuspendLayout();
            this._generateForLaserGroup.SuspendLayout();
            this._generalGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ofs";
            // 
            // _ofsX
            // 
            this._ofsX.Location = new System.Drawing.Point(63, 64);
            this._ofsX.Name = "_ofsX";
            this._ofsX.Size = new System.Drawing.Size(99, 20);
            this._ofsX.TabIndex = 7;
            this._ofsX.Text = "0";
            // 
            // _ofsY
            // 
            this._ofsY.Location = new System.Drawing.Point(168, 61);
            this._ofsY.Name = "_ofsY";
            this._ofsY.Size = new System.Drawing.Size(85, 20);
            this._ofsY.TabIndex = 8;
            this._ofsY.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Scale";
            // 
            // _scaleX
            // 
            this._scaleX.Location = new System.Drawing.Point(63, 41);
            this._scaleX.Name = "_scaleX";
            this._scaleX.Size = new System.Drawing.Size(99, 20);
            this._scaleX.TabIndex = 4;
            this._scaleX.Text = "1";
            // 
            // _scaleY
            // 
            this._scaleY.Location = new System.Drawing.Point(168, 40);
            this._scaleY.Name = "_scaleY";
            this._scaleY.Size = new System.Drawing.Size(85, 20);
            this._scaleY.TabIndex = 5;
            this._scaleY.Text = "1";
            // 
            // _swapXY
            // 
            this._swapXY.AutoSize = true;
            this._swapXY.Location = new System.Drawing.Point(63, 19);
            this._swapXY.Name = "_swapXY";
            this._swapXY.Size = new System.Drawing.Size(70, 17);
            this._swapXY.TabIndex = 2;
            this._swapXY.Text = "Swap XY";
            this._swapXY.UseVisualStyleBackColor = true;
            // 
            // _filename
            // 
            this._filename.Location = new System.Drawing.Point(56, 12);
            this._filename.Name = "_filename";
            this._filename.Size = new System.Drawing.Size(446, 20);
            this._filename.TabIndex = 0;
            // 
            // _fileopen
            // 
            this._fileopen.Location = new System.Drawing.Point(508, 9);
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
            this._load.Location = new System.Drawing.Point(382, 286);
            this._load.Name = "_load";
            this._load.Size = new System.Drawing.Size(77, 23);
            this._load.TabIndex = 9;
            this._load.Text = "Load";
            this._load.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(465, 286);
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
            this._autoScaleGb.Location = new System.Drawing.Point(304, 44);
            this._autoScaleGb.Name = "_autoScaleGb";
            this._autoScaleGb.Size = new System.Drawing.Size(235, 96);
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
            this._loadGCode.Location = new System.Drawing.Point(292, 286);
            this._loadGCode.Name = "_loadGCode";
            this._loadGCode.Size = new System.Drawing.Size(84, 23);
            this._loadGCode.TabIndex = 15;
            this._loadGCode.Text = "Load GCode";
            this._loadGCode.UseVisualStyleBackColor = true;
            // 
            // _generateForEngrageGroup
            // 
            this._generateForEngrageGroup.Controls.Add(this._engraveUseParameter);
            this._generateForEngrageGroup.Controls.Add(this.label11);
            this._generateForEngrageGroup.Controls.Add(this.label10);
            this._generateForEngrageGroup.Controls.Add(this.label6);
            this._generateForEngrageGroup.Controls.Add(this._engraveZUp);
            this._generateForEngrageGroup.Controls.Add(this._engraveZDown);
            this._generateForEngrageGroup.Location = new System.Drawing.Point(21, 177);
            this._generateForEngrageGroup.Name = "_generateForEngrageGroup";
            this._generateForEngrageGroup.Size = new System.Drawing.Size(246, 94);
            this._generateForEngrageGroup.TabIndex = 18;
            this._generateForEngrageGroup.TabStop = false;
            this._generateForEngrageGroup.Text = "Engrave";
            // 
            // _engraveUseParameter
            // 
            this._engraveUseParameter.AutoSize = true;
            this._engraveUseParameter.Location = new System.Drawing.Point(85, 64);
            this._engraveUseParameter.Name = "_engraveUseParameter";
            this._engraveUseParameter.Size = new System.Drawing.Size(95, 17);
            this._engraveUseParameter.TabIndex = 21;
            this._engraveUseParameter.Text = "Use parameter";
            this._engraveUseParameter.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(171, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = "Down";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(95, 16);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(21, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "Up";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "ZPos";
            // 
            // _engraveZUp
            // 
            this._engraveZUp.Location = new System.Drawing.Point(85, 33);
            this._engraveZUp.Name = "_engraveZUp";
            this._engraveZUp.Size = new System.Drawing.Size(69, 20);
            this._engraveZUp.TabIndex = 11;
            // 
            // _engraveZDown
            // 
            this._engraveZDown.Location = new System.Drawing.Point(160, 33);
            this._engraveZDown.Name = "_engraveZDown";
            this._engraveZDown.Size = new System.Drawing.Size(59, 20);
            this._engraveZDown.TabIndex = 12;
            // 
            // _generateForEngrave
            // 
            this._generateForEngrave.AutoSize = true;
            this._generateForEngrave.Location = new System.Drawing.Point(28, 157);
            this._generateForEngrave.Name = "_generateForEngrave";
            this._generateForEngrave.Size = new System.Drawing.Size(126, 17);
            this._generateForEngrave.TabIndex = 3;
            this._generateForEngrave.Text = "Generate for engrave";
            this._generateForEngrave.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "File";
            // 
            // _generateForLaserGroup
            // 
            this._generateForLaserGroup.Controls.Add(this.label8);
            this._generateForLaserGroup.Controls.Add(this._laserOff);
            this._generateForLaserGroup.Controls.Add(this.label9);
            this._generateForLaserGroup.Controls.Add(this._laserOn);
            this._generateForLaserGroup.Location = new System.Drawing.Point(278, 177);
            this._generateForLaserGroup.Name = "_generateForLaserGroup";
            this._generateForLaserGroup.Size = new System.Drawing.Size(180, 94);
            this._generateForLaserGroup.TabIndex = 19;
            this._generateForLaserGroup.TabStop = false;
            this._generateForLaserGroup.Text = "Laser";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Laser Off";
            // 
            // _laserOff
            // 
            this._laserOff.Location = new System.Drawing.Point(85, 44);
            this._laserOff.Name = "_laserOff";
            this._laserOff.Size = new System.Drawing.Size(69, 20);
            this._laserOff.TabIndex = 14;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 13;
            this.label9.Text = "Laser On";
            // 
            // _laserOn
            // 
            this._laserOn.Location = new System.Drawing.Point(85, 21);
            this._laserOn.Name = "_laserOn";
            this._laserOn.Size = new System.Drawing.Size(69, 20);
            this._laserOn.TabIndex = 11;
            // 
            // _generateForLaser
            // 
            this._generateForLaser.AutoSize = true;
            this._generateForLaser.Location = new System.Drawing.Point(278, 157);
            this._generateForLaser.Name = "_generateForLaser";
            this._generateForLaser.Size = new System.Drawing.Size(109, 17);
            this._generateForLaser.TabIndex = 20;
            this._generateForLaser.Text = "Generate for laser";
            this._generateForLaser.UseVisualStyleBackColor = true;
            // 
            // _generalGroup
            // 
            this._generalGroup.Controls.Add(this._swapXY);
            this._generalGroup.Controls.Add(this.label1);
            this._generalGroup.Controls.Add(this._scaleX);
            this._generalGroup.Controls.Add(this._scaleY);
            this._generalGroup.Controls.Add(this.label2);
            this._generalGroup.Controls.Add(this._ofsY);
            this._generalGroup.Controls.Add(this._ofsX);
            this._generalGroup.Location = new System.Drawing.Point(21, 44);
            this._generalGroup.Name = "_generalGroup";
            this._generalGroup.Size = new System.Drawing.Size(262, 96);
            this._generalGroup.TabIndex = 21;
            this._generalGroup.TabStop = false;
            this._generalGroup.Text = "Options";
            // 
            // LoadOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 316);
            this.Controls.Add(this.label7);
            this.Controls.Add(this._filename);
            this.Controls.Add(this._fileopen);
            this.Controls.Add(this._generalGroup);
            this.Controls.Add(this._autoScaleGb);
            this.Controls.Add(this._generateForLaser);
            this.Controls.Add(this._generateForEngrave);
            this.Controls.Add(this._generateForLaserGroup);
            this.Controls.Add(this._generateForEngrageGroup);
            this.Controls.Add(this._loadGCode);
            this.Controls.Add(this.button1);
            this.Controls.Add(this._load);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoadOptionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Load ...";
            this._autoScaleGb.ResumeLayout(false);
            this._autoScaleGb.PerformLayout();
            this._generateForEngrageGroup.ResumeLayout(false);
            this._generateForEngrageGroup.PerformLayout();
            this._generateForLaserGroup.ResumeLayout(false);
            this._generateForLaserGroup.PerformLayout();
            this._generalGroup.ResumeLayout(false);
            this._generalGroup.PerformLayout();
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
		private System.Windows.Forms.GroupBox _generateForEngrageGroup;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox _engraveZDown;
		private System.Windows.Forms.TextBox _engraveZUp;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox _generateForLaserGroup;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox _laserOff;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox _laserOn;
		private System.Windows.Forms.CheckBox _engraveUseParameter;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.RadioButton _generateForLaser;
		private System.Windows.Forms.GroupBox _generalGroup;
		private System.Windows.Forms.RadioButton _generateForEngrave;
	}
}