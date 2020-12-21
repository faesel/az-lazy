using System.Threading.Tasks;
using az_lazy.Commands.AddQueue;
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

            await LocalStorageFixture.AddQueueRunner.Run(new AddQueueOptions { Name = queueName });
            var queueList = await LocalStorageFixture.AzureQueueManager.GetQueues(DevStorageConnectionString);

            Assert.Contains(queueList, x => x.Name.Equals(queueName));
        }

        [Fact(DisplayName = "Can remove queue successfully")]
        public async Task CanRemoveQueueSuccessfully()
        {
            const string queueName = "removequeue";

            await LocalStorageFixture.AddQueueRunner.Run(new AddQueueOptions { Name = queueName });
            var queueList = await LocalStorageFixture.AzureQueueManager.GetQueues(DevStorageConnectionString);

            Assert.Contains(queueList, x => x.Name.Equals(queueName));

            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { RemoveQueue = queueName });

            queueList = await LocalStorageFixture.AzureQueueManager.GetQueues(DevStorageConnectionString);

            Assert.DoesNotContain(queueList, x => x.Name.Equals(queueName));
        }

        [Fact(DisplayName = "Can move posion queues back to main queue")]
        public async Task CanMovePoisonQueuesToMainQueue()
        {
            const string normalQueueName = "posionmovetest";

            await LocalStorageFixture.AzureQueueManager.CreateQueue(DevStorageConnectionString, normalQueueName);
            await LocalStorageFixture.AzureQueueManager.ClearQueue(DevStorageConnectionString, normalQueueName);

            const string poisonQueueName = "posionmovetest-poison";

            await LocalStorageFixture.AzureQueueManager.CreateQueue(DevStorageConnectionString, poisonQueueName);
            await LocalStorageFixture.AzureQueueManager.ClearQueue(DevStorageConnectionString, poisonQueueName);
            await LocalStorageFixture.AzureQueueManager.AddMessage(DevStorageConnectionString, poisonQueueName, @"{ ""poison"" : true }");

            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { CureQueue = normalQueueName });

            var queueList = await LocalStorageFixture.AzureQueueManager.GetQueues(DevStorageConnectionString);

            var normalQueue = queueList.Find(x => x.Name.Equals(normalQueueName));
            var poisonQueue = queueList.Find(x => x.Name.Equals(poisonQueueName));

            await normalQueue.FetchAttributesAsync();
            await poisonQueue.FetchAttributesAsync();

            Assert.Equal(1, normalQueue.ApproximateMessageCount);
            Assert.Equal(0, poisonQueue.ApproximateMessageCount);
        }

        [Fact(DisplayName = "Can clear queue of all messages")]
        public async Task CanClearQueueSuccessfully()
        {
            const string queueName = "clearqueue";

            await LocalStorageFixture.AzureQueueManager.CreateQueue(DevStorageConnectionString, queueName);
            await LocalStorageFixture.AzureQueueManager.AddMessage(DevStorageConnectionString, queueName, "{}");

            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { ClearQueue = queueName });

            var queueList = await LocalStorageFixture.AzureQueueManager.GetQueues(DevStorageConnectionString);
            var clearedQueue = queueList.Find(x => x.Name.Equals(queueName));
            await clearedQueue.FetchAttributesAsync();

            Assert.Equal(0, clearedQueue.ApproximateMessageCount);
        }

        [Fact(DisplayName = "Can peek a new message to queue")]
        public async Task CanSuccessfullyViewMessage()
        {
            const string queueName = "peekmessage";
            const string messageText = @"{ ""test"" : true }";

            await LocalStorageFixture.AzureQueueManager.CreateQueue(DevStorageConnectionString, queueName);
            await LocalStorageFixture.AzureQueueManager.ClearQueue(DevStorageConnectionString, queueName);
            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { AddQueue = queueName, AddMessage = messageText });

            var message = await LocalStorageFixture.AzureQueueManager.PeekMessages(DevStorageConnectionString, queueName, 1);

            Assert.NotNull(message);
            Assert.Single(message);
            Assert.Equal(message[0].MessageText, messageText);
        }

        [Fact(DisplayName = "Can move messages from one queue to another")]
        public async Task CanMoveMessagesSuccessfully()
        {
            const string fromQueueName = "fromqueuename";

            await LocalStorageFixture.AzureQueueManager.CreateQueue(DevStorageConnectionString, fromQueueName);
            await LocalStorageFixture.AzureQueueManager.ClearQueue(DevStorageConnectionString, fromQueueName);
            await LocalStorageFixture.AzureQueueManager.AddMessage(DevStorageConnectionString, fromQueueName, @"{ ""test"" : true }");

            const string toQueueName = "toqueuename";

            await LocalStorageFixture.AzureQueueManager.CreateQueue(DevStorageConnectionString, toQueueName);
            await LocalStorageFixture.AzureQueueManager.ClearQueue(DevStorageConnectionString, toQueueName);

            await LocalStorageFixture.QueueRunner.Run(new QueueOptions { From = fromQueueName, To = toQueueName });

            var queueList = await LocalStorageFixture.AzureQueueManager.GetQueues(DevStorageConnectionString);

            var fromQueue = queueList.Find(x => x.Name.Equals(fromQueueName));
            var toQueue = queueList.Find(x => x.Name.Equals(toQueueName));

            await fromQueue.FetchAttributesAsync();
            await toQueue.FetchAttributesAsync();

            Assert.Equal(0, fromQueue.ApproximateMessageCount);
            Assert.Equal(1, toQueue.ApproximateMessageCount);
        }
    }
}
