using System;

#pragma warning disable 1591

namespace Bluegrams.Application
{
    /// <summary>
    /// Represents the possible options for update notifications.
    /// </summary>
    public enum UpdateNotifyMode
    {
        NewUpdate = 0,
        Always = 1,
        NewUncheckedUpdate = 2,
        Never = 3,
        Auto = 4
    }
}
