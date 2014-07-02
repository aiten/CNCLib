namespace GCode.GUI
{
    partial class MainForm
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
			this._connect = new System.Windows.Forms.Button();
			this._com = new System.Windows.Forms.ComboBox();
			this._paint = new System.Windows.Forms.Button();
			this._reset = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _connect
			// 
			this._connect.Location = new System.Drawing.Point(128, 12);
			this._connect.Name = "_connect";
			this._connect.Size = new System.Drawing.Size(102, 23);
			this._connect.TabIndex = 14;
			this._connect.Text = "Connect";
			this._connect.UseVisualStyleBackColor = true;
			this._connect.Click += new System.EventHandler(this._connect_Click);
			// 
			// _com
			// 
			this._com.FormattingEnabled = true;
			this._com.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9"});
			this._com.Location = new System.Drawing.Point(20, 11);
			this._com.Name = "_com";
			this._com.Size = new System.Drawing.Size(102, 21);
			this._com.TabIndex = 13;
			// 
			// _paint
			// 
			this._paint.Location = new System.Drawing.Point(20, 79);
			this._paint.Name = "_paint";
			this._paint.Size = new System.Drawing.Size(210, 23);
			this._paint.TabIndex = 16;
			this._paint.Text = "Paint";
			this._paint.UseVisualStyleBackColor = true;
			this._paint.Click += new System.EventHandler(this._paint_Click);
			// 
			// _reset
			// 
			this._reset.AutoSize = true;
			this._reset.Location = new System.Drawing.Point(128, 41);
			this._reset.Name = "_reset";
			this._reset.Size = new System.Drawing.Size(54, 17);
			this._reset.TabIndex = 17;
			this._reset.Text = "Reset";
			this._reset.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(248, 261);
			this.Controls.Add(this._reset);
			this.Controls.Add(this._paint);
			this.Controls.Add(this._connect);
			this.Controls.Add(this._com);
			this.Name = "MainForm";
			this.Text = "GCode";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _connect;
        private System.Windows.Forms.ComboBox _com;
        private System.Windows.Forms.Button _paint;
		private System.Windows.Forms.CheckBox _reset;
    }
}