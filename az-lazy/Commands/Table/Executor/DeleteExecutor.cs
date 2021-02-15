using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Table.Executor
{
    public class DeleteExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public DeleteExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Delete))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Deleting rows ... ", async _ =>
                    {
                        try
                        {
                            if(!string.IsNullOrEmpty(opts.PartitionKey) || !string.IsNullOrEmpty(opts.RowKey))
                            {
                                var selectedConnection = LocalStorageManager.GetSelectedConnection();
                                var deleteCount = await AzureTableManager.DeleteRow(selectedConnection.ConnectionString, opts.Delete, opts.PartitionKey, opts.RowKey);

                                AnsiConsole.MarkupLine($"Deleting rows ... [bold green]Successful[/]");
                                AnsiConsole.MarkupLine($"Finished deleting rows, {deleteCount} rows removed");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"[bold red]PartitionKey or RowKey are required in order to delete rows[/]");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Deleting rows ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}