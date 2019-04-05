using System.Collections.Generic;
using JustSaying.Models;
using Newtonsoft.Json.Linq;

namespace JustSaying.Messaging.MessageSerialisation
{
    public class MessageAttributesMessageTypeKeyTransport : IMessageTypeKeyTransport
    {
        public string RetrieveFrom(Amazon.SQS.Model.Message message)
        {
            JObject messageJson = JObject.Parse(message.Body);

            JToken messageAttributesAttr = messageJson["MessageAttributes"];

            if(messageAttributesAttr ==  null)
                throw new MessageFormatNotSupportedException("MessageAttributes missing from message");

            JToken justSayingMessageTypeAttr = messageAttributesAttr[RequiredMessageAttributes.JustSayingMessageType];

            if (justSayingMessageTypeAttr == null)
                throw new MessageFormatNotSupportedException($"{RequiredMessageAttributes.JustSayingMessageType} missing from MessageAttributes");

            return justSayingMessageTypeAttr.Value<string>("Value");
        }

        public void Store(string messagTypeKey, Message msg)
        {
            if (msg.MessageAttributes == null)
            {
                msg.MessageAttributes = new Dictionary<string, MessageAttributeValue>();
            }

            msg.MessageAttributes?.Add(RequiredMessageAttributes.JustSayingMessageType, new Models.MessageAttributeValue
            {
                DataType = "String",
                StringValue = messagTypeKey
            });
        }
    }
}
