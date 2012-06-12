using System.Linq;
using Worker_App.Core;

namespace Worker_App
{
    public class WorkerAppFacade
    {
        private readonly IFileDownloader _fileDownloader;
        private readonly IHashCalculator _hashCalculator;
        private readonly IMessageSubscriber _messageSubscriber;
        private readonly IMessageSender _messageSender;

        public WorkerAppFacade(IFileDownloader fileDownloader, IHashCalculator hashCalculator, IMessageSubscriber messageSubscriber, IMessageSender messageSender)
        {
            _fileDownloader = fileDownloader;
            _hashCalculator = hashCalculator;
            _messageSubscriber = messageSubscriber;
            _messageSender = messageSender;
        }
    }


}