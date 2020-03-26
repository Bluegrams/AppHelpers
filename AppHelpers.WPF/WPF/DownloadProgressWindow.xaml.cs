using System;
using System.Windows;

namespace Bluegrams.Application.WPF
{
    public partial class DownloadProgressWindow : Window
    {
        /// <summary>
        /// The download progress.
        /// </summary>
        public IProgress<int> DownloadProgress { get; }

        /// <summary>
        /// Creates a new instance of DownloadProgressWindow.
        /// </summary>
        public DownloadProgressWindow(AppUpdate update)
        {
            InitializeComponent();
            DownloadProgress = new Progress<int>(v => progDownload.Value = v);
            txtMessage.Text = String.Format(Properties.Resources.DownloadProgressWindow_Text, update.Version);
        }
    }
}
