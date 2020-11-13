using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace az_lazy.Manager
{
    public interface IAzureContainerManager
    {
        Task<List<BlobContainerItem>> GetContainers(string connectionString);
        Task<string> CreateContainer(string connectionString, PublicAccessType publicAccessLevel, string containerName);
        Task RemoveContainer(string connectionString, string removeContainer);
        Task<List<TreeNode>> ContainerTree(string connectionString, string containerName, int? depth);
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

        public async Task<List<TreeNode>> ContainerTree(string connectionString, string containerName, int? depth)
        {
            try
            {
                var container = new BlobContainerClient(connectionString, containerName);

                var containerChildren = new List<TreeNode>();
                var treeNodes = new List<TreeNode>() {
                    new TreeNode {
                        Name = container.Name,
                        Children = containerChildren
                    }
                };

                await ContainerTree(container, "", 0, containerChildren, depth).ConfigureAwait(false);

                return treeNodes;
            }
            catch(Exception ex)
            {
                throw new ContainerException(ex);
            }
        }

        private async Task ContainerTree(BlobContainerClient container, string prefix, int level, List<TreeNode> children, int? depth)
        {
            const string folderDelimiter = "/";

            await foreach (var page in container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: folderDelimiter).AsPages())
            {
                foreach (var pageValues in page.Values)
                {
                    if(pageValues.IsBlob)
                    {
                        var blob = pageValues.Blob;
                        children.Add(new TreeNode {
                            Name = string.IsNullOrEmpty(prefix) ? blob.Name : blob.Name.Replace(prefix, string.Empty),
                            Information = $"({blob.Properties.ContentLength / 1024}kb) {blob.Properties.LastModified.Value.DateTime.ToShortDateString()}"
                        });
                    }

                    var incrementedLevel = level + 1;
                    if ((!depth.HasValue || incrementedLevel != depth.Value) && pageValues.IsPrefix)
                    {
                        var prefixName = string.IsNullOrEmpty(prefix) ? pageValues.Prefix.Replace(folderDelimiter, string.Empty) : pageValues.Prefix.Replace(prefix, string.Empty).Replace(folderDelimiter, string.Empty);
                        var prefixChildren = new List<TreeNode>();
                        children.Add(new TreeNode { Name = prefixName, Children = prefixChildren });
                        await ContainerTree(container, pageValues.Prefix, incrementedLevel, prefixChildren, depth).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}