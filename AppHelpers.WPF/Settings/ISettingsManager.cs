using System.Configuration;

namespace Bluegrams.Application
{
    /// <summary>
    /// Provides a generic interface for a property settings manager.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISettingsManager<T> where T : class
    {
        /// <summary>
        /// The item whose properties are managed.
        /// </summary>
        T Context { get; }
        /// <summary>
        /// The managed settings.
        /// </summary>
        CustomSettings CustomSettings { get; }

        /// <summary>
        /// Setup property managing.
        /// </summary>
        /// <param name="loadEvent">Name of the event when property settings should be loaded.</param>
        /// <param name="saveEvent">Name of the event when property settings shoudld be saved.</param>
        void Initialize(string loadEvent, string saveEvent);
        /// <summary>
        /// Manage some implementation specific default properties.
        /// </summary>
        void ManageDefault();
        /// <summary>
        /// Manage the public property of the context object with the given name.
        /// </summary>
        /// <param name="propertyName">Name of the managed property.</param>
        /// <param name="defaultValue">If not null, the default value when no saved value was found.</param>
        /// <param name="roamed">True of setting should be roamed, otherwise false.</param>
        void Manage(string propertyName, object defaultValue = null, bool roamed = false);
        /// <summary>
        /// Manage the public property of the context object with the given name.
        /// </summary>
        /// <param name="propertyName">Name of the managed property.</param>
        /// <param name="serializeAs">Specified how this setting should be serialized.</param>
        /// <param name="defaultValue">If not null, the default value when no saved value was found.</param>
        /// <param name="roamed">True of setting should be roamed, otherwise false.</param>
        void Manage(string propertyName, SettingsSerializeAs serializeAs, object defaultValue = null, bool roamed = false);
        /// <summary>
        /// Manage the public properties of the context object with the given names.
        /// </summary>
        /// <param name="propertyNames"></param>
        void Manage(params string[] propertyNames);
    }
}
