﻿using System;
using System.IO;

namespace Bluegrams.Application.Attributes
{
    /// <summary>
    /// Specifies the license under which the product is published.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ProductLicenseAttribute : Attribute
    {
        /// <summary>
        /// The link to the product license.
        /// </summary>
        public Link LicenseLink { get; }

        /// <summary>
        /// Creates a new instance of the ProductLicenseAttribute class.
        /// </summary>
        public ProductLicenseAttribute(string url, string displayText)
        {
            if (!isAbsolutePath(url))
                url = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, url);
            LicenseLink = new Link(url, displayText);
        }

        private static bool isAbsolutePath(string url)
        {
            Uri uri;
            return Uri.TryCreate(url, UriKind.Absolute, out uri);
        }
    }
}
