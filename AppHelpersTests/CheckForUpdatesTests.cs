using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Bluegrams.Application;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppHelpersTests
{
    [TestClass]
    public class CheckForUpdatesTests
    {
        [TestInitialize]
        public void SetupTest()
        {
            SetEntryAssembly(Assembly.GetExecutingAssembly());
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
        }

        [TestMethod]
        public void TestCheckForUpdates()
        {
            AutoResetEvent autoReset = new AutoResetEvent(false);
            var fullPath = Path.GetFullPath("TestFiles\\AppUpdate.xml");
            UpdateCheckerBase updateChecker = new UpdateCheckerBase(fullPath);
            updateChecker.UpdateCheckCompleted += (object sender, UpdateCheckEventArgs e) =>
            {
                Assert.IsTrue(e.Successful);
                Assert.IsTrue(e.NewVersion);
                Assert.AreEqual(UpdateNotifyMode.Always, e.UpdateNotifyMode);
                Assert.AreEqual("1.2.3.42", e.Update.Version);
                Assert.AreEqual(new DateTime(2019, 12, 29), e.Update.ReleaseDate);
                Assert.AreEqual(4, e.Update.Downloads.Length);
                Assert.AreEqual("Some release notes. Some more text.", e.Update.VersionNotes);
                autoReset.Set();
            };
            updateChecker.CheckForUpdates(UpdateNotifyMode.Always);
            // wait for event to be fired
            bool signaled = autoReset.WaitOne(3000);
            Assert.IsTrue(signaled);
        }

        /// <summary>
        /// Allows setting the Entry Assembly when needed. 
        /// From https://stackoverflow.com/a/21888521.
        /// </summary>
        internal static void SetEntryAssembly(Assembly assembly)
        {
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }
    }
}
