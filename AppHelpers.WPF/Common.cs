using System;
using System.Globalization;

namespace Bluegrams.Application
{
    public class ChangeCultureEventArgs : EventArgs
    {
        public CultureInfo NewCulture { get; }

        public bool SuppressDefault { get; set; } = false;

        public ChangeCultureEventArgs(CultureInfo newCulture)
        {
            this.NewCulture = newCulture;
        }
    }
}
