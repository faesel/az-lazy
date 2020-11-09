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
    public interface IAzureQueueManager
    {
        Task<List<CloudQueue>> GetQueues(string connectionString);
        Task<bool> CreateQueue(string connectionString, string queueName);
        Task<bool> RemoveQueue(string connectionString, string queueName);
        Task<bool> ClearQueue(string connectionString, string queueName);
        Task<bool> MovePoisonQueues(string connectionString, string queueName);
        Task<bool> AddMessage(string connectionString, string queueName, string message);
        Task WatchQueue(string connectionString, string watch);
        Task<PeekedMessage[]> PeekMessages(string connectionString, string queueToView, int viewCount);
        Task<bool> MoveMessages(string connectionString, string from, string to);
    }

    public class AzureQueueManager : IAzureQueueManager
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

                var poisonQueueName = $"{queueName}-poison";

                var queueClient = new QueueClient(connectionString, queueName);
                var poisonQueueClient = new QueueClient(connectionString,poisonQueueName);

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

        public async Task<PeekedMessage[]> PeekMessages(string connectionString, string queueToView, int viewCount)
        {
            try
            {
                var queueClient = new QueueClient(connectionString, queueToView);
                var queueExists = await queueClient.ExistsAsync().ConfigureAwait(false);

                if (!queueExists)
                {
                    throw new QueueException("Either the queue or its related poison queue do not exist");
                }

                viewCount = viewCount == 0 ? 1 : viewCount;
                viewCount = viewCount > 32 ? 32 : viewCount;

                var messages = await queueClient.PeekMessagesAsync(maxMessages: viewCount).ConfigureAwait(false);

                return messages.Value;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }

        public async Task<bool> MoveMessages(string connectionString, string from, string to)
        {
            try
            {
                var fromQueueClient = new QueueClient(connectionString, from);
                var toQueueClient = new QueueClient(connectionString, to);

                var fromQueueExists = await fromQueueClient.ExistsAsync().ConfigureAwait(false);
                var toQueueExists = await toQueueClient.ExistsAsync().ConfigureAwait(false);

                if (!fromQueueExists || !toQueueExists)
                {
                    throw new QueueException($"Either {from} or {to} queue does not exist");
                }

                var fromProperties = await fromQueueClient.GetPropertiesAsync().ConfigureAwait(false);
                var fromQueueCount = fromProperties.Value.ApproximateMessagesCount;

                ConsoleHelper.WriteLineInfo($"Found {fromQueueCount} messages in {from} queue");
                const string message = "Moving messages";
                ConsoleHelper.WriteInfoWaiting(message, true);

                QueueMessage[] fromMessagees = null;
                int processed = 0;

                do
                {
                    fromMessagees = await fromQueueClient.ReceiveMessagesAsync(maxMessages: 32).ConfigureAwait(false);

                    foreach (var fromMessage in fromMessagees)
                    {
                        await toQueueClient.SendMessageAsync(fromMessage.MessageText).ConfigureAwait(false);
                        await fromQueueClient.DeleteMessageAsync(fromMessage.MessageId, fromMessage.PopReceipt).ConfigureAwait(false);
                    }

                    processed += fromMessagees.Length;

                    if (fromMessagees.Length > 0)
                    {
                        ConsoleHelper.WriteInfoWaitingPct(message, processed / fromQueueCount * 100, true);
                    }
                }
                while (fromMessagees.Length > 0);

                ConsoleHelper.WriteLineSuccessWaiting(message);

                return true;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }
    }
}