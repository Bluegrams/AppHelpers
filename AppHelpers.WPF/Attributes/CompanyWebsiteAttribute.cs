using System;

namespace Bluegrams.Application.Attributes
{
    /// <summary>
    /// Specifies the website of the developer company.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CompanyWebsiteAttribute : Attribute
    {
        /// <summary>
        /// The link to the company website.
        /// </summary>
        public Link WebsiteLink { get; }

        /// <summary>
        /// Creates a new instance of the CompanyWebsiteAttribute class.
        /// </summary>
        public CompanyWebsiteAttribute(string url, string displayText)
        {
            WebsiteLink = new Link(url, displayText);
        }
    }
}
