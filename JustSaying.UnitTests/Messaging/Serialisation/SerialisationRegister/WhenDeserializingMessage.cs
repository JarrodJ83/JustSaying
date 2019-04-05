using System;
using JustBehave;
using JustSaying.Messaging.MessageSerialisation;
using JustSaying.Models;
using NSubstitute;
using Shouldly;
using Xunit;

namespace JustSaying.UnitTests.Messaging.Serialisation.SerialisationRegister
{
    public class WhenDeserializingMessage : XBehaviourTest<MessageSerialisationRegister>
    {

        private readonly IMessageSubjectProvider _messageSubjectProvider = Substitute.For<IMessageSubjectProvider>();



        private class CustomMessage : Message
        {
        }

        protected override MessageSerialisationRegister CreateSystemUnderTest() =>
            new MessageSerialisationRegister(_messageSubjectProvider);

        private string messageBody = "msgBody";

        private const string MessageTypeKey = "Key";
        protected override void Given()
        {
            _messageSubjectProvider.GetSubjectForType(Arg.Any<Type>()).Returns(MessageTypeKey);
            RecordAnyExceptionsThrown();
        }

        protected override void When()
        {
            var messageSerialiser = Substitute.For<IMessageSerialiser>();
            messageSerialiser.Deserialise(messageBody, typeof (CustomMessage)).Returns(new CustomMessage());
            SystemUnderTest.AddSerialiser<CustomMessage>(messageSerialiser);
        }

        [Fact]
        public void ThrowsMessageFormatNotSupportedWhenMessabeBodyIsUnserializable()
        {
            new Action(() => SystemUnderTest.DeserializeMessage(new Amazon.SQS.Model.Message(), "invalidKey")).ShouldThrow<MessageFormatNotSupportedException>();
        }

        [Fact]
        public void TheMappingContainsTheSerialiser()
        {
            var msg = SystemUnderTest.DeserializeMessage(new Amazon.SQS.Model.Message()
            {
                Body = messageBody
            }, MessageTypeKey);

            msg.ShouldNotBeNull();
        }
    }
}
