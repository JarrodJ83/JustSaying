using System.Collections.Generic;
using JustBehave;
using JustSaying.Messaging.MessageSerialisation;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using SQSMessage = Amazon.SQS.Model.Message;

namespace JustSaying.UnitTests.Messaging.Serialisation.MessageTypeTransport
{
    public class WhenRetrievingMessageTypeKey : XBehaviourTest<MessageAttributesMessageTypeKeyTransport>
    {
        private const string JustSayingMessageType = "Test";
        SQSMessage InboundMessage = new SQSMessage()
        {
            Body = JsonConvert.SerializeObject(new
            {
                MessageAttributes = new Dictionary<string, Attr>
                {
                    {
                        nameof(JustSayingMessageType), new Attr{ Type = "String", Value = JustSayingMessageType }
                    }
                }
            })
        };

        SQSMessage InboundMessageMissingJustSayingMessageTypeAttribute = new SQSMessage()
        {
            Body = JsonConvert.SerializeObject(new
            {
                MessageAttributes = new Dictionary<string, Attr>
                {
                }
            })
        };

        SQSMessage InboundMessageMissingMessageAttributes = new SQSMessage()
        {
            Body = JsonConvert.SerializeObject(new
            {
                MessageAttributes = new Dictionary<string, Attr>
                {
                }
            })
        };


        class Attr
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }

        protected override void Given()
        {

        }

        protected override void When()
        {

        }

        [Fact]
        public void JustSayingMessageType_Returned()
        {
            SystemUnderTest.RetrieveFrom(InboundMessage).ShouldBe(JustSayingMessageType);
        }

        [Fact]
        public void JustSayingMessageType_Missing_JustSayingMessageType_Throws_NotSupportedException()
        {
            var exception = Should.Throw<MessageFormatNotSupportedException>(() => SystemUnderTest.RetrieveFrom(InboundMessageMissingJustSayingMessageTypeAttribute));
            exception.Message.ShouldContain(nameof(JustSayingMessageType));
        }

        [Fact]
        public void JustSayingMessageType_Missing_MessageAttributes_Throws_NotSupportedException()
        {
            var exception = Should.Throw<MessageFormatNotSupportedException>(() => SystemUnderTest.RetrieveFrom(InboundMessageMissingMessageAttributes));
            exception.Message.ShouldContain("MessageAttributes");
        }



    }
}
