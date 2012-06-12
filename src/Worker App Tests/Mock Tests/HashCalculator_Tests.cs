using System.IO;
using System.Security.Cryptography;
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
    public class HashCalculator_Tests
    {
        private IFileInfoWrap _fileInfoStub;
        private IFileStreamWrap _fileStreamStub;
        private IBinaryReaderWrap _binaryReaderStub;
        private string _testFileLocation;
        private int _sizeOfFile;
        private HashCalculator _hashCalc;

        [SetUp]
        public void TestInit()
        {
            _fileInfoStub = MockRepository.GenerateStub<IFileInfoWrap>();
            _fileStreamStub = MockRepository.GenerateStub<IFileStreamWrap>();
            _binaryReaderStub = MockRepository.GenerateStub<IBinaryReaderWrap>();

            _testFileLocation = @"C:\Image Temp Folder\Image_Test.png"; // Sample Value.
            _sizeOfFile = 1024; // Sample Value.
            _hashCalc = new HashCalculator();
        }

        [Test]
        public void File_Exists()
        {
            _fileInfoStub.Stub(x => x.Exists).Return(true);

            Assert.IsTrue(_hashCalc.CheckIfFileExists(_fileInfoStub));
        }

        [Test]
        public void File_Can_Be_Opened_For_Reading()
        {
            _fileStreamStub.Stub(x => x.CanRead).Return(true);

            Assert.IsTrue(_hashCalc.CheckIfFileIsReadable(_fileStreamStub));
        }

        [Test]
        public void File_Can_Be_Read_Successfully()
        {
            _binaryReaderStub.Stub(x => x.ReadBytes(_sizeOfFile)).Return(new byte[1024]);

            try
            {
                Assert.IsInstanceOf(typeof(byte[]), _hashCalc.ReadInFileBytes(_binaryReaderStub, _sizeOfFile));
            }
            catch (IOException)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void File_Is_Hashed_Correctly()
        {
            var fileBytes = new byte[_sizeOfFile];
            for (var i = 0; i < _sizeOfFile; i++)
            {
                fileBytes[i] = (byte) i;
            }

            Assert.AreEqual(new SHA256Managed().ComputeHash(fileBytes), _hashCalc.SHA256Encrypt(fileBytes));
        }

        /*
         * HashCalculator stub implementation.
         */
        public class HashCalculator
        {
            public bool CheckIfFileExists(IFileInfoWrap file)
            {
                return file.Exists;
            }

            public bool CheckIfFileIsReadable(IFileStreamWrap fileStream)
            {
                return fileStream.CanRead;
            }

            public byte[] ReadInFileBytes(IBinaryReaderWrap binaryReader, int fileSize)
            {
                var buffer = binaryReader.ReadBytes(fileSize);

                if (buffer.Length != 0)
                {
                    return buffer;
                }
                else
                {
                    throw new IOException();
                }
            }

            public byte[] SHA256Encrypt(byte[] fileBytes)
            {
                var sha256Hasher = new SHA256Managed();
                var hashedDataBytes = sha256Hasher.ComputeHash(fileBytes);
                return hashedDataBytes;
            }
        }
    }
}
