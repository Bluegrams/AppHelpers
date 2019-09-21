using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides helper methods for application updates.
    /// </summary>
    public class UpdateCheckerBase : IUpdateChecker
    {
        /// <inheritdoc />
        public string UpdateCheckUrl { get; set; }

        /// <inheritdoc />
        public event EventHandler<UpdateCheckEventArgs> UpdateCheckCompleted;

        /// <summary>
        /// Creates a new instance of the UpdateChecker class.
        /// </summary>
        /// <param name="url">The location of the file containing update information.</param>
        public UpdateCheckerBase(string url)
        {
            this.UpdateCheckUrl = url;
        }

        /// <inheritdoc />
        public void CheckForUpdates(UpdateNotifyMode notifyMode = UpdateNotifyMode.NewUpdate)
        {
            Task.Run(() => getUpdateData(UpdateCheckUrl))
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        OnUpdateCheckCompleted(
                            new UpdateCheckEventArgs(false, null, notifyMode, t.Exception.InnerException)
                        );
                    }
                    else if (t.IsCompleted)
                    {
                        OnUpdateCheckCompleted(new UpdateCheckEventArgs(true, t.Result, notifyMode));
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Raises the UpdateCheckCompleted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdateCheckCompleted(UpdateCheckEventArgs e)
        {
            UpdateCheckCompleted?.Invoke(this, e);
        }

        /// <summary>
        /// Downloads the file specified in the given update and verifies the hash sum if available.
        /// </summary>
        /// <param name="update">The update to be downloaded.</param>
        /// <returns>The full path to the downloaded file or null if an error occurred.</returns>
        public async Task<string> DownloadUpdate(AppUpdate update)
        {
            string fileName = update.DownloadFileName ?? Path.GetFileName(update.DownloadLink);
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            try
            {
                WebClient client = new WebClient();
                await client.DownloadFileTaskAsync(update.DownloadLink, filePath);
                Debug.WriteLine(String.Format("Downloaded update to {0}", filePath));
            }
            catch { return null; }
            if (!String.IsNullOrEmpty(update.MD5Hash))
            {
                if (verifyHash(update.MD5Hash, filePath))
                    return filePath;
                else
                {
                    File.Delete(filePath);
                    return null;
                }
            }
            else return filePath;
        }

        /// <summary>
        /// Starts the MSI installer at the given location and exits this application.
        /// </summary>
        /// <param name="installerFile">The MSI installer file.</param>
        /// <param name="passive">If true, only show a minimal UI, otherwise show full UI.</param>
        public void ApplyMsiUpdate(string installerFile, bool passive = true)
        {
            string mode = passive ? "/passive" : "";
            Process proc = new Process();
            proc.StartInfo.FileName = "msiexec.exe";
            proc.StartInfo.Arguments = $"/i \"{installerFile}\" {mode}";
            proc.Start();
            Environment.Exit(0);
        }

        /// <summary>
        /// Shows the downloaded file in explorer.
        /// </summary>
        public void ShowUpdateDownload(string file)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "explorer.exe";
            proc.StartInfo.Arguments = $"/select,\"{file}\"";
            proc.Start();
        }

        private bool verifyHash(string checkHash, string fileName)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    string fileHash = BitConverter.ToString(md5Hash.ComputeHash(stream)).Replace("-", "");
                    return fileHash.Equals(checkHash, StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }

        private AppUpdate getUpdateData(string url)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AppUpdate));
            WebClient client = new WebClient();
            using (Stream stream = client.OpenRead(url))
            {
                return (AppUpdate)serializer.Deserialize(stream);
            }
        }
    }
}
