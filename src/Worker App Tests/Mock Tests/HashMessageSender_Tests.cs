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
    public class HashMessageSender_Tests
    {
        private MonitorStub _monitorStub;
        private byte[] _fileHashedBytes;

        [SetUp]
        public void InitTest()
        {
            _monitorStub = new MonitorStub();
            _fileHashedBytes = new byte[1024]; // Sample empty byte[].
        }

        [Test]
        public void Is_Monitor_Available()
        {
            _monitorStub.ReadyToReceiveHash = true;
            Assert.IsTrue(_monitorStub.ReadyToReceiveHash);
        }

        [Test]
        public void Sent_Hash_Message_Successfully()
        {
            _monitorStub.ReadyToReceiveHash = true;

            // Get and bind to the service (monitor).
            var serviceChannelFactory = new ChannelFactory<IMonitorService>(
                new NetTcpBinding(), "");
            var service = serviceChannelFactory.CreateChannel();

            // Send the message (hash) and get the acknowledgement.
            bool acknowledgement = service.Message(_fileHashedBytes);

            (service as ICommunicationObject).Close();

            Assert.IsTrue(acknowledgement);
        }

        /*
         * ServiceContract monitor interface.
         */
        [ServiceContract]
        public interface IMonitorService
        {
            [OperationContract]
            bool Message(byte[] hashedBytes);
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
            public bool ReadyToReceiveHash { get; set; }

            public bool Message(byte[] hashedBytes)
            {
                return true;
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
