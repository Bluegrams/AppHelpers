using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Bluegrams.Application.Properties;

namespace Bluegrams.Application.WinForms
{
    /// <summary>
    /// Provides a basic 'About' box showing important app information.
    /// </summary>
    [DesignerCategory("")]
    public class AboutForm : Form
    {
        /// <summary>
        /// Specifies the service used for update checking when clicking the update button.
        /// </summary>
        public IUpdateChecker UpdateChecker { get; set; }

        /// <summary>
        /// A custom color used e.g. for coloring the heading of the window.
        /// </summary>
        public Color AccentColor { get; set; } = Color.DarkGray;

        private Panel panTitle;
        private PictureBox picIcon;
        private TableLayoutPanel tableLayout;
        private GroupBox grpLanguage;
        private ComboBox cmbLang;
        private Button butUpdate, butChangeLang;
        private Label lblTitle;

        /// <summary>
        /// Creates a new instance of the class AboutForm.
        /// </summary>
        /// <param name="icon">Product icon.</param>
        public AboutForm(Bitmap icon = null)
        {
            Initialize();
            this.Load += AboutForm_Load;
            this.KeyPreview = true;
            this.KeyDown += AboutForm_KeyDown;
            if (icon != null)
            {
                picIcon.Image = icon;
                picIcon.BackColor = Color.Transparent;
            }
        }

        protected void Initialize()
        {
            this.SuspendLayout();
            // --- Title ---
            panTitle = new Panel();
            panTitle.SuspendLayout();
            // picIcon
            picIcon = new PictureBox()
            {
                Location = new Point(5, 14),
                Size = new Size(32, 32)
            };
            // lblTitle
            lblTitle = new Label()
            {
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(42, 18),
                Size = new Size(228, 24),
                Text = AppInfo.ProductName
            };
            // panTitle
            panTitle.Location = new Point(5, 0);
            panTitle.Size = new Size(280, 60);
            panTitle.Controls.Add(picIcon);
            panTitle.Controls.Add(lblTitle);
            // --- Table ---
            tableLayout = new TableLayoutPanel();
            tableLayout.SuspendLayout();
            tableLayout.ColumnCount = 2;
            tableLayout.Location = new Point(5, 60);
            tableLayout.Size = new Size(280, 145);
            #region Table content
            // Description
            Label lblDescr = new Label()
            {
                Dock = DockStyle.Fill,
                Text = AppInfo.Description
            };
            tableLayout.Controls.Add(lblDescr, 0, 0);
            tableLayout.SetColumnSpan(lblDescr, 2);
            // Version
            tableLayout.Controls.Add(new Label() { Dock = DockStyle.Fill, Text = Resources.strVersion }, 0, 1);
            Label lblVersion = new Label()
            {
                Dock = DockStyle.Left,
                Text = AppInfo.Version
            };
            butUpdate = new Button()
            {
                Location = new Point(195, 82),
                Size = new Size(85, 23),
                Text = Resources.strUpdate,
            };
            butUpdate.Click += butUpdate_Click;
            tableLayout.Controls.Add(lblVersion, 1, 1);
            // Developer
            tableLayout.Controls.Add(new Label() { Dock = DockStyle.Fill, Text = Resources.strDeveloper }, 0, 2);
            if (AppInfo.CompanyWebsite != null)
            {
                LinkLabel lnkCompany = new LinkLabel()
                {
                    Dock = DockStyle.Fill,
                    Text = AppInfo.CompanyWebsite.Description
                };
                lnkCompany.Links.Add(0, lnkCompany.Text.Length, AppInfo.CompanyWebsite.Url);
                lnkCompany.LinkClicked += lnk_Clicked;
                tableLayout.Controls.Add(lnkCompany, 1, 2);
            }
            else
            {
                tableLayout.Controls.Add(new Label() { Dock = DockStyle.Fill, Text = AppInfo.Company }, 1, 2);
            }
            // Copyright
            Label lblCopyright = new Label()
            {
                Dock = DockStyle.Fill,
                Text = AppInfo.Copyright
            };
            tableLayout.Controls.Add(lblCopyright, 0, 3);
            tableLayout.SetColumnSpan(lblCopyright, 2);
            // License
            if (AppInfo.ProductLicense != null)
            {
                tableLayout.Controls.Add(new Label() { Dock = DockStyle.Fill, Text = Resources.strLicense }, 0, 4);
                LinkLabel lnkLicense = new LinkLabel()
                {
                    Dock = DockStyle.Fill,
                    Text = AppInfo.ProductLicense.Description
                };
                lnkLicense.Links.Add(0, lnkLicense.Text.Length, AppInfo.ProductLicense.Url);
                lnkLicense.LinkClicked += lnk_Clicked;
                tableLayout.Controls.Add(lnkLicense, 1, 4);
            }
            // Website
            LinkLabel lnkWebsite = new LinkLabel()
            {
                Dock = DockStyle.Fill,
                Text = AppInfo.ProductWebsite.Description
            };
            lnkWebsite.Links.Add(0, lnkWebsite.Text.Length, AppInfo.ProductWebsite.Url);
            lnkWebsite.LinkClicked += lnk_Clicked;
            tableLayout.Controls.Add(lnkWebsite, 0, 5);
            tableLayout.SetColumnSpan(lnkWebsite, 2);
            #endregion
            // --- Language ---
            if (AppInfo.SupportedCultures?.Length > 0)
            {
                grpLanguage = new GroupBox();
                grpLanguage.SuspendLayout();
                // cmbLanguage
                cmbLang = new ComboBox()
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Location = new Point(15, 18),
                    Size = new Size(165, 24)
                };
                foreach (CultureInfo cu in AppInfo.SupportedCultures)
                {
                    cmbLang.Items.Add(cu.DisplayName);
                    if (cu.TwoLetterISOLanguageName == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
                        cmbLang.SelectedIndex = cmbLang.Items.Count - 1;
                }
                // butChangeLang
                butChangeLang = new Button()
                {
                    Location = new Point(185, 17),
                    Size = new Size(85, 23),
                    Text = Resources.strRestart
                };
                butChangeLang.Click += butChangeLang_Click;
                // grpLanguage
                grpLanguage.Location = new Point(5, 207);
                grpLanguage.Size = new Size(280, 50);
                grpLanguage.Text = Resources.strAppLanguage;
                grpLanguage.Controls.Add(cmbLang);
                grpLanguage.Controls.Add(butChangeLang);
            }
            // --- AboutForm ---
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(290, grpLanguage != null ? 265 : 215);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = Resources.strAbout + " " + AppInfo.Title;
            this.Controls.Add(panTitle);
            this.Controls.Add(tableLayout);
            if (grpLanguage != null)
                this.Controls.Add(grpLanguage);
            this.Controls.Add(butUpdate);
            butUpdate.BringToFront();
            panTitle.ResumeLayout(false);
            tableLayout.ResumeLayout(false);
            grpLanguage?.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            if (UpdateChecker != null)
                butUpdate.Visible = true;
            else butUpdate.Visible = false;
            if (picIcon.Image == null)
                picIcon.BackColor = this.AccentColor;
            lblTitle.ForeColor = this.AccentColor;
        }

        private void AboutForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void butChangeLang_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.InfoWindow_RestartNewLang, "", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                == DialogResult.OK)
            {
                changeCulture(AppInfo.SupportedCultures[cmbLang.SelectedIndex]);
            }
        }

        private void butUpdate_Click(object sender, EventArgs e)
        {
            UpdateChecker?.CheckForUpdates(UpdateNotifyMode.Always);
        }

        private void lnk_Clicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Link.LinkData.ToString()))
            {
                try
                {
                    Process.Start(e.Link.LinkData.ToString());
                }
                catch { /* Silently fail */ }
            }
        }

        // Note: Needs WindowManager instance for automatic change.
        private void changeCulture(CultureInfo culture)
        {
            Settings.Default.Culture = culture.Name;
            Settings.Default.Save();
            /* Explicitly close the owner form if available to ensure closing events are called:
                https://stackoverflow.com/a/13527298/9145461 */
            this.Owner?.Close();
            System.Windows.Forms.Application.Exit();
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string[] args = new string[Environment.GetCommandLineArgs().Length - 1];
                Array.Copy(Environment.GetCommandLineArgs(), 1, args, 0, args.Length);
                Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location, String.Join(" ", args));
            }
            else Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
        }
    }
}
