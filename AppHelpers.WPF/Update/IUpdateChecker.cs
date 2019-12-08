using System;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides helper methods for application updates.
    /// </summary>
    public interface IUpdateChecker
    {
        /// <summary>
        /// The location of the file containing update information.
        /// </summary>
        string UpdateCheckUrl { get; }
        /// <summary>
        /// An identifier specifying the download option to use.
        /// Falls back to the default option specified by DownloadLink if identifier is not found.
        /// </summary>
        string DownloadIdentifier { get; }
        /// <summary>
        /// This event is raised after a check for updates finished.
        /// </summary>
        event EventHandler<UpdateCheckEventArgs> UpdateCheckCompleted;
        /// <summary>
        /// Checks for update information at the given URL.
        /// </summary>
        /// <param name="notifyMode">Specifies how update notifications should be shown after checking for updates.</param>
        void CheckForUpdates(UpdateNotifyMode notifyMode = UpdateNotifyMode.NewUpdate);
    }
}
