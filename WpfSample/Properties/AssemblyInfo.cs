using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using Bluegrams.Application.Attributes;

// Specifies if the manager should be run in portable mode.
[assembly: AppPortable(true)]

// Additional assembly attributes used to fill the 'About' box.
[assembly: ProductWebsite("http://example.org", "Example.org")]
[assembly: ProductLicense("https://opensource.org/licenses/BSD-3-Clause", "BSD-3-Clause License")]
[assembly: CompanyWebsite("http://example.org", "Example Inc.")]

// General Information about an assembly is controlled through the following
// set of attributes.
[assembly: AssemblyTitle("WpfSample")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Example Inc.")]
[assembly: AssemblyProduct("WpfSample")]
[assembly: AssemblyCopyright("Copyright © 2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]
[assembly: ThemeInfo(
    ResourceDictionaryLocation.None,
    ResourceDictionaryLocation.SourceAssembly
)]

[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]

// Sets the neutral language of the app.
// This attribute is important for the language selector of the about box.
[assembly: NeutralResourcesLanguage("en")]
