using System;
using System.Globalization;

namespace Bluegrams.Application
{
    /// <summary>
    /// Holds event data for the CultureChanging event in the 'About' window.
    /// </summary>
    public class ChangeCultureEventArgs : EventArgs
    {
        /// <summary>
        /// The new UI culture to set.
        /// </summary>
        public CultureInfo NewCulture { get; }

        /// <summary>
        /// Indicates that the culture was changed successfully. Also prevents the default behaviour from executing.
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Indicates that the default culture changing behaviours should not be executed.
        /// </summary>
        public bool SuppressDefault { get; set; } = false;

        /// <summary>
        /// Creates new event data for the CultureChanging event in the 'About' window.
        /// </summary>
        /// <param name="newCulture">Thw new UI culture to set.</param>
        public ChangeCultureEventArgs(CultureInfo newCulture)
        {
            this.NewCulture = newCulture;
        }
    }
}
