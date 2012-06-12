using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Messages;
using NUnit.Framework;
using Rhino.Mocks;
using Worker_App;
using Worker_App.Core;

namespace Worker_App_Tests
{
    [TestFixture]
    public class IntegrationTests
    {

        public class MockSubscriber : IMessageSubscriber
        {
            private List<Action<FileToProcessMessage>> _handlers;

            public MockSubscriber()
            {
                _handlers = new List<Action<FileToProcessMessage>>();
            }

            public void Subscribe(Action<FileToProcessMessage> handler)
            {
                if (handler == null) throw new ArgumentNullException("handler");

                _handlers.Add(handler);
            }


            public void SimulatePublishedMessage(FileToProcessMessage message)
            {
                _handlers.ForEach(h=> h.Invoke(message));
            }


        }

        [Test]
        public void Should_fire_message_with_hash()
        {
            var stubDownloader = MockRepository.GenerateStub<IFileDownloader>();
            var hashCalculator = MockRepository.GenerateStub<IHashCalculator>();
            var messageSubscriber = new MockSubscriber();
            var messageSender = MockRepository.GenerateStub<IMessageSender>();

            var facade = new WorkerAppFacade(stubDownloader, hashCalculator, messageSubscriber, messageSender);

            var message = new FileToProcessMessage() {Uri = "just a mock for now"};

            messageSubscriber.SimulatePublishedMessage(message);

            messageSender.AssertWasCalled(s => s.SendMessage(Arg<FileProcessedMessage>.Matches(m=> m.Hash == "MOCK_HASH")));
        }
    }
}
