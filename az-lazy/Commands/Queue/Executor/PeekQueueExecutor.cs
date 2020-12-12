using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Extensions;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Queue.Executor
{
    public class PeekQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public PeekQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
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

                    var table = new Spectre.Console.Table();
                    table.Border(TableBorder.DoubleEdge);
                    table.Expand();
                    table.LeftAligned();

                    table.AddColumn("[yellow]Number[/]");
                    table.AddColumn("[yellow]Message Id[/]");
                    table.AddColumn("[yellow]Message Text[/]");
                    table.AddColumn("[yellow]Inserted On[/]");

                    var peekedMessageList = peekedMessages.ToList();
                    foreach (var message in peekedMessageList)
                    {
                        var index = peekedMessageList.IndexOf(message);

                        var rawMessageText = message.MessageText;
                        var isBase64 = rawMessageText.IsBase64();
                        var messageText = isBase64 ?
                            System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(rawMessageText)) :
                            rawMessageText;

                        table.AddRow(
                            new Markup($"[grey62]{index + 1}[/]"),
                            new Markup($"[grey93]{message.MessageId}[/]"),
                            new Markup($"[grey93]{messageText}[/]"),
                            new Markup($"[grey62]{message.InsertedOn}[/]"));
                    }

                    AnsiConsole.Render(table);
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