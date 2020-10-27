using AutoFixture.Xunit2;
using az_lazy.Commands;
using System.Threading.Tasks;
using Xunit;

namespace az_lazy.test.ConnectionTest
{
    [Collection("LocalStorage")]
    [Trait("Connection", "Integration test for connections")]
    public class Connection
    {
        private const string DevStorageName = "devStorage";
        private const string DevStorageConnectionString = "UseDevelopmentStorage=true";

        private readonly LocalStorageFixture LocalStorageFixture;

        public Connection(LocalStorageFixture localStorageFixture)
        {
            this.LocalStorageFixture = localStorageFixture;
        }

        [Fact(DisplayName = "Connections always have development storage")]
        public void ContainsDevelopmentStorageConnection()
        {
            var connectionList = LocalStorageFixture.LocalStorageManager.GetConnections();

            Assert.Contains(connectionList, x => x.ConnectionName.Equals(DevStorageName) &&
                    x.ConnectionString.Equals(DevStorageConnectionString));
        }

        [Fact(DisplayName = "Cannot remove development storage")]
        public async Task CannotRemoveDevelopmentStorage()
        {
            var result = await LocalStorageFixture.ConnectionRunner.Run(new ConnectionOptions { RemoveConnection = DevStorageName });

            var connectionList = LocalStorageFixture.LocalStorageManager.GetConnections();

            Assert.False(result);
            Assert.Contains(connectionList, x => x.ConnectionName.Equals(DevStorageName) &&
                    x.ConnectionString.Equals(DevStorageConnectionString));
        }

        [AutoData]
        [Theory(DisplayName = "Can list multiple connections")]
        public async Task CanListMultipleConnections_WhenExists(string fakeConnection, string fakeConnectionString)
        {
            LocalStorageFixture.LocalStorageManager.AddConnection(fakeConnection, fakeConnectionString);

            var result = await LocalStorageFixture.ConnectionRunner.Run(new ConnectionOptions { List = true });

            var connections = LocalStorageFixture.LocalStorageManager.GetConnections();

            Assert.True(result);
            Assert.Contains(connections, x => x.ConnectionName.Equals(fakeConnection));
        }

        [AutoData]
        [Theory(DisplayName = "Can select a connection when it exists")]
        public async Task CanSelectConnection_WithCorrectName(string fakeConnectionForSelection, string fakeConnectionString)
        {
            LocalStorageFixture.LocalStorageManager.AddConnection(fakeConnectionForSelection, fakeConnectionString);

            var result = await LocalStorageFixture.ConnectionRunner.Run(new ConnectionOptions { SelectConnection = fakeConnectionForSelection });

            var selectedConnection = LocalStorageFixture.LocalStorageManager.GetSelectedConnection();

            Assert.True(result);
            Assert.Equal(fakeConnectionForSelection, selectedConnection.ConnectionName);

            await LocalStorageFixture.ConnectionRunner.Run(new ConnectionOptions { SelectConnection = DevStorageName });
        }

        [AutoData]
        [Theory(DisplayName = "Cannot select a connection when it does not exist")]
        public async Task CannotSelectConnection_WithWrongName(string fakeConnectionForSelection, string fakeConnectionString)
        {
            LocalStorageFixture.LocalStorageManager.AddConnection(fakeConnectionForSelection, fakeConnectionString);

            var result = await LocalStorageFixture.ConnectionRunner.Run(new ConnectionOptions { SelectConnection = fakeConnectionForSelection + "wrong name" });

            var selectedConnection = LocalStorageFixture.LocalStorageManager.GetSelectedConnection();

            Assert.False(result);
            Assert.NotEqual(fakeConnectionForSelection, selectedConnection.ConnectionName);
        }
    }
}