using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;

namespace az_lazy.Manager
{
    public interface IAzureStorageManager
    {
        Task<bool> TestConnection(string connectionName, string connectionString);
        Task<List<CloudQueue>> GetQueues(string connectionName, string connectionString);
    }

    public class AzureStorageManager : IAzureStorageManager
    {
        public async Task<bool> TestConnection(string connectionName, string connectionString)
        {
            var isSuccesfull = true;

            try
            {
                var queues = await GetQueues(connectionName, connectionString);

                return true;
            }
            catch(Exception ex)
            {
                //TODO: highlight error
                return false;
            }
        }

        public async Task<List<CloudQueue>> GetQueues(string connectionName, string connectionString)
        {
            var storageCredentials = new StorageCredentials(connectionName, connectionString);
            var storageAccount = new CloudStorageAccount(storageCredentials, true);
            var queueClient = storageAccount.CreateCloudQueueClient();

            QueueContinuationToken token = null;
            List<CloudQueue> cloudQueueList = new List<CloudQueue>();

            do
            {
                QueueResultSegment segment = await queueClient.ListQueuesSegmentedAsync(token);
                token = segment.ContinuationToken;
                cloudQueueList.AddRange(segment.Results);
            }
            while (token != null);

            return cloudQueueList;
        }
    }
}