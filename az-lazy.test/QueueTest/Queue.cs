using System.Threading.Tasks;
using AutoFixture.Xunit2;
using az_lazy.Commands.Queue;
using Xunit;

namespace az_lazy.test.QueueTest
{
    [Collection("LocalStorage")]
    [Trait("Queue", "Integration test for queues")]
    public class Queue
    {
        private const string DevStorageConnectionString = "UseDevelopmentStorage=true";
        private readonly LocalStorageFixture LocalStorageFixture;

        public Queue(LocalStorageFixture localStorageFixture)
        {
            this.LocalStorageFixture = localStorageFixture;
        }

        [AutoData]
        [Theory(DisplayName = "Can create new queue successfully")]
        public async Task CanCreateNewQueueSuccesfully(string queueName)
        {
            var result = await LocalStorageFixture.AddQueueRunner.Run(new AddQueueOptions { Name = queueName }).ConfigureAwait(false);
            var queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);

            Assert.True(result);
            Assert.Contains(queueList, x => x.Name.Equals(queueName));
        }
    }
}
