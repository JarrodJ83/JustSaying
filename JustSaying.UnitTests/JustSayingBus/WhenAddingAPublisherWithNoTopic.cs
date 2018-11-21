using System.Threading.Tasks;
using JustSaying.Messaging;
using JustSaying.TestingFramework;
using NSubstitute;
using Shouldly;
using Xunit;

namespace JustSaying.UnitTests.JustSayingBus
{
    public class WhenAddingAPublisherWithNoTopic : GivenAServiceBus
    {
        protected override Task Given()
        {
            RecordAnyExceptionsThrown();
            return Task.CompletedTask;
        }

        protected override Task When()
        {
            SystemUnderTest.AddMessagePublisher<SimpleMessage>(Substitute.For<IMessagePublisher>(), string.Empty);

            return Task.CompletedTask;
        }

        [Fact]
        public void ExceptionThrown()
        {
            ThrownException.ShouldNotBeNull();
        }
    }
}
