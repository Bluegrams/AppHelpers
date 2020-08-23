using System;
using System.Windows;
using System.Globalization;
using static Bluegrams.Application.Properties.Resources;

namespace Bluegrams.Application.WPF
{
    public partial class UpdateWindow : Window
    {
        public bool SkipVersion { get; private set; }

        /// <summary>
        /// Creates a new instance of UpdateWindow.
        /// </summary>
        public UpdateWindow(bool hasUpdate, AppUpdate update, bool allowSkip = false)
        {
            InitializeComponent();
            this.DataContext = update;
            txtTitle.Text = String.Format(UpdateWindow_Header, AppInfo.ProductName);
            txtVersion.Text = String.Format(UpdateWindow_Version,
                update.Version, update.ReleaseDate.ToShortDateString(), AppInfo.Version);
            txtWhatsNew.Text = String.Format(UpdateWindow_WhatsNew, update.Version);
            // Fill release notes
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            txtReleaseNotes.Text = update.GetReleaseNotes(langCode);
            if (String.IsNullOrWhiteSpace(txtReleaseNotes.Text))
                stackVersionNotes.Visibility = Visibility.Collapsed;
            else stackVersionNotes.Visibility = Visibility.Visible;
            // skip button
            if (!allowSkip) butSkip.Visibility = Visibility.Collapsed;
            else butSkip.Visibility = Visibility.Visible;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.SkipVersion = true;
            this.Close();
        }
    }
}
