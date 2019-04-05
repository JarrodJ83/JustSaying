using System.Collections.Generic;
using Message = JustSaying.Models.Message;
using SQSMessage = Amazon.SQS.Model.Message;

namespace JustSaying.Messaging.MessageSerialisation
{
    public class MessageSerialisationRegister : IMessageSerialisationRegister
    {
        private readonly IMessageSubjectProvider _messageSubjectProvider;
        private readonly Dictionary<string, TypeSerialiser> _map = new Dictionary<string, TypeSerialiser>();

        public MessageSerialisationRegister(IMessageSubjectProvider messageSubjectProvider)
        {
            _messageSubjectProvider = messageSubjectProvider;
        }
        
        public void AddSerialiser<T>(IMessageSerialiser serialiser) where T : Message
        {
            var typeNameKey = _messageSubjectProvider.GetSubjectForType(typeof(T));
            if (!_map.ContainsKey(typeNameKey))
            {
                _map.Add(typeNameKey, new TypeSerialiser(typeof(T), serialiser));
            }
        }

        public Message DeserializeMessage(SQSMessage msg, string messageTypeKey)
        {
            TypeSerialiser typeSerializer = GetTypeSerializer(messageTypeKey);
            return typeSerializer.Serialiser.Deserialise(msg.Body, typeSerializer.Type);
        }

        public string Serialise(Message message, bool serializeForSnsPublishing)
        {
            var messageTypeKey = _messageSubjectProvider.GetSubjectForType(message.GetType());
            var typeSerializer = GetTypeSerializer(messageTypeKey);
            return typeSerializer.Serialiser.Serialise(message, serializeForSnsPublishing, messageTypeKey);
        }
        
        private TypeSerialiser GetTypeSerializer(string messageTypeKey)
        {
            if (!_map.ContainsKey(messageTypeKey))
                throw new MessageFormatNotSupportedException(
                    $"No seralizer for {messageTypeKey} was found.");

            return _map[messageTypeKey];
        }
    }
}
