using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Bluegrams.Application.Properties;

namespace Bluegrams.Application.WinForms
{
    [DesignerCategory("")]
    public class DownloadProgressForm : Form
    {
        private AppUpdate update;
        private CancellationTokenSource cancellationTokenSource;

        private ProgressBar progDownload;
        private Label lblMessage;
        private Button butCancel;

        public IProgress<int> DownloadProgress { get; }

        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        /// <summary>
        /// Creates a new instance of the class UpdateForm.
        /// </summary>
        public DownloadProgressForm(AppUpdate update)
        {
            this.update = update;
            Initialize();
            DownloadProgress = new Progress<int>(v => progDownload.Value = v);
            cancellationTokenSource = new CancellationTokenSource();
        }

        protected void Initialize()
        {
            // --- Controls ---
            progDownload = new ProgressBar()
            {
                Location = new Point(5, 10),
                Size = new Size(280, 30)
            };
            lblMessage = new Label()
            {
                Location = new Point(5, 50),
                Size = new Size(280, 20),
                Text = String.Format(Resources.DownloadProgressWindow_Text, update?.Version)
            };
            butCancel = new Button()
            {
                Location = new Point(195, 80),
                Size = new Size(90, 22),
                Text = Resources.strCancel
            };
            butCancel.Click += ButCancel_Click;
            // --- Form ---
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(290, 110);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = Resources.DownloadProgressWindow_Header;
            this.Controls.Add(progDownload);
            this.Controls.Add(lblMessage);
            this.Controls.Add(butCancel);
            this.ResumeLayout(false);
            this.FormClosing += DownloadProgressForm_FormClosing;
        }

        private void ButCancel_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
            this.Close();
        }

        private void DownloadProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
