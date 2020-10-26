using System;
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

        [Fact(DisplayName = "Can create new queue successfully")]
        public async Task CanCreateNewQueueSuccessfully()
        {
            const string queueName = "addedqueue";

            var result = await LocalStorageFixture.AddQueueRunner.Run(new AddQueueOptions { Name = queueName }).ConfigureAwait(false);
            var queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);

            Assert.True(result);
            Assert.Contains(queueList, x => x.Name.Equals(queueName));
        }

        [Fact(DisplayName = "Can remove queue successfully")]
        public async Task CanRemoveQueueSuccessfully()
        {
            const string queueName = "removequeue";

            await LocalStorageFixture.AddQueueRunner.Run(new AddQueueOptions { Name = queueName }).ConfigureAwait(false);
            var queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);

            Assert.Contains(queueList, x => x.Name.Equals(queueName));

            var removeQueueResult = await LocalStorageFixture.QueueRunner.Run(new QueueOptions { RemoveQueue = queueName }).ConfigureAwait(false);
            queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);

            Assert.True(removeQueueResult);
            Assert.DoesNotContain(queueList, x => x.Name.Equals(queueName));
        }

        [Fact(DisplayName = "Can move posion queues back to main queue")]
        public async Task CanMovePoisonQueuesToMainQueue()
        {
            const string normalQueueName = "posionmovetest";

            await LocalStorageFixture.AzureStorageManager.CreateQueue(DevStorageConnectionString, normalQueueName);
            await LocalStorageFixture.AzureStorageManager.ClearQueue(DevStorageConnectionString, normalQueueName);

            const string poisonQueueName = "posionmovetest-poison";

            await LocalStorageFixture.AzureStorageManager.CreateQueue(DevStorageConnectionString, poisonQueueName);
            await LocalStorageFixture.AzureStorageManager.ClearQueue(DevStorageConnectionString, poisonQueueName);
            await LocalStorageFixture.AzureStorageManager.AddMessage(DevStorageConnectionString, poisonQueueName, "{ 'poison': true }");

            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { CureQueue = normalQueueName }).ConfigureAwait(false);

            var queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);

            var normalQueue = queueList.Find(x => x.Name.Equals(normalQueueName));
            var poisonQueue = queueList.Find(x => x.Name.Equals(poisonQueueName));

            await normalQueue.FetchAttributesAsync();
            await poisonQueue.FetchAttributesAsync();

            Assert.Equal(1, normalQueue.ApproximateMessageCount);
            Assert.Equal(0, poisonQueue.ApproximateMessageCount);
        }
    }
}
