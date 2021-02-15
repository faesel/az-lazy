using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Queue.Executor
{
    public class ClearQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public ClearQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.ClearQueue))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Clearing queue {opts.ClearQueue} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureStorageManager.ClearQueue(selectedConnection.ConnectionString, opts.ClearQueue);

                            AnsiConsole.MarkupLine($"Clearing queue {opts.ClearQueue} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished clearing queue {opts.ClearQueue}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Clearing queue {opts.ClearQueue} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}