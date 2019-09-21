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
        private readonly Form parent;
        private readonly CustomSettings settings;

        /// <summary>
        /// Creates a new instance of the WinFormsUpdateChecker class.
        /// </summary>
        /// <param name="url">The location of the file containing update information.</param>
        /// <param name="parent">The parent window of the update window to show.</param>
        public WinFormsUpdateChecker(string url, Form parent = null) : base(url)
        {
            this.parent = parent;
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
                updateWindow.Owner = parent;
                if (updateWindow.ShowDialog() == DialogResult.OK)
                {
                    string path = Task.Run(async () => await DownloadUpdate(e.Update)).Result;
                    if (path != null)
                    {
                        if (System.IO.Path.GetExtension(path) == ".msi")
                            ApplyMsiUpdate(path);
                        else ShowUpdateDownload(path);
                    }
                    else
                    {
                        MessageBox.Show(parent, Resources.Box_UpdateFailed, Resources.Box_UpdateFailed_Title,
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else if (e.UpdateNotifyMode == UpdateNotifyMode.Always)
            {
                MessageBox.Show(parent, Resources.Box_NoNewUpdate, Resources.strSoftwareUpdate);
            }
        }
    }
}
