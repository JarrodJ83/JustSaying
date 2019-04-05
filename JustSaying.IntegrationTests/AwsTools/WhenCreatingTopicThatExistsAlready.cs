using System.Threading.Tasks;
using JustSaying.AwsTools.MessageHandling;
using JustSaying.Messaging.MessageSerialisation;
using Shouldly;
using Xunit;

namespace JustSaying.IntegrationTests.AwsTools
{
    [Collection(GlobalSetup.CollectionName)]
    public class WhenCreatingTopicThatExistsAlready : WhenCreatingTopicByName
    {
        private bool _createWasSuccessful;
        private SnsTopicByName _topic;

        protected override async Task When()
        {
            _topic = new SnsTopicByName(
                UniqueName,
                Client,
                new MessageSerialisationRegister(MessageSubjectProvider),
                LoggerFactory,
                new NonGenericMessageSubjectProvider(),
                new MessageAttributesMessageTypeKeyTransport());

            _createWasSuccessful = await _topic.CreateAsync();
        }

        [AwsFact]
        public void CreateCallIsStillSuccessful()
        {
            _createWasSuccessful.ShouldBeTrue();
        }

        [AwsFact]
        public void TopicArnIsPopulated()
        {
            _topic.Arn.ShouldNotBeNull();
            _topic.Arn.ShouldEndWith(_topic.TopicName);
            _topic.Arn.ShouldBe(CreatedTopic.Arn);
        }
    }
}
