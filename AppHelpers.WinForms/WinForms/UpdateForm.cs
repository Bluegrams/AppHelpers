using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Bluegrams.Application.Properties;

namespace Bluegrams.Application.WinForms
{
    [DesignerCategory("")]
    public class UpdateForm : Form
    {
        private AppUpdate update;
        private Panel panTitle, panNotes, panBottom;

        /// <summary>
        /// Creates a new instance of the class UpdateForm.
        /// </summary>
        public UpdateForm(bool hasUpdate, AppUpdate update)
        {
            this.update = update;
            Initialize();
        }

        protected void Initialize()
        {
            // --- Title ---
            panTitle = new Panel();
            panTitle.SuspendLayout();
            // lblTitle
            Label lblTitle = new Label()
            {
                Font = new Font(this.Font, FontStyle.Bold),
                Location = new Point(5, 5),
                Size = new Size(280, 35),
                Text = String.Format(Resources.UpdateWindow_Header, AppInfo.ProductName)
            };
            // lblVersion
            Label lblVersion = new Label()
            {
                Location = new Point(5, 40),
                Size = new Size(280, 45),
                Text = String.Format(Resources.UpdateWindow_Version,
                        update.Version, update.ReleaseDate.ToShortDateString(), AppInfo.Version)
            };
            // panTitle
            panTitle.Location = new Point(0, 0);
            panTitle.Size = new Size(290, 85);
            panTitle.Controls.Add(lblTitle);
            panTitle.Controls.Add(lblVersion);
            // --- Version Notes ---
            if (!String.IsNullOrWhiteSpace(update.VersionNotes))
            {
                panNotes = new Panel();
                panNotes.SuspendLayout();
                // lblWhatsNew
                Label lblWhatsNew = new Label()
                {
                    Font = new Font(this.Font, FontStyle.Bold),
                    Location = new Point(5, 0),
                    Size = new Size(280, 20),
                    Text = String.Format(Resources.UpdateWindow_WhatsNew, update.Version)
                };
                TextBox txtVersionNotes = new TextBox()
                {
                    Location = new Point(5, 20),
                    Multiline = true,
                    ReadOnly = true,
                    Size = new Size(280, 100),
                    Text = update.VersionNotes
                };
                // panNotes
                panNotes.Location = new Point(0, 85);
                panNotes.Size = new Size(290, 120);
                panNotes.Controls.Add(lblWhatsNew);
                panNotes.Controls.Add(txtVersionNotes);
            }
            // --- Buttons ---
            panBottom = new Panel();
            panBottom.SuspendLayout();
            // butSubmit
            Button butSubmit = new Button()
            {
                Location = new Point(100, 9),
                Size = new Size(80, 22),
                Text = Resources.strUpdate
            };
            butSubmit.Click += butSubmit_Click;
            // butRemindLater
            Button butRemindLater = new Button()
            {
                Location = new Point(185, 9),
                Size = new Size(100, 22),
                Text = Resources.strRemindLater
            };
            // panBottom
            panBottom.Location = new Point(0, panNotes != null ? 205 : 85);
            panBottom.Size = new Size(290, 40);
            panBottom.Controls.Add(butSubmit);
            panBottom.Controls.Add(butRemindLater);
            // --- AboutForm ---
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AcceptButton = butSubmit;
            this.CancelButton = butRemindLater;
            this.ClientSize = new Size(290, panNotes != null ? 245 : 125);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = Resources.strSoftwareUpdate;
            this.Controls.Add(panTitle);
            if (panNotes != null)
                this.Controls.Add(panNotes);
            this.Controls.Add(panBottom);
            panTitle.ResumeLayout(false);
            panNotes?.ResumeLayout(false);
            panBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void butSubmit_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
