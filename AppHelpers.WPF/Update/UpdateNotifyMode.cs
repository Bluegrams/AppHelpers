using System;

namespace Bluegrams.Application
{
    /// <summary>
    /// Represents the possible options for update notifications.
    /// </summary>
    public enum UpdateNotifyMode
    {
        /// <summary>
        /// Show a notification if a new update is available.
        /// </summary>
        NewUpdate = 0,
        /// <summary>
        /// Always show a notification.
        /// </summary>
        Always = 1,
        /// <summary>
        /// Show a notification if a new update is available that has not been shown to the user before.
        /// </summary>
        NewUncheckedUpdate = 2,
        /// <summary>
        /// Never show a notification.
        /// </summary>
        Never = 3,
        /// <summary>
        /// Show a notification if a new update is available and allow the user to skip notifications for the new version.
        /// </summary>
        Auto = 4
    }
}
