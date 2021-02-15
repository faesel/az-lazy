using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MimeTypes;
using Spectre.Console;
using az_lazy.Commands.Container.Dto;

namespace az_lazy.Manager
{
    public interface IAzureContainerManager
    {
        Task<List<BlobContainerItem>> GetContainers(string connectionString);
        Task<string> CreateContainer(string connectionString, PublicAccessType publicAccessLevel, string containerName);
        Task RemoveContainer(string connectionString, string removeContainer);
        Task<List<BlobTreeNode>> ContainerTree(string connectionString, string containerName, int? depth, bool detailed, string prefix);
        Task RemoveBlob(string connectionString, string containerName, string blobToRemove);
        Task UploadBlob(string connectionString, string containerName, string fileToUpload, string containerLocation);
        Task UploadBlobFromFolder(string connectionString, string containerName, string searchDirectory, string containerLocation);
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
                await blobServiceClient.CreateBlobContainerAsync(containerName, PublicAccessType.Blob);

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
                await blobServiceClient.DeleteBlobContainerAsync(removeContainer);
            }
            catch(Exception ex)
            {
                throw new ContainerException(ex);
            }
        }

        public async Task<List<BlobTreeNode>> ContainerTree(string connectionString, string containerName, int? depth, bool detailed, string prefix)
        {
            try
            {
                var container = new BlobContainerClient(connectionString, containerName);

                var containerChildren = new List<BlobTreeNode>();
                var treeNodes = new List<BlobTreeNode>() {
                    new BlobTreeNode {
                        Name = container.Name,
                        Children = containerChildren
                    }
                };

                await ContainerTree(container, prefix, 0, containerChildren, depth, detailed, prefix);

                return treeNodes;
            }
            catch(Exception ex)
            {
                throw new ContainerException(ex);
            }
        }

        private async Task ContainerTree(BlobContainerClient container, string prefix, int level, List<BlobTreeNode> children, int? depth, bool detailed, string prefixSearch)
        {
            const string folderDelimiter = "/";

            await foreach (var page in container.GetBlobsByHierarchyAsync(prefix: prefix, delimiter: folderDelimiter).AsPages())
            {
                foreach (var pageValues in page.Values)
                {
                    if(pageValues.IsBlob)
                    {
                        var blob = pageValues.Blob;
                        children.Add(new BlobTreeNode {
                            Name = string.IsNullOrEmpty(prefix) ? blob.Name : blob.Name.Replace(prefix, string.Empty),
                            Information = detailed ? $"({blob.Properties.ContentLength / 1024}kb) {blob.Properties.LastModified.Value.DateTime.ToShortDateString()}" : string.Empty
                        });
                    }

                    var incrementedLevel = level + 1;
                    if ((!depth.HasValue || incrementedLevel != depth.Value) && pageValues.IsPrefix)
                    {
                        var prefixName = string.IsNullOrEmpty(prefix) || prefix.Equals(prefixSearch) ?
                            pageValues.Prefix.Replace(folderDelimiter, string.Empty) :
                            pageValues.Prefix.Replace(prefix, string.Empty).Replace(folderDelimiter, string.Empty);

                        var prefixChildren = new List<BlobTreeNode>();
                        children.Add(new BlobTreeNode { Name = prefixName, Children = prefixChildren });
                        await ContainerTree(container, pageValues.Prefix, incrementedLevel, prefixChildren, depth, detailed, prefixSearch);
                    }
                }
            }
        }

        public async Task RemoveBlob(string connectionString, string containerName, string blobToRemove)
        {
            try
            {
                var container = new BlobContainerClient(connectionString, containerName);
                await container.DeleteBlobIfExistsAsync(blobToRemove);
            }
            catch (Exception ex)
            {
                throw new ContainerException(ex);
            }
        }

        public async Task UploadBlob(string connectionString, string containerName, string fileToUpload, string containerLocation)
        {
            try
            {
                var fileName = Path.GetFileName(fileToUpload);

                if (string.IsNullOrEmpty(fileName))
                {
                    throw new ContainerException($"Path {fileToUpload} does not contain a file name");
                }

                var fileType = MimeTypeMap.GetMimeType(Path.GetExtension(fileToUpload));
                var uploadPath = string.IsNullOrEmpty(containerLocation) ? string.Empty : $"{containerLocation}";

                if (!uploadPath.EndsWith("/") && !string.IsNullOrEmpty(containerLocation))
                {
                    uploadPath += "/";
                }

                using FileStream fileStream = File.OpenRead(fileToUpload);
                var container = new BlobContainerClient(connectionString, containerName);
                var blobClient = container.GetBlobClient($"{uploadPath}{fileName}");

                await blobClient.UploadAsync(fileStream, new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = fileType
                    }
                });
            }
            catch (Exception ex)
            {
                throw new ContainerException(ex);
            }
        }

        public async Task UploadBlobFromFolder(string connectionString, string containerName, string searchDirectory, string containerLocation)
        {
            try
            {
                var files = Directory.GetFiles(searchDirectory, "*", SearchOption.AllDirectories).ToList();

                AnsiConsole.MarkupLine($"Found {files.Count} files, beginning upload ...");

                double lastPercentage = 0;
                double percentageComplete = 0;
                List<double> progress = new List<double>();

                await AnsiConsole.Progress()
                    .AutoClear(false)
                    .Columns(new ProgressColumn[]
                    {
                        new TaskDescriptionColumn(),
                        new ProgressBarColumn(),
                        new PercentageColumn(),
                        new RemainingTimeColumn(),
                        new SpinnerColumn()
                    })
                    .StartAsync(async ctx =>
                    {
                        var task1 = ctx.AddTask($"[green]Uploading Fles[/]");

                        while (!ctx.IsFinished)
                        {
                            foreach (var file in files)
                            {
                                var fileName = Path.GetFileName(file);

                                var subfolderAndFile = file
                                    .Replace(searchDirectory, string.Empty)
                                    .Replace(fileName, string.Empty);

                                if (!string.IsNullOrEmpty(containerLocation))
                                    subfolderAndFile = containerLocation + subfolderAndFile;

                                if (subfolderAndFile.StartsWith(@"\"))
                                    subfolderAndFile = subfolderAndFile[1..];

                                if (subfolderAndFile.EndsWith(@"\"))
                                    subfolderAndFile = subfolderAndFile[0..^1];

                                await UploadBlob(connectionString, containerName, file, subfolderAndFile);

                                var fileIndex = files.IndexOf(file) + 1;
                                percentageComplete = ((double)fileIndex) / files.Count * 100;
                                task1.Increment(percentageComplete - lastPercentage);
                                lastPercentage = percentageComplete;

                                task1.Description = @$"[green]Uploading {fileIndex} of {files.Count} Fles[/]";
                            }
                        }
                    });
            }
            catch(Exception ex)
            {
                throw new ContainerException(ex);
            }
        }
    }
}