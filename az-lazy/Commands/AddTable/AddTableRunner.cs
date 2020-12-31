using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.AddTable
{
    public class AddTableRunner : ICommandRunner<AddTableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public AddTableRunner(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task<bool> Run(AddTableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Name))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Creating table {opts.Name} ...", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureTableManager.Create(selectedConnection.ConnectionString, opts.Name);

                            AnsiConsole.MarkupLine($"Creating table {opts.Name} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished creating table {opts.Name}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Creating table {opts.Name} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }

            return true;
        }
    }
}