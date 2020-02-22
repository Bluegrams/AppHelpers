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
// Specifiy a list of cultures the application explicitly supports to fill a combo box 
// that allows switching between these. If this property is not specified, 
// the combo box won't be visible on the 'About' box.
[assembly: SupportedCultures("de", "en", "es")]

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
