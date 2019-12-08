using System;
using System.ComponentModel;
using System.Reflection;
using System.Configuration;

namespace Bluegrams.Application
{
    /// <summary>
    /// The custom settings handled by the manager.
    /// </summary>
    public sealed class CustomSettings : SettingsBase
    {
        // All properties in this settings class use the same provider.
        private readonly SettingsProvider provider;

        /// <summary>
        /// Creates a new instance of the class CustomSettings.
        /// </summary>
        /// <param name="portable">Specifies if PortableSettingsProvider should be used.</param>
        /// <param name="groupName">Explicitly specify a custom settings section name.</param>
        public CustomSettings(bool portable, string groupName = null) : base()
        {
            if (portable) provider = new PortableSettingsProvider();
            else provider = new LocalFileSettingsProvider();
            provider.Initialize("CommonSettingsProvider", null);
            var context = new SettingsContext();
            context["GroupName"] = groupName ?? GetType().FullName;
            context["SettingsClassType"] = GetType();
            var providers = new SettingsProviderCollection() { provider };
            base.Initialize(context, new SettingsPropertyCollection(), providers);
        }

        /// <summary>
        /// Adds a settings property.
        /// </summary>
        /// <param name="propInfo">The property info of the property to add.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <param name="roamed">Specifies if this setting should be roamed.</param>
        internal void AddSetting(PropertyInfo propInfo, object defaultValue = null, bool roamed = true)
        {
            AddSetting(propInfo, defaultValue, roamed, getSerializeAs(propInfo.PropertyType));
        }

        /// <summary>
        /// Adds a settings property.
        /// </summary>
        /// <param name="propInfo">The property info of the property to add.</param>
        /// <param name="serializeAs">The serialization mode of the property.</param>
        /// <param name="defaultValue">The default value of the property.</param>
        /// <param name="roamed">Specifies if this setting should be roamed.</param>
        internal void AddSetting(PropertyInfo propInfo, object defaultValue, bool roamed, SettingsSerializeAs serializeAs)
        {
            AddSetting(propInfo.Name, propInfo.PropertyType, defaultValue, roamed, serializeAs);
        }

        internal void AddSetting(string name, Type type, object defaultValue, bool roamed, SettingsSerializeAs serializeAs)
        {
            SettingsProperty settingsProp = new SettingsProperty(name);
            settingsProp.PropertyType = type;
            settingsProp.Provider = provider;
            settingsProp.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());
            if (roamed)
            {
                settingsProp.Attributes.Add(typeof(SettingsManageabilityAttribute),
                    new SettingsManageabilityAttribute(SettingsManageability.Roaming));
            }
            settingsProp.SerializeAs = serializeAs;
            settingsProp.DefaultValue = defaultValue?.ToString();
            Properties.Add(settingsProp);
        }

        /// <summary>
        /// Reloads the application settings property values from persistent storage.
        /// </summary>
        public void Reload()
        {
            if (PropertyValues != null)
            {
                PropertyValues.Clear();
            }
        }

        /// <summary>
        /// Calls Reset on the provider.
        /// </summary>
        public void Reset()
        {
            if (Properties != null)
            {
                IApplicationSettingsProvider clientProv = provider as IApplicationSettingsProvider;
                if (clientProv != null)
                {
                    clientProv.Reset(Context);
                }
            }
            Reload();
        }

        /// <summary>
        /// Calls Upgrade on the provider.
        /// </summary>
        public void Upgrade()
        {
            if (Properties != null)
            {
                IApplicationSettingsProvider clientProv = provider as IApplicationSettingsProvider;
                if (clientProv != null)
                {
                    clientProv.Upgrade(Context, Properties);
                }
            }
            Reload();
        }

        private SettingsSerializeAs getSerializeAs(Type type)
        {
            // Check whether the type has a TypeConverter that can convert to/from string
            TypeConverter tc = TypeDescriptor.GetConverter(type);
            bool toString = tc.CanConvertTo(typeof(string));
            bool fromString = tc.CanConvertFrom(typeof(string));
            if (toString && fromString)
            {
                return SettingsSerializeAs.String;
            }
            //Else use Xml Serialization 
            return SettingsSerializeAs.Xml;
        }
    }
}
