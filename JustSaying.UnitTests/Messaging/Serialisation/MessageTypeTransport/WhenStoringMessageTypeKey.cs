using System;
using System.Collections.Generic;
using System.Text;
using JustBehave;
using JustSaying.Messaging;
using JustSaying.Messaging.MessageSerialisation;
using JustSaying.Models;
using JustSaying.TestingFramework;
using Shouldly;
using Xunit;

namespace JustSaying.UnitTests.Messaging.Serialisation.MessageTypeTransport
{
    public class WhenStoringMessageTypeKey : XBehaviourTest<MessageAttributesMessageTypeKeyTransport>
    {
        private const string messageTypeKey = "Testing";
        private Message OutboundMessage;

        protected override void Given()
        {
            OutboundMessage = new AnotherSimpleMessage();
        }

        protected override void When()
        {
            SystemUnderTest.Store(messageTypeKey, OutboundMessage);
        }

        [Fact]
        public void Message_Should_Have_JustSayingMessageType_Attribute()
        {
            OutboundMessage.MessageAttributes.ContainsKey(RequiredMessageAttributes.JustSayingMessageType).ShouldBeTrue();
        }

        [Fact]
        public void JustSayingMessageType_Attribute_Value_Should_Be_Set()
        {
            OutboundMessage.MessageAttributes[RequiredMessageAttributes.JustSayingMessageType].StringValue.ShouldBe(messageTypeKey);
        }
    }
}
