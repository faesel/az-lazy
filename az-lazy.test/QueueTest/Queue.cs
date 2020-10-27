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

            await LocalStorageFixture.AzureStorageManager.CreateQueue(DevStorageConnectionString, normalQueueName).ConfigureAwait(false);
            await LocalStorageFixture.AzureStorageManager.ClearQueue(DevStorageConnectionString, normalQueueName).ConfigureAwait(false);

            const string poisonQueueName = "posionmovetest-poison";

            await LocalStorageFixture.AzureStorageManager.CreateQueue(DevStorageConnectionString, poisonQueueName).ConfigureAwait(false);
            await LocalStorageFixture.AzureStorageManager.ClearQueue(DevStorageConnectionString, poisonQueueName).ConfigureAwait(false);
            await LocalStorageFixture.AzureStorageManager.AddMessage(DevStorageConnectionString, poisonQueueName, "{ 'poison': true }").ConfigureAwait(false);

            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { CureQueue = normalQueueName }).ConfigureAwait(false);

            var queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);

            var normalQueue = queueList.Find(x => x.Name.Equals(normalQueueName));
            var poisonQueue = queueList.Find(x => x.Name.Equals(poisonQueueName));

            await normalQueue.FetchAttributesAsync().ConfigureAwait(false);
            await poisonQueue.FetchAttributesAsync().ConfigureAwait(false);

            Assert.Equal(1, normalQueue.ApproximateMessageCount);
            Assert.Equal(0, poisonQueue.ApproximateMessageCount);
        }

        [Fact(DisplayName = "Can clear queue of all messages")]
        public async Task CanClearQueueSuccessfully()
        {
            const string queueName = "clearqueue";

            await LocalStorageFixture.AzureStorageManager.CreateQueue(DevStorageConnectionString, queueName).ConfigureAwait(false);
            await LocalStorageFixture.AzureStorageManager.AddMessage(DevStorageConnectionString, queueName, "{}").ConfigureAwait(false);

            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { ClearQueue = queueName }).ConfigureAwait(false);

            var queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);
            var clearedQueue = queueList.Find(x => x.Name.Equals(queueName));
            await clearedQueue.FetchAttributesAsync().ConfigureAwait(false);

            Assert.Equal(0, clearedQueue.ApproximateMessageCount);
        }

        [Fact(DisplayName = "Can add a new message to queue")]
        public async Task CanSuccessfullyAddMessage()
        {
            const string queueName = "addmessages";

            await LocalStorageFixture.AzureStorageManager.CreateQueue(DevStorageConnectionString, queueName).ConfigureAwait(false);
            await LocalStorageFixture.AzureStorageManager.ClearQueue(DevStorageConnectionString, queueName).ConfigureAwait(false);
            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { AddQueue = queueName, AddMessage = "{}" }).ConfigureAwait(false);

            var queueList = await LocalStorageFixture.AzureStorageManager.GetQueues(DevStorageConnectionString).ConfigureAwait(false);
            var clearedQueue = queueList.Find(x => x.Name.Equals(queueName));
            await clearedQueue.FetchAttributesAsync().ConfigureAwait(false);

            Assert.Equal(1, clearedQueue.ApproximateMessageCount);
        }
    }
}
