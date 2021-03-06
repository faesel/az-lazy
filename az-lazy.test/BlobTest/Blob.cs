using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using az_lazy.Commands.Blob;
using Azure.Storage.Blobs.Models;
using Xunit;

namespace az_lazy.test.BlobTest
{
    [Collection("LocalStorage")]
    [Trait("Blob", "Integration test for blobs")]
    public class Blob
    {
        private const string DevStorageConnectionString = "UseDevelopmentStorage=true";

        private readonly LocalStorageFixture LocalStorageFixture;

        public Blob(LocalStorageFixture localStorageFixture)
        {
            this.LocalStorageFixture = localStorageFixture;
        }

        [Fact(DisplayName = "Can successfully upload a blob to a container")]
        public async Task CanUploadNewBlob()
        {
            const string containerName = "newuploadcontainer";
            const string fileName = "test.txt";

            try
            {
                await LocalStorageFixture.AzureContainerManager.RemoveContainer(DevStorageConnectionString, containerName);
            }
            catch (Exception)
            {
                //Suppress, its most likely because the container doesnt exist
            }
            await LocalStorageFixture.AzureContainerManager.CreateContainer(DevStorageConnectionString, PublicAccessType.None, containerName);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "Files", fileName);

            await LocalStorageFixture.BlobRunner.Run(new BlobOptions { Container = containerName, UploadFile = path });

            var blobNodes = await LocalStorageFixture.AzureContainerManager.ContainerTree(DevStorageConnectionString, containerName, depth: 1, false, string.Empty);

            Assert.Single(blobNodes[0].Children);
            Assert.Equal(fileName, blobNodes[0].Children[0].Name);
        }

        [Fact(DisplayName = "Can successfully delete a blob from a container")]
        public async Task CanDeleteBlob()
        {
            const string containerName = "deleteblobcontainer";
            const string fileName = "test.txt";

            try
            {
                await LocalStorageFixture.AzureContainerManager.RemoveContainer(DevStorageConnectionString, containerName);
            }
            catch (Exception)
            {
                //Suppress, its most likely because the container doesnt exist
            }
            await LocalStorageFixture.AzureContainerManager.CreateContainer(DevStorageConnectionString, PublicAccessType.None, containerName);

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestData", "Files", fileName);
            await LocalStorageFixture.AzureContainerManager.UploadBlob(DevStorageConnectionString,  containerName, path, string.Empty);

            await LocalStorageFixture.BlobRunner.Run(new BlobOptions { Container = containerName, Remove = fileName });

            var blobNodes = await LocalStorageFixture.AzureContainerManager.ContainerTree(DevStorageConnectionString, containerName, depth: 1, false, string.Empty);

            Assert.Empty(blobNodes[0].Children);
        }
    }
}