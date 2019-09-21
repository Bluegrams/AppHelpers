using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace Bluegrams.Application
{
    /// <summary>
    /// Base class for a window settings manager for WPF or WinForms.
    /// </summary>
    public abstract class WindowManager<T> : ISettingsManager<T> where T : class
    {
        private HashSet<string> managedSettings;

        /// <inheritdoc />
        public T Context { get; }
        /// <inheritdoc />
        public CustomSettings CustomSettings { get; }

        /// <summary>
        /// Initializes a new instance of MiniAppManagerBase.
        /// </summary>
        /// <param name="context">The parent window.</param>
        public WindowManager(T context)
        {
            this.Context = context;
            managedSettings = new HashSet<string>();
            CustomSettings = new CustomSettings(AppInfo.IsPortable.GetValueOrDefault());
        }

        /// <summary>
        /// Setup property managing with implementation specific default events.
        /// </summary>
        public abstract void Initialize();

        /// <inheritdoc />
        public void Initialize(string loadEvent, string saveEvent)
        {
            EventInfo loadInfo = typeof(T).GetEvent(loadEvent);
            var dLoad = Helpers.CreateHandler(loadInfo, () => ContextLoad());
            loadInfo.AddEventHandler(Context, dLoad);
            EventInfo saveInfo = typeof(T).GetEvent(saveEvent);
            var dSave = Helpers.CreateHandler(saveInfo, () => ContextSave());
            saveInfo.AddEventHandler(Context, dSave);
        }

        /// <inheritdoc />
        public abstract void ManageDefault();

        /// <inheritdoc />
        public void Manage(string propertyName, object defaultValue = null, bool roamed = false)
        {
            managedSettings.Add(propertyName);
            CustomSettings.AddSetting(Context.GetType().GetProperty(propertyName), defaultValue, roamed);
        }

        /// <inheritdoc />
        public void Manage(string propertyName, SettingsSerializeAs serializeAs, object defaultValue = null, bool roamed = false)
        {
            managedSettings.Add(propertyName);
            CustomSettings.AddSetting(Context.GetType().GetProperty(propertyName), defaultValue, roamed, serializeAs);
        }

        /// <inheritdoc />
        public void Manage(params string[] propertyNames)
        {
            foreach (string prop in propertyNames)
                Manage(prop);
        }

        /// <summary>
        /// Handles the the event used for loading settings.
        /// </summary>
        protected virtual void ContextLoad()
        {
            foreach (string s in managedSettings)
            {
                object setting = CustomSettings[s];
                if (setting == null) continue;
                var prop = Context.GetType().GetProperty(s);
                prop.SetValue(Context, setting);
            }
        }

        /// <summary>
        /// Handles the event used for saving settings.
        /// </summary>
        protected virtual void ContextSave()
        {
            if (managedSettings.Count < 1) return;
            foreach (string s in managedSettings)
            {
                CustomSettings[s] = Context.GetType().GetProperty(s).GetValue(Context);
            }
            CustomSettings.Save();
        }
    }
}
