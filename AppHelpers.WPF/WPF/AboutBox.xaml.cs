using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Bluegrams.Application.WPF
{
    /// <summary>
    /// Provides a basic 'About' box showing important app information.
    /// </summary>
    public partial class AboutBox : Window
    {
        private readonly bool showLanguageSelection;

        /// <summary>
        /// Specifies the service used for update checking when clicking the update button.
        /// </summary>
        public IUpdateChecker UpdateChecker { get; set; }

        /// <summary>
        /// A custom color used e.g. for coloring the heading of the window.
        /// </summary>
        public Color AccentColor { get; set; } = Colors.DarkGray;

        /// <summary>
        /// Raised when the user changes the UI language of the app.
        /// </summary>
        public event EventHandler<ChangeCultureEventArgs> CultureChanging;

        /// <summary>
        /// Creates a new instance of the class AboutBox.
        /// </summary>
        /// <param name="icon">Product icon.</param>
        public AboutBox(ImageSource icon = null, bool showLanguageSelection = true)
        {
            this.DataContext = this;
            this.showLanguageSelection = showLanguageSelection;
            InitializeComponent();
            if (icon != null)
            {
                this.imgIcon.Source = icon;
                this.Icon = icon;
                brdIcon.Background = new SolidColorBrush(Colors.Transparent);
            }
            this.Title = Properties.Resources.strAbout + " " + Title;
            // show available company information
            if (AppInfo.CompanyWebsite != null)
                lblCompanyWebsite.Visibility = Visibility.Visible;
            else lblCompany.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (showLanguageSelection)
            {
                ICollection<CultureInfo> supportedCultures;
                if (AppInfo.SupportedCultures != null)
                    supportedCultures = AppInfo.SupportedCultures;
                else supportedCultures = AppInfo.GetSupportedCultures();
                bool exactMatch = false;
                foreach (CultureInfo cu in supportedCultures.OrderBy(c => c.Name))
                {
                    comLanguages.Items.Add(cu);
                    if (exactMatch) continue;
                    if (cu.Name == CultureInfo.CurrentUICulture.Name)
                    {
                        comLanguages.SelectedIndex = comLanguages.Items.Count - 1;
                        exactMatch = true;
                    }
                    else if (cu.TwoLetterISOLanguageName == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
                    {
                        comLanguages.SelectedIndex = comLanguages.Items.Count - 1;
                    }
                }
            }
            if (comLanguages.Items.Count < 1)
            {
                stackLang.Visibility = Visibility.Collapsed;
            }
            if (UpdateChecker != null)
                butUpdate.Visibility = Visibility.Visible;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Uri.OriginalString))
            {
                try
                {
                    Process.Start(e.Uri.OriginalString);
                }
                catch { /* Silently fail */ }
            }
        }

        private void butRestart_Click(object sender, RoutedEventArgs e)
        {
            changeCulture(comLanguages.SelectedItem as CultureInfo);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void butUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateChecker.CheckForUpdates(UpdateNotifyMode.Always);
        }

        // Note: Needs WindowManager instance for automatic change.
        private void changeCulture(CultureInfo culture)
        {
            if (culture == null) return;
            ChangeCultureEventArgs e = new ChangeCultureEventArgs(culture);
            CultureChanging?.Invoke(this, e);
            if (e.Success)
            {
                Properties.Settings.Default.Culture = culture.Name;
                Properties.Settings.Default.Save();
                this.Close();
                return;
            }
            if (!e.SuppressDefault)
            {
                if (MessageBox.Show(Properties.Resources.InfoWindow_RestartNewLang, "", MessageBoxButton.OKCancel, MessageBoxImage.Warning)
                    == MessageBoxResult.OK)
                {
                    Properties.Settings.Default.Culture = culture.Name;
                    Properties.Settings.Default.Save();
                    System.Windows.Application.Current.Shutdown();
                    if (Environment.GetCommandLineArgs().Length > 1)
                    {
                        string[] args = new string[Environment.GetCommandLineArgs().Length - 1];
                        Array.Copy(Environment.GetCommandLineArgs(), 1, args, 0, args.Length);
                        Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location, String.Join(" ", args));
                    }
                    else Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
                }
            }
        }
    }
}
