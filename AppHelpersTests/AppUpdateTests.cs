using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Bluegrams.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHelpersTests
{
    [TestClass]
    public class AppUpdateTests
    {
        private const string UPDATE_FILE = "TestFiles\\AppUpdate.xml";
        private const string DOWNLOAD_FILE = "TestFiles\\DownloadTestFile.txt";

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(AppUpdate));
        UpdateCheckerBase updateChecker;
        AppUpdate appUpdate;
        string downloadFileText;

        [TestInitialize]
        public void SetupTest()
        {
            updateChecker = new UpdateCheckerBase(Path.GetFullPath(UPDATE_FILE));
            using (FileStream s = File.OpenRead(UPDATE_FILE))
            {
                appUpdate = (AppUpdate)xmlSerializer.Deserialize(s);
            }
            downloadFileText = File.ReadAllText(DOWNLOAD_FILE);
        }

        [TestMethod]
        public void TestResolveEntryAvailable()
        {
            updateChecker.DownloadIdentifier = "fileHashMD5";
            DownloadEntry entry = updateChecker.ResolveDownloadEntry(appUpdate);
            Assert.AreEqual("TestFiles\\DownloadTestFile.txt", entry.Link);
            Assert.AreEqual("Download.txt", entry.FileName);
        }

        [TestMethod]
        public void TestResolveEntryNotAvailable()
        {
            updateChecker.DownloadIdentifier = "notAvailableIdentifier";
            DownloadEntry entry = updateChecker.ResolveDownloadEntry(appUpdate);
            Assert.AreEqual("http://www.example.com", entry.Link);
            Assert.AreEqual("Default.zip", entry.FileName);
            Assert.IsNull(entry.FileHash);
        }

        [TestMethod]
        public async Task TestDownloadUpdateMD5()
        {
            updateChecker.DownloadIdentifier = "fileHashMD5";
            string downloadPath = await updateChecker.DownloadUpdate(appUpdate);
            Assert.IsNotNull(downloadPath);
            Assert.AreEqual(downloadFileText, File.ReadAllText(downloadPath));
        }

        [TestMethod]
        public async Task TestDownloadUpdateSHA256()
        {
            updateChecker.DownloadIdentifier = "fileHashSHA256";
            string downloadPath = await updateChecker.DownloadUpdate(appUpdate);
            Assert.IsNotNull(downloadPath);
            Assert.AreEqual(downloadFileText, File.ReadAllText(downloadPath));
        }

        [DataTestMethod]
        [DataRow("otherDownload", "DownloadTestFile.txt", true)]
        [DataRow("otherDownload2", "CorrectFileName.zip", false)]
        public async Task TestDownloadUpdateNoFileName(string identifier, string expectedName, bool download)
        {
            updateChecker.DownloadIdentifier = identifier;
            // first check FileName
            DownloadEntry entry = updateChecker.ResolveDownloadEntry(appUpdate);
            Assert.AreEqual(expectedName, entry.FileName);
            // now perform download
            if (!download) return;
            string downloadPath = await updateChecker.DownloadUpdate(appUpdate);
            Assert.IsNotNull(downloadPath);
            Assert.AreEqual(downloadFileText, File.ReadAllText(downloadPath));
        }

        [TestMethod]
        public async Task TestFileVerificationFails()
        {
            updateChecker.DownloadIdentifier = "invalidDownload";
            await Assert.ThrowsExceptionAsync<UpdateFailedException>(
                () => updateChecker.DownloadUpdate(appUpdate));
        }
    }
}
