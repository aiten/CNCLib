using System.Threading;
namespace Plotter.GUI
{
    partial class SendFileForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (Com != null)
            {
                Com.AbortCommand();
            }
            if (_eventlist != null)
            {
                _eventlist = null;
                if (_guicopyThread.ThreadState == ThreadState.Suspended)
                    _guicopyThread.Resume();
            }
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this._filename = new System.Windows.Forms.TextBox();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._fileopen = new System.Windows.Forms.Button();
            this._file_start = new System.Windows.Forms.Button();
            this._results = new System.Windows.Forms.ListView();
            this._step = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._command = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._result = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._headerpanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._file_abort = new System.Windows.Forms.Button();
            this._debug_break = new System.Windows.Forms.Button();
            this._singlestep = new System.Windows.Forms.CheckBox();
            this._debug_run = new System.Windows.Forms.Button();
            this._debug_step = new System.Windows.Forms.Button();
            this._commandBtn = new System.Windows.Forms.Button();
            this._sendcommand = new System.Windows.Forms.TextBox();
            this._panel = new System.Windows.Forms.Panel();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this._headerpanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this._panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File";
            // 
            // _filename
            // 
            this._filename.Location = new System.Drawing.Point(66, 19);
            this._filename.Name = "_filename";
            this._filename.Size = new System.Drawing.Size(313, 20);
            this._filename.TabIndex = 1;
            // 
            // _openFileDialog
            // 
            this._openFileDialog.FileName = "openFileDialog1";
            // 
            // _fileopen
            // 
            this._fileopen.Location = new System.Drawing.Point(385, 16);
            this._fileopen.Name = "_fileopen";
            this._fileopen.Size = new System.Drawing.Size(31, 23);
            this._fileopen.TabIndex = 2;
            this._fileopen.Text = "?";
            this._fileopen.UseVisualStyleBackColor = true;
            this._fileopen.Click += new System.EventHandler(this._fileopen_Click);
            // 
            // _file_start
            // 
            this._file_start.Location = new System.Drawing.Point(31, 45);
            this._file_start.Name = "_file_start";
            this._file_start.Size = new System.Drawing.Size(263, 23);
            this._file_start.TabIndex = 3;
            this._file_start.Text = "Start";
            this._file_start.UseVisualStyleBackColor = true;
            this._file_start.Click += new System.EventHandler(this._startFile_Click);
            // 
            // _results
            // 
            this._results.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._step,
            this._command,
            this._result});
            this._results.Cursor = System.Windows.Forms.Cursors.Default;
            this._results.Dock = System.Windows.Forms.DockStyle.Fill;
            this._results.GridLines = true;
            this._results.Location = new System.Drawing.Point(0, 0);
            this._results.Name = "_results";
            this._results.Size = new System.Drawing.Size(929, 585);
            this._results.TabIndex = 4;
            this._results.UseCompatibleStateImageBehavior = false;
            this._results.View = System.Windows.Forms.View.Details;
            // 
            // _step
            // 
            this._step.Text = "Step";
            // 
            // _command
            // 
            this._command.Text = "Command";
            this._command.Width = 700;
            // 
            // _result
            // 
            this._result.Text = "Result";
            this._result.Width = 200;
            // 
            // _headerpanel
            // 
            this._headerpanel.Controls.Add(this.groupBox1);
            this._headerpanel.Controls.Add(this._commandBtn);
            this._headerpanel.Controls.Add(this._sendcommand);
            this._headerpanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerpanel.Location = new System.Drawing.Point(0, 0);
            this._headerpanel.Name = "_headerpanel";
            this._headerpanel.Size = new System.Drawing.Size(929, 130);
            this._headerpanel.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._filename);
            this.groupBox1.Controls.Add(this._fileopen);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this._file_start);
            this.groupBox1.Controls.Add(this._file_abort);
            this.groupBox1.Controls.Add(this._debug_break);
            this.groupBox1.Controls.Add(this._singlestep);
            this.groupBox1.Controls.Add(this._debug_run);
            this.groupBox1.Controls.Add(this._debug_step);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(583, 106);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "File";
            // 
            // _file_abort
            // 
            this._file_abort.Location = new System.Drawing.Point(303, 45);
            this._file_abort.Name = "_file_abort";
            this._file_abort.Size = new System.Drawing.Size(113, 23);
            this._file_abort.TabIndex = 4;
            this._file_abort.Text = "Abort";
            this._file_abort.UseVisualStyleBackColor = true;
            this._file_abort.Click += new System.EventHandler(this._file_abort_Click);
            // 
            // _debug_break
            // 
            this._debug_break.Location = new System.Drawing.Point(433, 74);
            this._debug_break.Name = "_debug_break";
            this._debug_break.Size = new System.Drawing.Size(123, 23);
            this._debug_break.TabIndex = 8;
            this._debug_break.Text = "Break";
            this._debug_break.UseVisualStyleBackColor = true;
            this._debug_break.Click += new System.EventHandler(this._break_Click);
            // 
            // _singlestep
            // 
            this._singlestep.AutoSize = true;
            this._singlestep.Location = new System.Drawing.Point(31, 74);
            this._singlestep.Name = "_singlestep";
            this._singlestep.Size = new System.Drawing.Size(102, 17);
            this._singlestep.TabIndex = 6;
            this._singlestep.Text = "Start SingleStep";
            this._singlestep.UseVisualStyleBackColor = true;
            // 
            // _debug_run
            // 
            this._debug_run.Location = new System.Drawing.Point(433, 45);
            this._debug_run.Name = "_debug_run";
            this._debug_run.Size = new System.Drawing.Size(123, 23);
            this._debug_run.TabIndex = 7;
            this._debug_run.Text = "Run";
            this._debug_run.UseVisualStyleBackColor = true;
            this._debug_run.Click += new System.EventHandler(this._debug_Run_Click);
            // 
            // _debug_step
            // 
            this._debug_step.Location = new System.Drawing.Point(433, 16);
            this._debug_step.Name = "_debug_step";
            this._debug_step.Size = new System.Drawing.Size(123, 23);
            this._debug_step.TabIndex = 5;
            this._debug_step.Text = "Step";
            this._debug_step.UseVisualStyleBackColor = true;
            this._debug_step.Click += new System.EventHandler(this._debug_step_Click);
            // 
            // _commandBtn
            // 
            this._commandBtn.Location = new System.Drawing.Point(857, 53);
            this._commandBtn.Name = "_commandBtn";
            this._commandBtn.Size = new System.Drawing.Size(60, 23);
            this._commandBtn.TabIndex = 11;
            this._commandBtn.Text = "Send";
            this._commandBtn.UseVisualStyleBackColor = true;
            this._commandBtn.Click += new System.EventHandler(this._commandBtn_Click);
            // 
            // _sendcommand
            // 
            this._sendcommand.Location = new System.Drawing.Point(608, 27);
            this._sendcommand.Name = "_sendcommand";
            this._sendcommand.Size = new System.Drawing.Size(309, 20);
            this._sendcommand.TabIndex = 10;
            this._sendcommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._sendcommand_KeyPress);
            // 
            // _panel
            // 
            this._panel.Controls.Add(this._results);
            this._panel.Cursor = System.Windows.Forms.Cursors.Default;
            this._panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panel.Location = new System.Drawing.Point(0, 130);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(929, 585);
            this._panel.TabIndex = 6;
            // 
            // _timer
            // 
            this._timer.Enabled = true;
            this._timer.Tick += new System.EventHandler(this._timer_Tick);
            // 
            // SendFileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 715);
            this.Controls.Add(this._panel);
            this.Controls.Add(this._headerpanel);
            this.Name = "SendFileForm";
            this.Text = "Arduino Plotter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SendFileForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this._headerpanel.ResumeLayout(false);
            this._headerpanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this._panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _filename;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.Button _fileopen;
        private System.Windows.Forms.Button _file_start;
        private System.Windows.Forms.ListView _results;
        private System.Windows.Forms.ColumnHeader _command;
        private System.Windows.Forms.ColumnHeader _result;
        private System.Windows.Forms.Panel _headerpanel;
        private System.Windows.Forms.Panel _panel;
        private System.Windows.Forms.Button _file_abort;
        private System.Windows.Forms.Button _debug_step;
        private System.Windows.Forms.CheckBox _singlestep;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.Button _debug_run;
        private System.Windows.Forms.Button _debug_break;
        private System.Windows.Forms.ColumnHeader _step;
        private System.Windows.Forms.Button _commandBtn;
        private System.Windows.Forms.TextBox _sendcommand;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

