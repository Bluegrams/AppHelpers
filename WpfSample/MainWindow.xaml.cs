using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using Bluegrams.Application;
using Bluegrams.Application.WPF;

namespace WpfSample
{
    /// <summary>
    /// An example app using AppHelpers for WPF.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string updateCheckUrl = "https://raw.githubusercontent.com/bluegrams/apphelpers/master/WpfSample/AppUpdateExample.xml";

        // Define new variables for the settings manager and the update checker.
        WpfWindowManager manager;
        IUpdateChecker updateChecker;

        public int OpenCount { get; set; }

        public MainWindow()
        {
            // Create new instances of WindowManager and UpdateChecker for WPF.
            manager = new WpfWindowManager(this);
            updateChecker = new WpfUpdateChecker(updateCheckUrl, this);

            // --- EXAMPLE 1/2: Persist Window State/ Make Settings Portable ---

            manager.ManageDefault();

            // Add any public property of the window with this method to let its value
            // be saved when the application is closed and loaded when it starts.
            manager.Manage(nameof(this.OpenCount), defaultValue: -1);

            // Initialize the manager. Please make sure this method is called BEFORE the window is initialized.
            manager.Initialize();

            //Initialize the window.
            InitializeComponent();
        }


        private void butAbout_Click(object sender, RoutedEventArgs e)
        {
            // --- EXAMPLE 3: Application 'About' Box ---
            // Load the icon used in the 'About' box.
            var baseUri = BaseUriHelper.GetBaseUri(this);
            BitmapSource icon = new BitmapImage(new Uri(baseUri, @"/bluelogo.png"));
            // Show the 'About' box.
            // The shown data is specified as assembly attributes in AssemblyInfo.cs.
            var aboutBox = new AboutBox(icon);
            aboutBox.Owner = this;
            aboutBox.UpdateChecker = updateChecker;
            aboutBox.ShowDialog();
            OpenCount++;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            loadSettings();
            // --- EXAMPLE 4: Check for App Updates ---
            // This event is fired when update checking has finished.
            updateChecker.UpdateCheckCompleted += delegate (object s, UpdateCheckEventArgs args)
            {
                Debug.WriteLine("Update check completed.");
                if (!args.Successful)
                {
                    Debug.WriteLine("Update check failed!");
                }
                else Debug.WriteLine($"Found version: {args.Update.Version}.");
            };
            updateChecker.CheckForUpdates();
        }

        private void loadSettings()
        {
            lblLang.Content = Properties.Resources.Text;
            lblCount.Content = OpenCount;
        }

        private void butCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            updateChecker.CheckForUpdates(UpdateNotifyMode.Always);
        }
    }
}
