using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
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
        public string DownloadIdentifier { get; set; }

        /// <inheritdoc />
        public event EventHandler<UpdateCheckEventArgs> UpdateCheckCompleted;

        /// <summary>
        /// Creates a new instance of the UpdateChecker class.
        /// </summary>
        /// <param name="url">The location of the file containing update information.</param>
        /// <param name="identifier">An identifier specifying the download option to use.</param>
        public UpdateCheckerBase(string url, string identifier = null)
        {
            this.UpdateCheckUrl = url;
            this.DownloadIdentifier = identifier;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }

        /// <inheritdoc />
        public void CheckForUpdates(UpdateNotifyMode notifyMode = UpdateNotifyMode.NewUpdate)
        {
            Task.Run(() => getUpdateData(UpdateCheckUrl))
                .ContinueWith(async t =>
                {
                    if (t.IsFaulted)
                    {
                        await OnUpdateCheckCompleted(
                            new UpdateCheckEventArgs(false, null, notifyMode, t.Exception.InnerException)
                        );
                    }
                    else if (t.IsCompleted)
                    {
                        await OnUpdateCheckCompleted(new UpdateCheckEventArgs(true, t.Result, notifyMode));
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Raises the UpdateCheckCompleted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual Task OnUpdateCheckCompleted(UpdateCheckEventArgs e)
        {
            UpdateCheckCompleted?.Invoke(this, e);
            return Task.FromResult(false);
        }

        /// <summary>
        /// Resolves the correct download information based on the DownloadIdentifier of the update checker.
        /// </summary>
        public DownloadEntry ResolveDownloadEntry(AppUpdate update)
        {
            DownloadEntry entry = null;
            if (DownloadIdentifier != null)
                entry = update.Downloads?.FirstOrDefault(de => de.Key == DownloadIdentifier);
            // construct a download entry from default values if key was not found.
            if (entry == null)
            {
                entry = new DownloadEntry()
                {
                    Link = update.DownloadLink,
                    FileName = update.DownloadFileName,
                    FileHash = null
                };
            }
            if (entry.FileName == null)
                entry.FileName = getFileName(entry.Link);
            return entry;
        }

        private string getFileName(string link)
        {
            if (Uri.TryCreate(link, UriKind.Absolute, out Uri uri))
                return Path.GetFileName(uri.LocalPath);
            else return Path.GetFileName(link);
        }

        /// <summary>
        /// Downloads the file specified in the given update and verifies the hash sum if available.
        /// </summary>
        /// <param name="update">The update to be downloaded.</param>
        /// <param name="downloadProgress">A download progress provider.</param>
        /// <param name="ct">A CancellationToken used to cancel the download.</param>
        /// <returns>The full path to the downloaded file if the download was successful; null if the download was canceled.</returns>
        /// <exception cref="UpdateFailedException">If downloading the download failed.</exception>
        public async Task<string> DownloadUpdate(AppUpdate update, IProgress<int> downloadProgress = null,
            CancellationToken ct = default(CancellationToken))
        {
            DownloadEntry entry = ResolveDownloadEntry(update);     
            Uri uri = new Uri(entry.Link, UriKind.RelativeOrAbsolute);
            string filePath = Path.Combine(Path.GetTempPath(), entry.FileName);
            try
            {
                if (uri.IsAbsoluteUri)
                {
                    WebClient client = new WebClient();
                    ct.Register(client.CancelAsync);
                    await client.DownloadFileTaskAsync(new Uri(entry.Link, UriKind.RelativeOrAbsolute), filePath, downloadProgress);
                }
                else
                {
                    File.Copy(uri.ToString(), filePath, true);
                }
                Debug.WriteLine(String.Format("Downloaded update to {0}", filePath));
            }
            catch (WebException ex) when (ex.Message == "The request was aborted: The request was canceled.")
            {
                // The download was cancelled by the user. Don't throw an exception.
                return null;
            }
            catch (Exception e)
            {
                throw new UpdateFailedException("Downloading the update failed.", e);
            }
            if (entry.FileHash != null)
            {
                if (VerifyHash(entry.FileHash, filePath))
                    return filePath;
                else
                {
                    File.Delete(filePath);
                    throw new UpdateFailedException("File verification failed.");
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

        /// <summary>
        /// Verifies a given file hash for a given file.
        /// </summary>
        /// <param name="fileHash">The file hash to check.</param>
        /// <param name="fileName">The name of the file to verify.</param>
        protected virtual bool VerifyHash(FileHash fileHash, string fileName)
        {
            // don't check if hash is empty
            if (String.IsNullOrEmpty(fileHash.Hash))
                return true;
            HashAlgorithm hashAlgo = getHashAlgorithm(fileHash.HashAlgorithm);
            using (var stream = File.OpenRead(fileName))
            {
                string hashString = BitConverter.ToString(hashAlgo.ComputeHash(stream)).Replace("-", "");
                return hashString.Equals(fileHash.Hash, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private HashAlgorithm getHashAlgorithm(string name)
        {
            switch (name)
            {
                case "MD5":
                    return MD5.Create();
                case "SHA1":
                    return SHA1.Create();
                case "SHA256":
                    return SHA256.Create();
                default:
                    throw new UpdateFailedException("Unsupported file hash algorithm.");
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
