using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Bluegrams.Application.WPF
{
    /// <summary>
    /// A simple dialog service using WPF message boxes.
    /// </summary>
    public class SimpleDialogService : IDialogService
    {
        protected Color AccentColor = Colors.DarkGray;

        /// <inheritdoc />
        public virtual void ShowAboutBox(IUpdateChecker updateChecker = null)
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.UpdateChecker = updateChecker;
            aboutBox.AccentColor = AccentColor;
            aboutBox.Owner = System.Windows.Application.Current.Windows
                                   .OfType<Window>().SingleOrDefault(x => x.IsActive);
            aboutBox.ShowDialog();
        }

        /// <inheritdoc />
        public virtual bool ShowConfirmation(string message, string title, string confirmText = null, string cancelText = null)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        /// <inheritdoc />
        public virtual void ShowError(string message, string title, string buttonText = null)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <inheritdoc />
        public virtual void ShowError(Exception error, string title, string buttonText = null)
        {
            MessageBox.Show(error.ToString(), title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <inheritdoc />
        public virtual void ShowMessage(string message, string title, string buttonText = null)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <inheritdoc />
        public virtual void ShowMessageBox(string message, string title)
        {
            MessageBox.Show(message, title);
        }

        /// <inheritdoc />
        public virtual void ShowWarning(string message, string title, string buttonText = null)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
