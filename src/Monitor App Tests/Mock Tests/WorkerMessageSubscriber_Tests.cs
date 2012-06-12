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
    public class WorkerMessageSubscriber_Tests
    {
        private string _hashValue;

        [SetUp]
        public void InitTest()
        {
            _hashValue = "HASHVALUE"; // Sample Value.
        }

        [Test]
        public void Get_Hash_Value_From_Worker_Successfully()
        {
            // Get and bind to the service (worker).
            var serviceChannelFactory = new ChannelFactory<IWorkerService>(
                new NetTcpBinding(), "");
            var service = serviceChannelFactory.CreateChannel();

            // Set and get the hash value from the worker.
            service.SetHashValue(_hashValue);
            string hashValueFromWorker = service.ProcessingComplete();

            (service as ICommunicationObject).Close();

            Assert.AreEqual(_hashValue, hashValueFromWorker);
        }

        /*
         * ServiceContract worker interface.
         */
        [ServiceContract]
        public interface IWorkerService
        {
            [OperationContract]
            string ProcessingComplete();
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
            private string _hashValue;

            public WorkerStub()
            {
                IsProcessing = false;
                _hashValue = null;
            }

            private bool IsProcessing { get; set; }

            public void SetHashValue(string hashValue)
            {
                _hashValue = hashValue;
            }

            public string ProcessingComplete()
            {
                if (!IsProcessing)
                {
                    return _hashValue;
                }
                else
                {
                    return null;
                }
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
