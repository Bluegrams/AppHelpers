using System;
using System.Threading;
using System.Windows;

namespace Bluegrams.Application.WPF
{
    public partial class DownloadProgressWindow : Window
    {
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// The download progress.
        /// </summary>
        public IProgress<int> DownloadProgress { get; }

        /// <summary>
        /// Cancellation token used for requests to cancel the download.
        /// </summary>
        public CancellationToken CancellationToken => cancellationTokenSource.Token;

        /// <summary>
        /// Creates a new instance of DownloadProgressWindow.
        /// </summary>
        public DownloadProgressWindow(AppUpdate update)
        {
            InitializeComponent();
            DownloadProgress = new Progress<int>(v => progDownload.Value = v);
            cancellationTokenSource = new CancellationTokenSource();
            txtMessage.Text = String.Format(Properties.Resources.DownloadProgressWindow_Text, update.Version);
        }

        private void Cancel_Clicked(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
