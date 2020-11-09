using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace az_lazy.Manager
{
    public interface IAzureContainerManager
    {
        Task<List<BlobContainerItem>> GetContainers(string connectionString);
        Task<string> CreateContainer(string connectionString, PublicAccessType publicAccessLevel, string containerName);
        Task RemoveContainer(string connectionString, string removeContainer);
    }

    public class AzureContainerManager : IAzureContainerManager
    {
        public async Task<List<BlobContainerItem>> GetContainers(string connectionString)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            List<BlobContainerItem> containerItems = new List<BlobContainerItem>();

            try
            {
                await foreach(var container in  blobServiceClient.GetBlobContainersAsync())
                {
                    containerItems.Add(container);
                }

                return containerItems;
            }
            catch (Exception ex)
            {
                throw new ContainerException(ex);
            }
        }

        public async Task<string> CreateContainer(string connectionString, PublicAccessType publicAccess, string containerName)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob).ConfigureAwait(false);

                return publicAccess == PublicAccessType.None ?
                    string.Empty :
                    $"{blobServiceClient.Uri}/{containerName}";
            }
            catch(Exception ex)
            {
                throw new ContainerException(ex);
            }
        }

        public async Task RemoveContainer(string connectionString, string removeContainer)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(connectionString);
                await blobServiceClient.DeleteBlobContainerAsync(removeContainer).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                throw new ContainerException(ex);
            }
        }
    }
}