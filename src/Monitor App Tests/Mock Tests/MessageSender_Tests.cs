using System;
using System.ServiceModel;
using NUnit.Framework;

/*
 * Solution: SiteDocs Challenge Console App
 * Author: Helmut Wollenberg
 * Date: June 10-11, 2012
 */

namespace Monitor_App_Tests.Mock_Tests
{
    [TestFixture]
    public class MessageSender_Tests
    {
        private WorkerStub _workerStub;
        private string _fileWebLocation;

        [SetUp]
        public void InitTest()
        {
            _workerStub = new WorkerStub();
            _fileWebLocation = "http://testurl.com/Image_Test.png"; // Sample Value.
        }

        [Test]
        public void Is_Worker_Available()
        {
            _workerStub.AvailableToWork = true;
            Assert.IsTrue(_workerStub.AvailableToWork);
        }

        [Test]
        public void Sent_Work_Request_Message_Successfully()
        {
            _workerStub.AvailableToWork = true;

            // Get and bind to the service (worker).
            var serviceChannelFactory = new ChannelFactory<IWorkerService>(
                new NetTcpBinding(), "");
            var service = serviceChannelFactory.CreateChannel();

            // Send the message (file's URI) and get the acknowledgement.
            bool acknowledgement = service.Message(_fileWebLocation);

            (service as ICommunicationObject).Close();

            Assert.IsTrue(acknowledgement);
        }

        /*
         * ServiceContract worker interface.
         */
        [ServiceContract]
        public interface IWorkerService
        {
            [OperationContract]
            bool Message(string fileURI);
        }

        /*
         * CommunicationObject interface for WorkerStub.
         */
        public interface ICommunicationObject
        {
            void Close();
        }

        /*
         * Worker service implementation (stub).
         */
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
        public class WorkerStub : IWorkerService, ICommunicationObject
        {
            public bool AvailableToWork { get; set; }

            public bool Message(string fileURI)
            {
                return true;
            }

            public void Close() { }
        }

        /*
         * Mock ChannelFactory for creating channels to worker service.
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
         * Channel stub to worker service.
         */
        public class MyChannel : WorkerStub { }
    }
}
