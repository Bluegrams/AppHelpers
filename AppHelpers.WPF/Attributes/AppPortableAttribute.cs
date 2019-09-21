using System;

namespace Bluegrams.Application.Attributes
{
    /// <summary>
    /// Specifies whether an application should use portable or local settings.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AppPortableAttribute : Attribute
    {
        /// <summary>
        /// True if the application is intended to be portable.
        /// </summary>
        public bool IsPortable { get; }

        /// <summary>
        /// Creates a new instance of the class AppPortableAttribute.
        /// </summary>
        public AppPortableAttribute(bool isPortable)
        {
            IsPortable = isPortable;
        }
    }
}
