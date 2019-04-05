using Message = JustSaying.Models.Message;
using SQSMessage = Amazon.SQS.Model.Message;

namespace JustSaying.Messaging.MessageSerialisation
{
    public interface IMessageTypeKeyTransport
    {
        string RetrieveFrom(SQSMessage msg);
        void Store(string messageTypeKey, Message msg);
    }
}
