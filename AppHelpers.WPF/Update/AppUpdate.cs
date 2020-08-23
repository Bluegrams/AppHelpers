using System;
using System.Linq;
using System.Xml.Serialization;

namespace Bluegrams.Application
{
    /// <summary>
    /// A class containing information about the latest app update.
    /// </summary>
    [Serializable]
    public class AppUpdate
    {
        /// <summary>
        /// The latest released version of the application.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The release date of the update.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// The link to the default download of the latest version.
        /// </summary>
        public string DownloadLink { get; set; }

        /// <summary>
        /// The name the default downloaded file should have.
        /// </summary>
        public string DownloadFileName { get; set; }

        /// <summary>
        /// A list of additional download options.
        /// </summary>
        [XmlArrayItem("Download")]
        public DownloadEntry[] Downloads { get; set; }

        /// <summary>
        /// Generic release notes of this version.
        /// Use ReleaseNotes for different languages.
        /// </summary>
        public string VersionNotes { get; set; }
        
        /// <summary>
        /// A list of release notes in different languages.
        /// If no matching item found, VersionNotes is used as a default.
        /// </summary>
        public ReleaseNote[] ReleaseNotes { get; set; }

        /// <summary>
        /// Gets a matching release notes string for the given language code.
        /// </summary>
        /// <param name="langCode">The language code to search for. If not specified, fall back to default VersionNotes.</param>
        /// <returns>The resolved string or null if none found.</returns>
        public string GetReleaseNotes(string langCode = null)
        {
            return ReleaseNotes?.FirstOrDefault(o => o.LanguageCode == langCode)?.Text ?? VersionNotes;
        }
    }

    /// <summary>
    /// A class representing one download entry.
    /// </summary>
    [Serializable]
    public class DownloadEntry
    {
        /// <summary>
        /// The identifier of the entry.
        /// </summary>
        [XmlAttribute("key")]
        public string Key { get; set; }

        /// <summary>
        /// The download link.
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// The name the downloaded file should have.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The file hash of the downloaded file.
        /// </summary>
        public FileHash FileHash { get; set; }
    }

    /// <summary>
    /// A class representing one release note item.
    /// </summary>
    public class ReleaseNote
    {
        /// <summary>
        /// The release note language.
        /// </summary>
        [XmlAttribute("lang")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// The release note text.
        /// </summary>
        [XmlText(Type = typeof(string))]
        public string Text { get; set; }
    }

    /// <summary>
    /// A class representing a file hash.
    /// </summary>
    public class FileHash
    {
        /// <summary>
        /// The hash algorithm used.
        /// </summary>
        [XmlAttribute("algorithm")]
        public string HashAlgorithm { get; set; } = "MD5";

        /// <summary>
        /// The file hash.
        /// </summary>
        [XmlText(Type = typeof(string))]
        public string Hash { get; set; }
    }
}
