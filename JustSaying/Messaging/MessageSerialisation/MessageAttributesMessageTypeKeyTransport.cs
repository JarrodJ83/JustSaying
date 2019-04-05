using JustSaying.Models;

namespace JustSaying.Messaging.MessageSerialisation
{
    public class MessageAttributesMessageTypeKeyTransport : IMessageTypeKeyTransport
    {
        public string RetrieveFrom(Amazon.SQS.Model.Message message)
        {
            if (message.MessageAttributes.ContainsKey(RequiredMessageAttributes.JustSayingMessageType))
            {
                return message.MessageAttributes[RequiredMessageAttributes.JustSayingMessageType].StringValue;
            }

            return string.Empty;
        }

        public void Store(string messagTypeKey, Message msg)
        {
            msg.MessageAttributes.Add(RequiredMessageAttributes.JustSayingMessageType, new Models.MessageAttributeValue
            {
                DataType = "String",
                StringValue = messagTypeKey
            });
        }
    }
}
