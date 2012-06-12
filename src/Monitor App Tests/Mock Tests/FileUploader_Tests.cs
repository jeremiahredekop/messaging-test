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

namespace Monitor_App_Tests.Mock_Tests
{
    [TestFixture]
    public class FileUploader_Tests
    {
        private IFileInfoWrap _fileInfoStub;
        private WebClient _webClient;
        private string _testURI;
        private string _testFileLocation;

        [SetUp]
        public void TestInit()
        {
            _fileInfoStub = MockRepository.GenerateStub<IFileInfoWrap>();

            _testURI = "http://testurl.com"; // Sample value.
            _testFileLocation = @"C:\Image_Test.png"; // Sample Value.
            _webClient = new WebClient(_testURI, _testFileLocation);
        }

        [Test]
        public void File_Exists()
        {
            _fileInfoStub.Stub(x => x.Exists).Return(true);

            Assert.IsTrue(new FileUploader().CheckIfFileExists(_fileInfoStub));
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
        public void Upload_File_Successfully()
        {
            try
            {
                _webClient.UploadFile(_testURI, _testFileLocation);
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
         * FileUploader stub implementation.
         */
        public class FileUploader
        {
            public bool CheckIfFileExists(IFileInfoWrap file)
            {
                return file.Exists;
            }
        }

        /*
         * A stub (based on WebClient) containing methods used by the FileUploader.
         */
        public class WebClient
        {
            private readonly string _webLocation;
            private readonly string _fileLocation;

            public WebClient(string webLocation, string fileLocation)
            {
                _webLocation = webLocation;
                _fileLocation = fileLocation;
            }

            /*
             * Uploads the file if the webLocation and fileLocation params are valid.
             * Validity is not really tested.
             */
            public byte[] UploadFile(string webLocation, string fileLocation)
            {
                if (string.IsNullOrEmpty(webLocation) || string.IsNullOrEmpty(fileLocation))
                {
                    throw new ArgumentNullException();
                }
                if (webLocation.Equals(_webLocation) && fileLocation.Equals(_fileLocation))
                {
                    return new byte[1];
                }
                else
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
