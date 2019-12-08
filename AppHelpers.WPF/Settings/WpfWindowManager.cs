using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides a WindowManager implementation managing settings such as size and location for a WPF application.
    /// </summary>
    public class WpfWindowManager : WindowManager<Window>
    {
        private const double DEFAULT = double.NaN;

        private bool manageDefault, sizeable;
        private double savedWidth, savedHeight;
        private double savedLeft, savedTop;
        private WindowState savedWindowState;

        /// <summary>
        /// If set to true, always saves and restores the window's size.
        /// </summary>
        public bool AlwaysTrackResize { get; set; }

        /// <summary>
        /// Creates a new instance of WpfWindowManager.
        /// </summary>
        /// <param name="parent">The window for which properties are managed.</param>
        public WpfWindowManager(Window parent) : base(parent)
        {
            if (AppInfo.IsPortable.GetValueOrDefault())
                PortableSettingsProvider.ApplyProvider(Properties.Settings.Default);
            setCulture();
        }

        private void setCulture()
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.Culture))
            {
                var culture = new CultureInfo(Properties.Settings.Default.Culture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            base.Initialize(nameof(Context.Loaded), nameof(Context.Closed));
        }

        /// <inheritdoc />
        public override void ManageDefault()
        {
            this.manageDefault = true;
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.Left)), DEFAULT, false);
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.Top)), DEFAULT, false);
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.WindowState)), WindowState.Normal, false);
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.Width)), DEFAULT, false);
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.Height)), DEFAULT, false);
        }

        #region "Parent Events"
        /// <inheritdoc />
        protected override void ContextLoad()
        {
            base.ContextLoad();
            if (manageDefault)
            {
                Context.LocationChanged += Parent_LocationChanged;
                sizeable = Context.ResizeMode == ResizeMode.CanResize || AlwaysTrackResize;
                if (sizeable) Context.SizeChanged += Parent_SizeChanged;
                if ((double)CustomSettings["Left"] != DEFAULT)
                    Context.Left = (double)CustomSettings["Left"];
                if ((double)CustomSettings["Top"] != DEFAULT)
                    Context.Top = (double)CustomSettings["Top"];
                if (sizeable)
                {
                    try
                    {
                        savedWidth = Context.Width; savedHeight = Context.Height;
                        if ((double)CustomSettings["Width"] != DEFAULT)
                            Context.Width = (double)CustomSettings["Width"];
                        if ((double)CustomSettings["Height"] != DEFAULT)
                            Context.Height = (double)CustomSettings["Height"];
                        Context.WindowState = (WindowState)CustomSettings["WindowState"];
                    }
                    catch
                    {
                        CustomSettings.Reset();
                        Context.WindowState = (WindowState)CustomSettings["WindowState"];
                    }
                }
                var newLoc = MoveIntoScreenBounds(Context);
                Context.Left = newLoc.X;
                Context.Top = newLoc.Y;
            }
        }

        /// <summary>
        /// Returns a new position within the screen bounds for a given window that may be outside.
        /// </summary>
        /// <param name="window">The window to be checked.</param>
        /// <returns>A new position within the screeen bounds if the current position is outside; the old position otherwise.</returns>
        public static Point MoveIntoScreenBounds(Window window)
        {
            Rect windowRect = new Rect(window.Left, window.Top,
                                       window.Width, window.Height);
            Rect screenRect = 
                new Rect(fromPhysical(window, SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop),
                         fromPhysical(window, SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth, 
                                              SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight));
            Point newLocation = windowRect.Location;
            if (!screenRect.IntersectsWith(windowRect))
            {
                if (windowRect.Left < screenRect.Left)
                    newLocation.X = screenRect.Left;
                else if (windowRect.Left > screenRect.Right)
                    newLocation.X = screenRect.Right - windowRect.Width;
                if (windowRect.Top < screenRect.Top)
                    newLocation.Y = screenRect.Top;
                else if (windowRect.Top >= screenRect.Bottom)
                    newLocation.Y = screenRect.Bottom - windowRect.Height;
            }
            return newLocation;
        }

        // transforms physical coordinates to a scaled point.
        private static Point fromPhysical(Visual vis, double x, double y)
        {
            Matrix transform = PresentationSource.FromVisual(vis).CompositionTarget.TransformFromDevice;
            return transform.Transform(new Point(x, y));
        }

        private void Parent_LocationChanged(object sender, EventArgs e)
        {
            if (Context.WindowState == WindowState.Normal)
            {
                savedLeft = Context.Left;
                savedTop = Context.Top;
            }
        }

        private void Parent_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Context.WindowState == WindowState.Minimized)
                return;
            savedWindowState = Context.WindowState;
            if (Context.WindowState == WindowState.Normal)
            {
                savedWidth = Context.Width;
                savedHeight = Context.Height;
            }
        }

        /// <inheritdoc />
        protected override void ContextSave()
        {
            base.ContextSave();
            if (manageDefault)
            {
                CustomSettings["Left"] = savedLeft;
                CustomSettings["Top"] = savedTop;
                if (sizeable)
                {
                    CustomSettings["Width"] = savedWidth;
                    CustomSettings["Height"] = savedHeight;
                    CustomSettings["WindowState"] = savedWindowState;
                }
                CustomSettings.Save();
            }
        }
        #endregion    
    }
}
