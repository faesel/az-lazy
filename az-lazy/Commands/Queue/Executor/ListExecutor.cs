using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Queue.Executor
{
    public class ListExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (opts.List)
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Fetching queues ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            var queueList = await AzureStorageManager.GetQueues(selectedConnection.ConnectionString);

                            if (queueList.Count > 0)
                            {
                                AnsiConsole.MarkupLine($"Fetching queues ... [bold green]Successful[/]");
                                AnsiConsole.Render(new Rule());

                                if (!string.IsNullOrEmpty(opts.Contains))
                                {
                                    queueList = queueList.Where(x => x.Name.Contains(opts.Contains)).ToList();
                                }

                                foreach (var queue in queueList)
                                {
                                    await queue.FetchAttributesAsync();

                                    var queueCount = queue.ApproximateMessageCount ?? 0;
                                    var isPoisonQueue = queue.Name.EndsWith("poison");
                                    var queueInformation = $"{queue.Name} ({queueCount})";

                                    if (isPoisonQueue)
                                    {
                                        AnsiConsole.MarkupLine($"[red]{queueInformation}[/]");
                                    }
                                    else
                                    {
                                        AnsiConsole.MarkupLine($"[green]{queueInformation}[/]");
                                    }
                                }
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"Fetching queues ... [bold red]Failed[/]");
                                AnsiConsole.MarkupLine($"[bold red]No queues found[/]");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Fetching queues ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}