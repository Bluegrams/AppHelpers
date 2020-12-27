using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using Bluegrams.Application.Attributes;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides assembly information.
    /// </summary>
    public static class AppInfo
    {
        /// <summary>
        /// The product name of the assembly.
        /// </summary>
        public static string ProductName
        {
            get
            {
                return ((AssemblyProductAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AssemblyProductAttribute)))?.Product;
            }
        }

        /// <summary>
        /// The title of the assembly.
        /// </summary>
        public static string Title
        {
            get
            {
                return ((AssemblyTitleAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AssemblyTitleAttribute)))?.Title;
            }
        }

        /// <summary>
        /// The company that published the assembly.
        /// </summary>
        public static string Company
        {
            get
            {
                return ((AssemblyCompanyAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AssemblyCompanyAttribute)))?.Company;
            }
        }

        /// <summary>
        /// The assembly version.
        /// </summary>
        public static string Version
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        /// <summary>
        /// The copyright notice.
        /// </summary>
        public static string Copyright
        {
            get
            {
                return ((AssemblyCopyrightAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AssemblyCopyrightAttribute)))?.Copyright;
            }
        }

        /// <summary>
        /// Whether or not <see cref="Copyright"/> has a value.
        /// </summary>
        public static bool HasCopyright
        {
            get
            {
                return !string.IsNullOrEmpty(Copyright);
            }
        }

        /// <summary>
        /// The assembly description.
        /// </summary>
        public static string Description
        {
            get
            {
                return ((AssemblyDescriptionAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AssemblyDescriptionAttribute)))?.Description;
            }
        }

        /// <summary>
        /// The name of the app's default culture, as specified with NeutralResourcesLanguageAttribute.
        /// </summary>
        public static string NeutralResourcesLanguage
        {
            get
            {
                return ((NeutralResourcesLanguageAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(NeutralResourcesLanguageAttribute)))?.CultureName;
            }
        }

        /// <summary>
        /// The path to the main executable of the assembly.
        /// </summary>
        public static string Location
        {
            get
            {
                return Assembly.GetEntryAssembly().Location;
            }
        }

        /// <summary>
        /// Returns a value specifying whether an application should use portable or local settings.
        /// </summary>
        public static bool? IsPortable
        {
            get
            {
                return ((AppPortableAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(AppPortableAttribute)))?.IsPortable;
            }
        }

        /// <summary>
        /// The product website of the assembly.
        /// </summary>
        public static Link ProductWebsite
        {
            get
            {
                return ((ProductWebsiteAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(ProductWebsiteAttribute)))?.WebsiteLink;
            }
        }

        /// <summary>
        /// The license under which the product is published.
        /// </summary>
        public static Link ProductLicense
        {
            get
            {
                return ((ProductLicenseAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(ProductLicenseAttribute)))?.LicenseLink;
            }
        }

        /// <summary>
        /// Whether or not the <see cref="ProductLicense"/> has a value.
        /// </summary>
        public static bool HasProductLicense
        {
            get
            {
                return ProductLicense != null;
            }
        }

        /// <summary>
        /// The website of the developer company.
        /// </summary>
        public static Link CompanyWebsite
        {
            get
            {
                return ((CompanyWebsiteAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(CompanyWebsiteAttribute)))?.WebsiteLink;
            }
        }

        /// <summary>
        /// The explicitly supported cultures of the assembly.
        /// </summary>
        public static CultureInfo[] SupportedCultures
        {
            get
            {
                return ((SupportedCulturesAttribute)Assembly.GetEntryAssembly().GetCustomAttribute(typeof(SupportedCulturesAttribute)))?.SupportedCultures;
            }
        }

        /// <summary>
        /// Gets a list of all languages supported by this app by searching for available satellite assemblies and inspecting NeutralResourcesLanguage.
        /// </summary>
        /// <returns>A collection of culture infos, representing the supported languages.</returns>
        public static ICollection<CultureInfo> GetSupportedCultures()
        {
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            string exeDir = Path.GetDirectoryName(Location);
            List<CultureInfo> supported = cultures
                .Where(c => !c.Equals(CultureInfo.InvariantCulture) && Directory.Exists(Path.Combine(exeDir, c.Name))).ToList();
            if (!String.IsNullOrEmpty(NeutralResourcesLanguage))
            {
                CultureInfo defaultCulture = new CultureInfo(NeutralResourcesLanguage);
                if (!supported.Contains(defaultCulture))
                    supported.Add(defaultCulture);
            }
            return supported;
        }
    }
}
