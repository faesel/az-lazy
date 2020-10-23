using System.Threading.Tasks;
using az_lazy.Commands.Queue;
using Xunit;

namespace az_lazy.test.QueueTest
{
    [Collection("LocalStorage")]
    [Trait("Queue", "Integration test for queues")]
    public class Queue
    {
        private readonly LocalStorageFixture LocalStorageFixture;

        public Queue(LocalStorageFixture localStorageFixture)
        {
            this.LocalStorageFixture = localStorageFixture;
        }

        [Fact(DisplayName = "Can create new queue successfully")]
        public async Task CanCreateNewQueueSuccesfully()
        {
            var result = await LocalStorageFixture.AddQueueRunner.Run(new AddQueueOptions { Name = "AddedQueue" });

            var connectionList = LocalStorageFixture.LocalStorageManager.GetConnections();

            Assert.False(result);
            Assert.Contains(connectionList, x => x.ConnectionName.Equals(DevStorageName) &&
                    x.ConnectionString.Equals(DevStorageConnectionString));
        }
    }
}
