using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using JustBehave;
using JustSaying.AwsTools.MessageHandling;
using JustSaying.Messaging.MessageHandling;
using JustSaying.Messaging.MessageSerialisation;
using JustSaying.Messaging.Monitoring;
using JustSaying.TestingFramework;
using JustSaying.UnitTests.AwsTools.MessageHandling.SqsNotificationListener.Support;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace JustSaying.UnitTests.AwsTools.MessageHandling.SqsNotificationListener
{
    public abstract class BaseQueuePollingTest : XAsyncBehaviourTest<JustSaying.AwsTools.MessageHandling.SqsNotificationListener>
    {
        protected const string QueueUrl = "url";
        protected IAmazonSQS Sqs;
        protected SimpleMessage DeserialisedMessage;
        protected const string MessageBody = "object";
        protected IHandlerAsync<SimpleMessage> Handler;
        protected IMessageMonitor Monitor;
        protected ILoggerFactory LoggerFactory;
        protected IMessageSerialisationRegister SerialisationRegister;
        protected IMessageLockAsync MessageLock;
        protected readonly Message Message = new Message();

        protected override JustSaying.AwsTools.MessageHandling.SqsNotificationListener CreateSystemUnderTest()
        {
            var queue = new SqsQueueByUrl(RegionEndpoint.EUWest1, QueueUrl, Sqs);
            return new JustSaying.AwsTools.MessageHandling.SqsNotificationListener(queue, SerialisationRegister, Monitor, LoggerFactory, null, MessageLock);
        }

        protected override void Given()
        {
            LoggerFactory = new LoggerFactory();
            Sqs = Substitute.For<IAmazonSQS>();
            SerialisationRegister = Substitute.For<IMessageSerialisationRegister>();
            Monitor = Substitute.For<IMessageMonitor>();
            Handler = Substitute.For<IHandlerAsync<SimpleMessage>>();
            LoggerFactory = Substitute.For<ILoggerFactory>();
            
            var response = GenerateResponseMessage(Guid.NewGuid());
            
            Sqs.ReceiveMessageAsync(
                    Arg.Any<ReceiveMessageRequest>(), 
                    Arg.Any<CancellationToken>())
                .Returns(
                    x => Task.FromResult(response),
                    x => Task.FromResult(new ReceiveMessageResponse()));

            DeserialisedMessage = new SimpleMessage { RaisingComponent = "Component" };
            SerialisationRegister.DeserializeMessage(Arg.Any<Message>(), Arg.Any<string>()).Returns(DeserialisedMessage);
        }
        protected override async Task When()
        {
            var doneSignal = new TaskCompletionSource<object>();
            var signallingHandler = new SignallingHandler<SimpleMessage>(doneSignal, Handler);

            SystemUnderTest.AddMessageHandler(() => signallingHandler);
            SystemUnderTest.Listen();

            // wait until it's done
            var doneOk = await Tasks.WaitWithTimeoutAsync(doneSignal.Task);

            SystemUnderTest.StopListening();

            doneOk.ShouldBeTrue("Timout occured before done signal");
        }

        protected ReceiveMessageResponse GenerateResponseMessage()
        {
            return GenerateResponseMessage(Guid.NewGuid());
        }

        protected ReceiveMessageResponse GenerateResponseMessage(Guid messageId)
        {
            return new ReceiveMessageResponse
            {
                Messages = new List<Message>
                {
                    new Message
                    {   
                        MessageId = messageId.ToString(),
                        Body = SqsMessageBody()
                    },
                    new Message
                    {
                        MessageId = messageId.ToString(),
                        Body = "{\"Message\":\"SOME_RANDOM_MESSAGE\"}"
                    }
                }
            };
        }

        protected string SqsMessageBody()
        {
            return "{\"Message\":\"" + MessageBody + "\"}";
        }
    }
}
