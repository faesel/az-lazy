using az_lazy.Manager;
using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.Table.Executor
{
    public class RemoveExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public RemoveExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Remove))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Removing table {opts.Remove} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            var result = await AzureTableManager.Remove(selectedConnection.ConnectionString, opts.Remove);

                            if(!result) {
                                AnsiConsole.MarkupLine($"Removing table {opts.Remove} ... [bold red]Failed[/]");
                                AnsiConsole.MarkupLine($"[bold red]Check to ensure the table name is correct[/]");
                            }
                            else {
                                AnsiConsole.MarkupLine($"Removing table {opts.Remove} ... [bold green]Successful[/]");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Removing table {opts.Remove} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}