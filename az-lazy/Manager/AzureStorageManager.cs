using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Helpers;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace az_lazy.Manager
{
    public interface IAzureStorageManager
    {
        Task<bool> TestConnection(string connectionString);
        Task<List<CloudQueue>> GetQueues(string connectionString);
        Task<bool> CreateQueue(string connectionString, string queueName);
        Task<bool> RemoveQueue(string connectionString, string queueName);
        Task<bool> ClearQueue(string connectionString, string queueName);
        Task<bool> MovePoisonQueues(string connectionString, string queueName);
        Task<bool> AddMessage(string connectionString, string queueName, string message);
        Task WatchQueue(string connectionString, string watch);
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
            catch (Exception ex)
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

        public async Task<bool> CreateQueue(string connectionString, string queueName)
        {
            try
            {
                // Try to create a queue that already exists
                QueueClient queue = new QueueClient(connectionString, queueName);
                await queue.CreateAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }

        public async Task<bool> RemoveQueue(string connectionString, string queueName)
        {
            try
            {
                QueueClient queue = new QueueClient(connectionString, queueName);
                await queue.DeleteAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }

        public async Task<bool> ClearQueue(string connectionString, string queueName)
        {
            try
            {
                QueueClient queue = new QueueClient(connectionString, queueName);
                await queue.ClearMessagesAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }

        public async Task<bool> AddMessage(string connectionString, string queueName, string message)
        {
            try
            {
                QueueClient queue = new QueueClient(connectionString, queueName);
                await queue.SendMessageAsync(message).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }

        public async Task<bool> MovePoisonQueues(string connectionString, string queueName)
        {
            try
            {
                if (queueName.Contains("poison"))
                {
                    throw new QueueException("Enter the queue name rather than the poison queue name to move all the messages");
                }

                var queueClient = new QueueClient(connectionString, queueName);
                var poisonQueueClient = new QueueClient(connectionString, $"{queueName}-poison");

                var queueExists = await queueClient.ExistsAsync().ConfigureAwait(false);
                var poisonQueueExists = await poisonQueueClient.ExistsAsync().ConfigureAwait(false);

                if (!queueExists || !poisonQueueExists)
                {
                    throw new QueueException("Either the queue or its related poison queue do not exist");
                }

                var poisonProperties = await poisonQueueClient.GetPropertiesAsync().ConfigureAwait(false);
                var poisonQueueCount = poisonProperties.Value.ApproximateMessagesCount;

                ConsoleHelper.WriteLineInfo($"Found {poisonQueueCount} poison messages");
                const string message = "Moving messages";
                ConsoleHelper.WriteInfoWaiting(message, true);

                QueueMessage[] poisonMessages = null;
                int processed = 0;

                do
                {
                    poisonMessages = await poisonQueueClient.ReceiveMessagesAsync(maxMessages: 32).ConfigureAwait(false);

                    foreach (var poisonMessage in poisonMessages)
                    {
                        await queueClient.SendMessageAsync(poisonMessage.MessageText).ConfigureAwait(false);
                        await poisonQueueClient.DeleteMessageAsync(poisonMessage.MessageId, poisonMessage.PopReceipt).ConfigureAwait(false);
                    }

                    processed += poisonMessages.Length;

                    if (poisonMessages.Length > 0)
                    {
                        ConsoleHelper.WriteInfoWaitingPct(message, processed / poisonQueueCount * 100, true);
                    }

                }
                while (poisonMessages.Length > 0);

                ConsoleHelper.WriteLineSuccessWaiting(message);

                return true;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }

        public async Task WatchQueue(string connectionString, string watch)
        {
            try
            {
                var queueClient = new QueueClient(connectionString, watch);
                var queueCount = 0;

                while (true)
                {
                    var queueProperties = await queueClient.GetPropertiesAsync().ConfigureAwait(false);
                    if (queueCount != queueProperties.Value.ApproximateMessagesCount)
                    {
                        var infomessage = queueCount == 0 ?
                            $"{queueProperties.Value.ApproximateMessagesCount} already in queue, waiting for more ..." :
                            $"Queue message added - {queueProperties.Value.ApproximateMessagesCount} in queue - {DateTime.UtcNow:MM/dd/yyyy HH:mm:ss}";

                        ConsoleHelper.WriteLineNormal(infomessage);

                        queueCount = queueProperties.Value.ApproximateMessagesCount;
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }
    }
}