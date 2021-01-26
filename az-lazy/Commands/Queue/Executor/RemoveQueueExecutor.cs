using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Queue.Executor
{
    public class RemoveQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public RemoveQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.RemoveQueue))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Removing queue {opts.RemoveQueue} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureStorageManager.RemoveQueue(selectedConnection.ConnectionString, opts.RemoveQueue);

                            AnsiConsole.MarkupLine($"Removing queue {opts.RemoveQueue} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished removing queue {opts.RemoveQueue}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Removing queue {opts.RemoveQueue} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}