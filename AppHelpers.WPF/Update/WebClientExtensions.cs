using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides extensions for WebClient.
    /// </summary>
    public static class WebClientExtensios
    {
        /// <summary>
        /// Downloads the specified resource to a local file as an asynchronous operation using a task object.
        /// Adapted from https://stackoverflow.com/a/35282714.
        /// </summary>
        /// <param name="webClient">The web client.</param>
        /// <param name="address">The URI of the resource to download.</param>
        /// <param name="fileName">The name of the file to be placed on the local computer.</param>
        /// <param name="progress">The download progress provider.</param>
        /// <returns></returns>
        public static async Task DownloadFileTaskAsync(
            this WebClient webClient,
            Uri address,
            string fileName,
            IProgress<int> progress = null)
        {
            // Create the task to be returned
            var tcs = new TaskCompletionSource<object>(address);
            // Setup the callback event handler handlers
            AsyncCompletedEventHandler completedHandler = (cs, ce) =>
            {
                if (ce.UserState == tcs)
                {
                    if (ce.Error != null) tcs.TrySetException(ce.Error);
                    else if (ce.Cancelled) tcs.TrySetCanceled();
                    else tcs.TrySetResult(null);
                }
            };
            DownloadProgressChangedEventHandler progressChangedHandler = (ps, pe) =>
            {
                if (pe.UserState == tcs)
                {
                    progress?.Report(pe.ProgressPercentage);
                }
            };
            try
            {
                webClient.DownloadFileCompleted += completedHandler;
                webClient.DownloadProgressChanged += progressChangedHandler;
                webClient.DownloadFileAsync(address, fileName, tcs);
                await tcs.Task;
            }
            finally
            {
                webClient.DownloadFileCompleted -= completedHandler;
                webClient.DownloadProgressChanged -= progressChangedHandler;
            }
        }
    }
}
