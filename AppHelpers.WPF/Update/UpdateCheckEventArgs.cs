﻿using System;

namespace Bluegrams.Application
{
    /// <summary>
    /// Contains the event data for an update check.
    /// </summary>
    public class UpdateCheckEventArgs : EventArgs
    {
        /// <summary>
        /// True if check for updates was successful, false if no Update object could be retrieved.
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        /// True if the version of the retrieved updated is higher than the current app version, false otherwise.
        /// </summary>
        public bool NewVersion { get; }

        /// <summary>
        /// The retrieved update information.
        /// </summary>
        public AppUpdate Update { get; }

        /// <summary>
        /// The requested notification verbosity for showing update information.
        /// </summary>
        public UpdateNotifyMode UpdateNotifyMode { get; }

        /// <summary>
        /// Holds the exception that caused the failure if the update check was not successful.
        /// </summary>
        public Exception UpdateCheckException { get; }

        /// <summary>
        /// Creates a new instance of type UpdateCheckEventArgs.
        /// </summary>
        public UpdateCheckEventArgs(bool success, AppUpdate update, UpdateNotifyMode notifyMode, Exception ex = null)
        {
            Successful = success;
            Update = update;
            if (update != null && ex == null)
                NewVersion = new Version(update.Version) > new Version(AppInfo.Version);
            UpdateNotifyMode = notifyMode;
            UpdateCheckException = ex;
        }
    }
}
