using Amazon.SQS.Model;
using Newtonsoft.Json.Linq;

namespace JustSaying.Messaging.MessageSerialisation
{
    public class BackwardCompatibilitySubjectMessageTypeKeyTransport : IMessageTypeKeyTransport
    {
        private readonly IMessageTypeKeyTransport _messageTypeKeyTransport;

        public BackwardCompatibilitySubjectMessageTypeKeyTransport(IMessageTypeKeyTransport messageTypeKeyTransport)
        {
            _messageTypeKeyTransport = messageTypeKeyTransport;
        }
        public string RetrieveFrom(Message msg)
        {
            var body = JObject.Parse(msg.Body);

            JToken type = body["Subject"];

            if (type != null)
            {
                return type.ToString();
            }

            return _messageTypeKeyTransport.RetrieveFrom(msg);
        }

        public void Store(string messageTypeKey, Models.Message msg)
        {
            _messageTypeKeyTransport.Store(messageTypeKey, msg);
        }
    }
}
