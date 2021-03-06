using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Queue.Executor
{
    public class AddMessageExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public AddMessageExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.AddQueue) || !string.IsNullOrEmpty(opts.AddMessage))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Adding message to queue {opts.AddQueue} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureStorageManager.AddMessage(selectedConnection.ConnectionString, opts.AddQueue, opts.AddMessage);

                            AnsiConsole.MarkupLine($"Adding message to queue {opts.AddQueue} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished adding message to queue {opts.AddQueue}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Adding message to queue {opts.AddQueue} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}