using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.AddQueue
{
    public class AddQueueRunner : ICommandRunner<AddQueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public AddQueueRunner(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task<bool> Run(AddQueueOptions options)
        {
            if(!string.IsNullOrEmpty(options.Name))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Creating new queue {options.Name} ...", async _ =>
                    {
                        var connection = LocalStorageManager.GetSelectedConnection();

                        try
                        {
                            await AzureStorageManager.CreateQueue(connection.ConnectionString, options.Name);

                            AnsiConsole.MarkupLine($"Creating new queue {options.Name} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished creating queue {options.Name}");
                        }
                        catch(QueueException ex)
                        {
                            AnsiConsole.MarkupLine($"Creating new queue {options.Name} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });

                return true;
            }

            return false;
        }
    }
}