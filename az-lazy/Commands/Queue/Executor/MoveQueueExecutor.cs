using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Queue.Executor
{
    public class MoveQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public MoveQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.From) || !string.IsNullOrEmpty(opts.To))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Moving message from {opts.From} to {opts.To} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureStorageManager.MoveMessages(selectedConnection.ConnectionString, opts.From, opts.To);

                            AnsiConsole.MarkupLine($"Moving message from {opts.From} to {opts.To} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished adding message to queue {opts.AddQueue}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Moving message from {opts.From} to {opts.To} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}