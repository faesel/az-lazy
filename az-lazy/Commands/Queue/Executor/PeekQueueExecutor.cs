using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using az_lazy.Extensions;
using az_lazy.Helpers;
using az_lazy.Manager;
using ConsoleTables;

namespace az_lazy.Commands.Queue.Executor
{
    public class PeekQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureStorageManager AzureStorageManager;

        public PeekQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureStorageManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.Peek))
            {
                var infoMessage = $"Peeking queue {opts.Peek}";
                ConsoleHelper.WriteInfoWaiting(infoMessage, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var peekedMessages = await AzureStorageManager.PeekMessages(selectedConnection.ConnectionString, opts.Peek, opts.PeekCount).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(infoMessage);

                    var table = new ConsoleTable("Number", "Message Id", "Message", "Inserted On");

                    foreach (var message in peekedMessages.Select((value, index) => new { value, index }))
                    {
                        var rawMessageText = message.value.MessageText;
                        var isBase64 = rawMessageText.IsBase64();
                        var messageText = isBase64 ?
                            System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(rawMessageText)) :
                            rawMessageText;

                        const string replaceWith = "";
                        var removeLineBreaks = messageText.Replace("\r\n", replaceWith).Replace("\n", replaceWith).Replace("\r", replaceWith);
                        var removeExtraSpaces = Regex.Replace(removeLineBreaks, @"\s+", " ");

                        table.AddRow(message.index + 1, message.value.MessageId, removeExtraSpaces, message.value.InsertedOn);
                    }

                    table.Write();
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting($"Failed to watch queue {opts.Watch}");
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}