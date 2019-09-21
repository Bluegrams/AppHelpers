using System;
using System.Threading.Tasks;
using System.Windows;
using Bluegrams.Application.Properties;
using Bluegrams.Application.WPF;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides helper methods for application updates for WPF.
    /// </summary>
    public class WpfUpdateChecker : UpdateCheckerBase
    {
        private readonly Window parent;
        private readonly CustomSettings settings;

        /// <summary>
        /// Creates a new instance of the WpfUpdateChecker class.
        /// </summary>
        /// <param name="url">The location of the file containing update information.</param>
        /// <param name="parent">The parent window of the update window to show.</param>
        public WpfUpdateChecker(string url, Window parent = null) : base(url)
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
                UpdateWindow updateWindow = new UpdateWindow(e.NewVersion, e.Update);
                updateWindow.Owner = parent;
                if (updateWindow.ShowDialog().GetValueOrDefault())
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
                            MessageBoxButton.OK, MessageBoxImage.Error);
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
