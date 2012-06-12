using System.Text;
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
    public class HashFileCreator_Tests
    {
        private IFileStreamWrap _fileStreamStub;
        private IFileWrap _fileStub;
        private string _fileName;
        private string _hashValue;

        [SetUp]
        public void InitTest()
        {
            _fileStreamStub = MockRepository.GenerateStub<IFileStreamWrap>();
            _fileStub = MockRepository.GenerateStub<IFileWrap>();
            _fileName = "Image_Test"; // Sample Value.
            _hashValue = "HASHVALUE"; // Sample Value.
        }

        [Test]
        public void Created_File_Successfully()
        {
            var creator = new HashFileCreator();
            creator.SetFileName(_fileName);

            // Setup the path of the file as a string and create the file.
            const string activeDir = @"C:\Users\Hash Values Folder";
            string fullPath = activeDir + @"\" + creator.FileName + ".sha2.txt";

            Assert.IsTrue(creator.CreateFile(_fileStub, fullPath));

            _fileStub.AssertWasCalled(x => x.Create(fullPath));

            // Get the byte[] hash value and write it to the file.
            var encoding = new UTF8Encoding();
            var hashValueByteArray = encoding.GetBytes(_hashValue);

            Assert.IsTrue(creator.WriteToFile(_fileStreamStub, hashValueByteArray));

            // Confirm the write to file was completed in its entirety.
            foreach (var t in hashValueByteArray)
            {
                _fileStreamStub.AssertWasCalled(x => x.WriteByte(t));
            }
        }

        /*
         * HashFileCreator stub implementation.
         */
        public class HashFileCreator
        {
            public void SetFileName(string fileName)
            {
                FileName = fileName;
            }

            public string FileName { get; set; }

            public bool CreateFile(IFileWrap file, string filePath)
            {
                file.Create(filePath);
                return true;
            }

            public bool WriteToFile(IFileStreamWrap fileStream, byte[] bytesToWrite)
            {
                using (fileStream)
                {
                    foreach (var t in bytesToWrite)
                    {
                        fileStream.WriteByte(t);
                    }
                }
                return true;
            }
        }
    }
}
