using System;
using System.IO;
using SystemWrapper.IO;
using NUnit.Framework;
using Rhino.Mocks;

/*
 * Solution: SiteDocs Challenge Console App
 * Author: Helmut Wollenberg
 * Date: June 10-11, 2012
 */

namespace Monitor_App_Tests.Mock_Tests
{
    [TestFixture]
    public class DirectoryMonitor_Tests
    {
        private IDirectoryInfoWrap _directoryInfoStub;

        private IFileSystemWatcherWrap _fileSystemWatcherMock;
        private bool _wasCalled;

        [SetUp]
        public void TestInit()
        {
            _directoryInfoStub = MockRepository.GenerateStub<IDirectoryInfoWrap>();

            _fileSystemWatcherMock = MockRepository.GenerateMock<IFileSystemWatcherWrap>();
        }

        [Test]
        public void Directory_Exists()
        {
            _directoryInfoStub.Stub(x => x.Exists).Return(true);

            Assert.IsTrue(new DirectoryMonitor().CheckIfDirectoryExists(_directoryInfoStub));
        }

        [Test]
        public void File_Created_In_Directory_Is_Correctly_Monitored()
        {
            _wasCalled = false;
            _fileSystemWatcherMock.Created += (sender, e) => _wasCalled = true;

            // Check to see if the Created event is raised correctly.
            _fileSystemWatcherMock.Raise(
                x => x.Created += null, _fileSystemWatcherMock, EventArgs.Empty as FileSystemEventArgs);

            Assert.IsTrue(_wasCalled);
        }

        [Test]
        public void File_Changed_In_Directory_Is_Correctly_Monitored()
        {
            _wasCalled = false;
            _fileSystemWatcherMock.Changed += (sender, e) => _wasCalled = true;

            // Check to see if the Changed event is raised correctly.
            _fileSystemWatcherMock.Raise(
                x => x.Changed += null, _fileSystemWatcherMock, EventArgs.Empty as FileSystemEventArgs);

            Assert.IsTrue(_wasCalled);
        }

        /*
         * DirectoryMonitor stub implementation.
         */
        public class DirectoryMonitor
        {
            public bool CheckIfDirectoryExists(IDirectoryInfoWrap directory)
            {
                return directory.Exists;
            }
        }
    }
}
