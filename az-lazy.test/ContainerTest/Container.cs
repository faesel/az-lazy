using System.Threading.Tasks;
using az_lazy.Commands.AddContainer;
using az_lazy.Commands.Container.Executor;
using Azure.Storage.Blobs.Models;
using Xunit;

namespace az_lazy.test.ContainerTest
{
    [Collection("LocalStorage")]
    [Trait("Container", "Integration test for containers")]
    public class Container
    {
        private const string DevStorageConnectionString = "UseDevelopmentStorage=true";

        private readonly LocalStorageFixture LocalStorageFixture;

        public Container(LocalStorageFixture localStorageFixture)
        {
            this.LocalStorageFixture = localStorageFixture;
        }

        [Fact(DisplayName = "Can successfully create a new container")]
        public async Task CanCreateNewContainer()
        {
            const string containerName = "newcontainer";

            await LocalStorageFixture.AddContainerRunner.Run(new AddContainerOptions { Name = containerName }).ConfigureAwait(false);
            var containerList = await LocalStorageFixture.AzureContainerManager.GetContainers(DevStorageConnectionString).ConfigureAwait(false);

            Assert.Contains(containerList, x => x.Name.Equals(containerName));
        }

        [Fact(DisplayName = "Can successfully remove a container container")]
        public async Task CanRemoveContainerSuccessfully()
        {
            const string containerName = "deletecontainer";

            await LocalStorageFixture.AzureContainerManager.CreateContainer(DevStorageConnectionString, PublicAccessType.None, containerName).ConfigureAwait(false);

            await LocalStorageFixture.ContainerRunner.Run(new ContainerOptions { RemoveContainer = containerName }).ConfigureAwait(false);
            var containerList = await LocalStorageFixture.AzureContainerManager.GetContainers(DevStorageConnectionString).ConfigureAwait(false);

            Assert.DoesNotContain(containerList, x => x.Name.Equals(containerName));
        }
    }
}