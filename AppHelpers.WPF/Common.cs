using System;
using System.Globalization;

namespace Bluegrams.Application
{
    public class ChangeCultureEventArgs : EventArgs
    {
        public CultureInfo NewCulture { get; }

        /// <summary>
        /// Indicates that the culture was changed successfully. Also prevents the default behaviour from executing.
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Indicates that the default culture changing behaviours should not be executed.
        /// </summary>
        public bool SuppressDefault { get; set; } = false;

        public ChangeCultureEventArgs(CultureInfo newCulture)
        {
            this.NewCulture = newCulture;
        }
    }
}
