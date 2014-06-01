using System.Threading;
namespace Roboter.GUI
{
    partial class MainForm
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
            if (_communication != null)
            {
                _communication.Abort = true;
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
            this._run = new System.Windows.Forms.Button();
            this._results = new System.Windows.Forms.ListView();
            this._step = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._command = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._result = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._headerpanel = new System.Windows.Forms.Panel();
            this._commandBtn = new System.Windows.Forms.Button();
            this._sendcommand = new System.Windows.Forms.TextBox();
            this._com = new System.Windows.Forms.ComboBox();
            this._break = new System.Windows.Forms.Button();
            this._executeRun = new System.Windows.Forms.Button();
            this._singlestep = new System.Windows.Forms.CheckBox();
            this._executesingleStep = new System.Windows.Forms.Button();
            this._abort = new System.Windows.Forms.Button();
            this._panel = new System.Windows.Forms.Panel();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this._headerpanel.SuspendLayout();
            this._panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "File";
            // 
            // _filename
            // 
            this._filename.Location = new System.Drawing.Point(60, 12);
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
            this._fileopen.Location = new System.Drawing.Point(379, 9);
            this._fileopen.Name = "_fileopen";
            this._fileopen.Size = new System.Drawing.Size(31, 23);
            this._fileopen.TabIndex = 2;
            this._fileopen.Text = "?";
            this._fileopen.UseVisualStyleBackColor = true;
            this._fileopen.Click += new System.EventHandler(this._fileopen_Click);
            // 
            // _run
            // 
            this._run.Location = new System.Drawing.Point(25, 40);
            this._run.Name = "_run";
            this._run.Size = new System.Drawing.Size(385, 23);
            this._run.TabIndex = 3;
            this._run.Text = "Run";
            this._run.UseVisualStyleBackColor = true;
            this._run.Click += new System.EventHandler(this._run_Click);
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
            this._results.Size = new System.Drawing.Size(929, 615);
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
            this._headerpanel.Controls.Add(this._commandBtn);
            this._headerpanel.Controls.Add(this._sendcommand);
            this._headerpanel.Controls.Add(this._com);
            this._headerpanel.Controls.Add(this._break);
            this._headerpanel.Controls.Add(this._executeRun);
            this._headerpanel.Controls.Add(this._singlestep);
            this._headerpanel.Controls.Add(this._executesingleStep);
            this._headerpanel.Controls.Add(this._abort);
            this._headerpanel.Controls.Add(this._filename);
            this._headerpanel.Controls.Add(this.label1);
            this._headerpanel.Controls.Add(this._run);
            this._headerpanel.Controls.Add(this._fileopen);
            this._headerpanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._headerpanel.Location = new System.Drawing.Point(0, 0);
            this._headerpanel.Name = "_headerpanel";
            this._headerpanel.Size = new System.Drawing.Size(929, 100);
            this._headerpanel.TabIndex = 5;
            // 
            // _commandBtn
            // 
            this._commandBtn.Location = new System.Drawing.Point(583, 70);
            this._commandBtn.Name = "_commandBtn";
            this._commandBtn.Size = new System.Drawing.Size(60, 23);
            this._commandBtn.TabIndex = 11;
            this._commandBtn.Text = "Send";
            this._commandBtn.UseVisualStyleBackColor = true;
            this._commandBtn.Click += new System.EventHandler(this._commandBtn_Click);
            // 
            // _sendcommand
            // 
            this._sendcommand.Location = new System.Drawing.Point(416, 73);
            this._sendcommand.Name = "_sendcommand";
            this._sendcommand.Size = new System.Drawing.Size(161, 20);
            this._sendcommand.TabIndex = 10;
            this._sendcommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this._sendcommand_KeyPress);
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
            this._com.Location = new System.Drawing.Point(432, 11);
            this._com.Name = "_com";
            this._com.Size = new System.Drawing.Size(102, 21);
            this._com.TabIndex = 9;
            // 
            // _break
            // 
            this._break.Location = new System.Drawing.Point(745, 71);
            this._break.Name = "_break";
            this._break.Size = new System.Drawing.Size(162, 23);
            this._break.TabIndex = 8;
            this._break.Text = "Break";
            this._break.UseVisualStyleBackColor = true;
            this._break.Click += new System.EventHandler(this._break_Click);
            // 
            // _executeRun
            // 
            this._executeRun.Location = new System.Drawing.Point(745, 41);
            this._executeRun.Name = "_executeRun";
            this._executeRun.Size = new System.Drawing.Size(162, 23);
            this._executeRun.TabIndex = 7;
            this._executeRun.Text = "Execute ...";
            this._executeRun.UseVisualStyleBackColor = true;
            this._executeRun.Click += new System.EventHandler(this._executeRun_Click);
            // 
            // _singlestep
            // 
            this._singlestep.AutoSize = true;
            this._singlestep.Location = new System.Drawing.Point(255, 75);
            this._singlestep.Name = "_singlestep";
            this._singlestep.Size = new System.Drawing.Size(77, 17);
            this._singlestep.TabIndex = 6;
            this._singlestep.Text = "SingleStep";
            this._singlestep.UseVisualStyleBackColor = true;
            // 
            // _executesingleStep
            // 
            this._executesingleStep.Location = new System.Drawing.Point(745, 12);
            this._executesingleStep.Name = "_executesingleStep";
            this._executesingleStep.Size = new System.Drawing.Size(172, 23);
            this._executesingleStep.TabIndex = 5;
            this._executesingleStep.Text = "Execute Step";
            this._executesingleStep.UseVisualStyleBackColor = true;
            this._executesingleStep.Click += new System.EventHandler(this._executesingleStep_Click);
            // 
            // _abort
            // 
            this._abort.Location = new System.Drawing.Point(25, 69);
            this._abort.Name = "_abort";
            this._abort.Size = new System.Drawing.Size(172, 23);
            this._abort.TabIndex = 4;
            this._abort.Text = "Abort";
            this._abort.UseVisualStyleBackColor = true;
            this._abort.Click += new System.EventHandler(this._abort_Click);
            // 
            // _panel
            // 
            this._panel.Controls.Add(this._results);
            this._panel.Cursor = System.Windows.Forms.Cursors.Default;
            this._panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._panel.Location = new System.Drawing.Point(0, 100);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(929, 615);
            this._panel.TabIndex = 6;
            // 
            // _timer
            // 
            this._timer.Enabled = true;
            this._timer.Tick += new System.EventHandler(this._timer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 715);
            this.Controls.Add(this._panel);
            this.Controls.Add(this._headerpanel);
            this.Name = "MainForm";
            this.Text = "Arduino Roboter";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this._headerpanel.ResumeLayout(false);
            this._headerpanel.PerformLayout();
            this._panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _filename;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.Button _fileopen;
        private System.Windows.Forms.Button _run;
        private System.Windows.Forms.ListView _results;
        private System.Windows.Forms.ColumnHeader _command;
        private System.Windows.Forms.ColumnHeader _result;
        private System.Windows.Forms.Panel _headerpanel;
        private System.Windows.Forms.Panel _panel;
        private System.Windows.Forms.Button _abort;
        private System.Windows.Forms.Button _executesingleStep;
        private System.Windows.Forms.CheckBox _singlestep;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.Button _executeRun;
        private System.Windows.Forms.Button _break;
        private System.Windows.Forms.ColumnHeader _step;
        private System.Windows.Forms.ComboBox _com;
        private System.Windows.Forms.Button _commandBtn;
        private System.Windows.Forms.TextBox _sendcommand;
    }
}

