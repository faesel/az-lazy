using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Extensions;
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
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Peeking queue {opts.Peek} ... ", async _ =>
                    {
                        try
                        {
                           var selectedConnection = LocalStorageManager.GetSelectedConnection();
                           var peekedMessages = await AzureStorageManager.PeekMessages(selectedConnection.ConnectionString, opts.Peek, opts.PeekCount);


                           if(peekedMessages == null || peekedMessages.Length == 0)
                            {
                                AnsiConsole.MarkupLine($"No messages found ... [bold red]Failed[/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"Peeking queue {opts.Peek} ... [bold green]Successful[/]");

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
                                        new Markup($"[grey93]{Markup.Escape(messageText)}[/]"),
                                        new Markup($"[grey62]{message.InsertedOn}[/]"));
                                }

                                AnsiConsole.Render(table);
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Peeking queue {opts.Peek} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}