using System;
using System.Net;
using SystemWrapper.IO;
using NUnit.Framework;
using Rhino.Mocks;

/*
 * Solution: SiteDocs Challenge Console App
 * Author: Helmut Wollenberg
 * Date: June 10-11, 2012
 */

namespace Worker_App_Tests.Mock_Tests
{
    [TestFixture]
    public class FileDownloader_Tests
    {
        private IDirectoryInfoWrap _directoryInfoStub;
        private WebClient _webClient;
        private string _testURI;
        private string _testFileName;
        private string _testDirectory;

        [SetUp]
        public void TestInit()
        {
            _directoryInfoStub = MockRepository.GenerateStub<IDirectoryInfoWrap>();

            _testURI = "http://testurl.com/Test_Image.png"; // Sample value.
            _testFileName = "Test_Image.png"; // Sample Value.
            _testDirectory = @"C:\Image Temp Folder"; // Sample Value.
            _webClient = new WebClient(_testURI, _testDirectory);
        }

        [Test]
        public void Directory_Exists()
        {
            _directoryInfoStub.Stub(x => x.Exists).Return(true);

            Assert.IsTrue(new FileDownloader().CheckIfDirectoryExists(_directoryInfoStub));
        }

        [Test]
        public void Web_Location_Exists()
        {
            try
            {
                _webClient.DownloadString(_testURI);
                Assert.Pass();
            }
            catch (Exception ex)
            {
                if ((ex is WebException) || (ex is ArgumentNullException) || (ex is NotSupportedException))
                {
                    Assert.Fail();
                }
                else
                {
                    throw;
                }
            }
        }

        [Test]
        public void Download_File_Successfully()
        {
            try
            {
                var downloader = new FileDownloader();
                downloader.FileName = _testFileName;
                string fullPath = _testDirectory + @"\" + downloader.FileName;

                _webClient.DownloadFile(_testURI, _testDirectory);
                Assert.Pass();
            }
            catch (Exception ex)
            {
                if ((ex is WebException) || (ex is ArgumentNullException) || (ex is NotSupportedException))
                {
                    Assert.Fail();
                }
                else
                {
                    throw;
                }
            }
        }

        /*
         * FileDownloader stub implementation.
         */
        public class FileDownloader
        {
            public string FileName { get; set; }

            public bool CheckIfDirectoryExists(IDirectoryInfoWrap directory)
            {
                return directory.Exists;
            }
        }

        /*
         * A stub (based on WebClient) containing methods used by the FileDownloader.
         */
        public class WebClient
        {
            private readonly string _webLocation;
            private readonly string _directory;

            public WebClient(string webLocation, string directory)
            {
                _webLocation = webLocation;
                _directory = directory;
            }

            /*
             * Downloads the file if the webLocation and fileLocation params are valid.
             * Validity is not really tested.
             */
            public void DownloadFile(string webLocation, string directory)
            {
                if (string.IsNullOrEmpty(webLocation) || string.IsNullOrEmpty(directory))
                {
                    throw new ArgumentNullException();
                }
                if (!(webLocation.Equals(_webLocation) && directory.Equals(_directory)))
                {
                    throw new WebException();
                }
            }

            /*
             * Used to verify the webLocation exists and is readable.
             */
            public string DownloadString(string webLocation)
            {
                if (string.IsNullOrEmpty(webLocation))
                {
                    throw new ArgumentNullException();
                }
                if (webLocation.Equals(_webLocation))
                {
                    return "";
                }
                else
                {
                    throw new WebException();
                }
            }
        }
    }
}
