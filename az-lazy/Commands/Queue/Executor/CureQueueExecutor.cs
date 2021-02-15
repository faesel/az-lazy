using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Queue.Executor
{
    public class CureQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public CureQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.CureQueue))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Clearing poison queue {opts.CureQueue}-poison ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureStorageManager.MovePoisonQueues(selectedConnection.ConnectionString, opts.CureQueue);

                            AnsiConsole.MarkupLine($"Clearing poison queue {opts.CureQueue}-poison ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished moving poison queue messages");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Clearing poison queue {opts.CureQueue}-poison ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}