using System;
using System.Globalization;

namespace Bluegrams.Application.Attributes
{
    /// <summary>
    /// Explicitly specifies cultures supported by this assembly.
    /// (Used in 'About' box and WindowManager.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class SupportedCulturesAttribute : Attribute
    {
        /// <summary>
        /// An array of all explicitly supported cultures.
        /// </summary>
        public CultureInfo[] SupportedCultures { get; }

        /// <summary>
        /// Creates a new instance of the SupportedCulturesAttribute class.
        /// </summary>
        public SupportedCulturesAttribute(params string[] cultures)
        {
            SupportedCultures = new CultureInfo[cultures.Length];
            for (int i = 0; i < cultures.Length; i++)
            {
                SupportedCultures[i] = new CultureInfo(cultures[i]);
            }
        }
    }
}
