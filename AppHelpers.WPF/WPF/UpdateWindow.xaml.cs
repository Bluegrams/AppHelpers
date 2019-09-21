using System;
using System.Windows;
using static Bluegrams.Application.Properties.Resources;

namespace Bluegrams.Application.WPF
{
    public partial class UpdateWindow : Window
    {
        /// <summary>
        /// Creates a new instance of UpdateWindow.
        /// </summary>
        public UpdateWindow(bool hasUpdate, AppUpdate update)
        {
            InitializeComponent();
            this.DataContext = update;
            txtTitle.Text = String.Format(UpdateWindow_Header, AppInfo.ProductName);
            txtVersion.Text = String.Format(UpdateWindow_Version,
                update.Version, update.ReleaseDate.ToShortDateString(), AppInfo.Version);
            txtWhatsNew.Text = String.Format(UpdateWindow_WhatsNew, update.Version);
            if (String.IsNullOrWhiteSpace(update.VersionNotes))
                stackVersionNotes.Visibility = Visibility.Collapsed;
            else stackVersionNotes.Visibility = Visibility.Visible;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
