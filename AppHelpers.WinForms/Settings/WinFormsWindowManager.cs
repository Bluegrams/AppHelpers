using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides a WindowManager implementation managing settings such as size and location for a WinForms application.
    /// </summary>
    public class WinFormsWindowManager : WindowManager<Form>
    {
        private const int DEFAULT = int.MinValue;

        private bool manageDefault, sizeable;
        private int savedWidth, savedHeight;
        private int savedLeft, savedTop;
        private FormWindowState savedWindowState;

        /// <summary>
        /// If set to true, always saves and restores the window's size.
        /// </summary>
        public bool AlwaysTrackResize { get; set; }

        /// <summary>
        /// Creates a new instance of WinFormsWindowManager.
        /// </summary>
        /// <param name="parent">The window for which properties are managed.</param>
        public WinFormsWindowManager(Form parent) : base(parent)
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
            base.Initialize(nameof(Context.Load), nameof(Context.FormClosing));
        }

        /// <inheritdoc />
        public override void ManageDefault()
        {
            this.manageDefault = true;
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.Left)), DEFAULT, false);
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.Top)), DEFAULT, false);
            CustomSettings.AddSetting(Context.GetType().GetProperty(nameof(Context.WindowState)), FormWindowState.Normal, false);
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
                sizeable = Context.FormBorderStyle == FormBorderStyle.Sizable || AlwaysTrackResize;
                if (sizeable) Context.SizeChanged += Parent_SizeChanged;
                if ((int)CustomSettings["Left"]  != DEFAULT)
                    Context.Left = (int)CustomSettings["Left"];
                if ((int)CustomSettings["Top"] != DEFAULT)
                    Context.Top = (int)CustomSettings["Top"];
                if (sizeable)
                {
                    try
                    {
                        savedWidth = Context.Width; savedHeight = Context.Height;
                        if ((int)CustomSettings["Width"] != DEFAULT)
                            Context.Width = (int)CustomSettings["Width"];
                        if ((int)CustomSettings["Height"] != DEFAULT)
                            Context.Height = (int)CustomSettings["Height"];
                        Context.WindowState = (FormWindowState)CustomSettings["WindowState"];
                    }
                    catch
                    {
                        CustomSettings.Reset();
                        Context.WindowState = (FormWindowState)CustomSettings["WindowState"];
                    }
                }
                Context.Location = MoveIntoScreenBounds(Context);
            }
        }

        /// <summary>
        /// Returns a new position within the screen bounds for a given window that may be outside.
        /// </summary>
        /// <param name="form">The window to be checked.</param>
        /// <returns>A new position within the screeen bounds if the current position is outside; the old position otherwise.</returns>
        public static Point MoveIntoScreenBounds(Form form)
        {
            Rectangle windowRect = form.Bounds;
            Rectangle screenRect = Screen.FromRectangle(windowRect).WorkingArea;
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

        private void Parent_LocationChanged(object sender, EventArgs e)
        {
            if (Context.WindowState == FormWindowState.Normal)
            {
                savedLeft = Context.Left;
                savedTop = Context.Top;
            }
        }

        private void Parent_SizeChanged(object sender, EventArgs e)
        {
            if (Context.WindowState == FormWindowState.Minimized)
                return;
            savedWindowState = Context.WindowState;
            if (Context.WindowState == FormWindowState.Normal)
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
