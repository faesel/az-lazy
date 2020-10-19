using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Exceptions;
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
            try
            {
                var queues = await GetQueues(connectionName, connectionString).ConfigureAwait(false);

                return true;
            }
            catch(Exception ex)
            {
                throw new ConnectionException(ex);
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
                QueueResultSegment segment = await queueClient.ListQueuesSegmentedAsync(token).ConfigureAwait(false);
                token = segment.ContinuationToken;
                cloudQueueList.AddRange(segment.Results);
            }
            while (token != null);

            return cloudQueueList;
        }
    }
}