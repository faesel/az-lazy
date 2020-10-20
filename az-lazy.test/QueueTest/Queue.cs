using Xunit;

namespace az_lazy.test.QueueTest
{
    [Collection("LocalStorage")]
    public class Queue
    {
        private readonly LocalStorageFixture LocalStorageFixture;

        public Queue(LocalStorageFixture localStorageFixture)
        {
            this.LocalStorageFixture = localStorageFixture;
        }
    }
}
