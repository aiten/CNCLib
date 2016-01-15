namespace CNCLib.GUI
{
    partial class GCodeUserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.SuspendLayout();
			// 
			// GCodeUserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Name = "GCodeUserControl";
			this.Size = new System.Drawing.Size(419, 270);
			this.BackColorChanged += new System.EventHandler(this.GCodeUserControl_BackColorChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.PlotterUserControl_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PlotterUserControl_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PlotterUserControl_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PlotterUserControl_MouseUp);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PlotterUserControl_MouseWheel);
			this.Resize += new System.EventHandler(this.PlotterUserControl_Resize);
			this.ResumeLayout(false);

        }

        #endregion

    }
}
