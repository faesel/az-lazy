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
using Spectre.Console;

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
                var queues = await GetQueues(connectionString);

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
                QueueResultSegment segment = await queueClient.ListQueuesSegmentedAsync(token);
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
                await queue.CreateAsync();

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
                await queue.DeleteAsync();

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
                await queue.ClearMessagesAsync();

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
                await queue.SendMessageAsync(message);

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

                var queueExists = await queueClient.ExistsAsync();
                var poisonQueueExists = await poisonQueueClient.ExistsAsync();

                if (!queueExists || !poisonQueueExists)
                {
                    throw new QueueException("Either the queue or its related poison queue do not exist");
                }

                var poisonProperties = await poisonQueueClient.GetPropertiesAsync();
                var poisonQueueCount = poisonProperties.Value.ApproximateMessagesCount;

                AnsiConsole.MarkupLine($"[grey]Found {poisonQueueCount} poison messages[/]");
                const string message = "Moving messages";
                AnsiConsole.Markup($"[grey]{message} ... [/]");

                QueueMessage[] poisonMessages = null;
                int processed = 0;

                do
                {
                    poisonMessages = await poisonQueueClient.ReceiveMessagesAsync(maxMessages: 32);

                    foreach (var poisonMessage in poisonMessages)
                    {
                        await queueClient.SendMessageAsync(poisonMessage.MessageText);
                        await poisonQueueClient.DeleteMessageAsync(poisonMessage.MessageId, poisonMessage.PopReceipt);
                    }

                    processed += poisonMessages.Length;

                    if (poisonMessages.Length > 0)
                    {
                        AnsiConsole.Markup($"[grey]{message} ... %{processed / poisonQueueCount * 100}[/]");
                    }
                }
                while (poisonMessages.Length > 0);

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
                    var queueProperties = await queueClient.GetPropertiesAsync();
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
                var queueExists = await queueClient.ExistsAsync();

                if (!queueExists)
                {
                    throw new QueueException("Either the queue or its related poison queue do not exist");
                }

                viewCount = viewCount == 0 ? 1 : viewCount;
                viewCount = viewCount > 32 ? 32 : viewCount;

                var messages = await queueClient.PeekMessagesAsync(maxMessages: viewCount);

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

                var fromQueueExists = await fromQueueClient.ExistsAsync();
                var toQueueExists = await toQueueClient.ExistsAsync();

                if (!fromQueueExists || !toQueueExists)
                {
                    throw new QueueException($"Either {from} or {to} queue does not exist");
                }

                var fromProperties = await fromQueueClient.GetPropertiesAsync();
                var fromQueueCount = fromProperties.Value.ApproximateMessagesCount;

                AnsiConsole.MarkupLine($"[grey]Found {fromQueueCount} messages in {from} queue[/]");

                const string message = "Moving messages";
                AnsiConsole.Markup($"[grey]{message} ...[/]");

                QueueMessage[] fromMessagees = null;
                int processed = 0;

                do
                {
                    fromMessagees = await fromQueueClient.ReceiveMessagesAsync(maxMessages: 32);

                    foreach (var fromMessage in fromMessagees)
                    {
                        await toQueueClient.SendMessageAsync(fromMessage.MessageText);
                        await fromQueueClient.DeleteMessageAsync(fromMessage.MessageId, fromMessage.PopReceipt);
                    }

                    processed += fromMessagees.Length;

                    if (fromMessagees.Length > 0)
                    {
                        AnsiConsole.Markup($"[grey]{message} ... %{processed / fromQueueCount * 100}[/]");
                    }
                }
                while (fromMessagees.Length > 0);

                return true;
            }
            catch (Exception ex)
            {
                throw new QueueException(ex);
            }
        }
    }
}