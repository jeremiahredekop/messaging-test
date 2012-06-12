using System;
using System.ServiceModel;
using NUnit.Framework;

/*
 * Solution: SiteDocs Challenge Console App
 * Author: Helmut Wollenberg
 * Date: June 10-11, 2012
 */

namespace Worker_App_Tests.Mock_Tests
{
    [TestFixture]
    public class MonitorMessageSubscriber_Tests
    {
        private string _fileLocationURI;

        [SetUp]
        public void InitTest()
        {
            _fileLocationURI = "http://testurl.com/Image_Test.png"; // Sample Value.
        }

        [Test]
        public void Get_File_Web_Location_From_Monitor_Successfully()
        {
            // Get and bind to the service (monitor).
            var serviceChannelFactory = new ChannelFactory<IMonitorService>(
                new NetTcpBinding(), "");
            var service = serviceChannelFactory.CreateChannel();

            // Set and get the file URL from the monitor.
            service.SetFileURI(_fileLocationURI);
            string fileLocationURIFromMonitor = service.FileReadyToDownload();

            (service as ICommunicationObject).Close();

            Assert.AreEqual(_fileLocationURI, fileLocationURIFromMonitor);
        }

        /*
         * ServiceContract monitor interface.
         */
        [ServiceContract]
        public interface IMonitorService
        {
            [OperationContract]
            string FileReadyToDownload();
        }

        /*
         * CommunicationObject interface for MonitorStub.
         */
        public interface ICommunicationObject
        {
            void Close();
        }

        /*
         * Monitor service implementation (stub).
         */
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
        public class MonitorStub : IMonitorService, ICommunicationObject
        {
            private string _fileLocationURI;

            public MonitorStub()
            {
                IsFileReady = true;
                _fileLocationURI = null;
            }

            private bool IsFileReady { get; set; }

            public void SetFileURI(string fileLocationURI)
            {
                _fileLocationURI = fileLocationURI;
            }

            public string FileReadyToDownload()
            {
                if (IsFileReady)
                {
                    return _fileLocationURI;
                }
                else
                {
                    return null;
                }
            }

            public void Close() { }
        }

        /*
         * Mock ChannelFactory for creating channels to monitor service.
         */
        public class ChannelFactory<T>
        {
            private readonly bool _factoryReady;

            public ChannelFactory(NetTcpBinding binding, string locationURI)
            {
                _factoryReady = true;
            }

            public MyChannel CreateChannel()
            {
                if (_factoryReady)
                {
                    return new MyChannel();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /*
         * Channel stub to monitor service.
         */
        public class MyChannel : MonitorStub { }
    }
}
