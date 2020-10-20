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
        Task<bool> TestConnection(string connectionString);
        Task<List<CloudQueue>> GetQueues(string connectionString);
    }

    public class AzureStorageManager : IAzureStorageManager
    {
        public async Task<bool> TestConnection(string connectionString)
        {
            try
            {
                var queues = await GetQueues(connectionString).ConfigureAwait(false);

                return true;
            }
            catch(Exception ex)
            {
                throw new ConnectionException(ex);
            }
        }

        public async Task<List<CloudQueue>> GetQueues(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
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