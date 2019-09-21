using System;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides standard dialog windows.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Displays a window containing application information.
        /// </summary>
        /// <param name="updateChecker">The service used to check for updates.</param>
        void ShowAboutBox(IUpdateChecker updateChecker = null);
        /// <summary>
        /// Displays a confirmation dialog.
        /// </summary>
        /// <param name="message">The message text to be shown.</param>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="confirmText">A custom text to be shown on the confirm button.</param>
        /// <param name="cancelText">A custom text to be shown on the cancel button.</param>
        /// <returns>A boolean value depending on the dialog result.</returns>
        bool ShowConfirmation(string message, string title, string confirmText = null, string cancelText = null);
        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message">The message text to be shown.</param>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="buttonText">A custom text to be shown on the only button on the window.</param>
        void ShowError(string message, string title, string buttonText = null);
        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="error">The exception to be shown.</param>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="buttonText">A custom text to be shown on the only button on the window.</param>
        void ShowError(Exception error, string title, string buttonText = null);
        /// <summary>
        /// Displays an information message.
        /// </summary>
        /// <param name="message">The message text to be shown.</param>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="buttonText">A custom text to be shown on the only button on the window.</param>
        void ShowMessage(string message, string title, string buttonText = null);
        /// <summary>
        /// Displays a simple message box.
        /// </summary>
        /// <param name="message">The message text to be shown.</param>
        /// <param name="title">The title of the message box.</param>
        void ShowMessageBox(string message, string title);
        /// <summary>
        /// Displays a warning message.
        /// </summary>
        /// <param name="message">The message text to be shown.</param>
        /// <param name="title">The title of the dialog window.</param>
        /// <param name="buttonText">A custom text to be shown on the only button on the window.</param>
        void ShowWarning(string message, string title, string buttonText = null);
    }
}
