using System.Linq;

namespace Worker_App.Core
{
    public interface IMessageSender
    {
        void SendMessage(object message);
    }
}