using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bluegrams.Application.Properties;
using Bluegrams.Application.WinForms;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides helper methods for application updates for WinForms.
    /// </summary>
    public class WinFormsUpdateChecker : UpdateCheckerBase
    {
        private readonly CustomSettings settings;

        /// <summary>
        /// Gets or sets the owning form of all update windows.
        /// </summary>
        public Form Owner { get; set; }

        /// <summary>
        /// Creates a new instance of the WinFormsUpdateChecker class.
        /// </summary>
        /// <param name="url">The location of the file containing update information.</param>
        /// <param name="owner">The parent window of the update window to show.</param>
        /// <param name="identifier">An identifier specifying the download option to use.</param>
        public WinFormsUpdateChecker(string url, Form owner = null, string identifier = null) : base(url, identifier)
        {
            this.Owner = owner;
            settings = new CustomSettings(AppInfo.IsPortable.GetValueOrDefault(), typeof(UpdateCheckerBase).FullName);
            settings.AddSetting("CheckedUpdate", typeof(string), "0.0", true, System.Configuration.SettingsSerializeAs.String);
        }

        /// <inheritdoc />
        protected override void OnUpdateCheckCompleted(UpdateCheckEventArgs e)
        {
            base.OnUpdateCheckCompleted(e);
            bool isNewer = e.Successful && new Version(e.Update.Version) > new Version((string)settings["CheckedUpdate"]);
            if (e.NewVersion)
            {
                settings["CheckedUpdate"] = e.Update.Version;
                settings.Save();
            }
            if (e.UpdateNotifyMode == UpdateNotifyMode.Never
                || e.UpdateNotifyMode == UpdateNotifyMode.NewUncheckedUpdate && !isNewer)
                return;
            else if (e.Successful && e.NewVersion)
            {
                UpdateForm updateWindow = new UpdateForm(e.NewVersion, e.Update);
                updateWindow.Owner = this.Owner;
                if (updateWindow.ShowDialog() == DialogResult.OK)
                {
                    try
                    { 
                        string path = Task.Run(async () => await DownloadUpdate(e.Update)).Result;
                        if (System.IO.Path.GetExtension(path) == ".msi")
                            ApplyMsiUpdate(path);
                        else ShowUpdateDownload(path);
                    }
                    catch (UpdateFailedException)
                    {
                        MessageBox.Show(this.Owner, Resources.Box_UpdateFailed, Resources.Box_UpdateFailed_Title,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (e.UpdateNotifyMode == UpdateNotifyMode.Always)
            {
                MessageBox.Show(this.Owner, Resources.Box_NoNewUpdate, Resources.strSoftwareUpdate);
            }
        }
    }
}
