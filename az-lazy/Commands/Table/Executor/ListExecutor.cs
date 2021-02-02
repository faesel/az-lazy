using System;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Table.Executor
{
    public class ListExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public ListExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if (opts.List)
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Fetching tables ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            var tables = await AzureTableManager.GetTables(selectedConnection.ConnectionString);

                            if (tables.Count > 0)
                            {
                                AnsiConsole.MarkupLine($"Fetching tables ... [bold green]Successful[/]");
                                AnsiConsole.Render(new Rule());

                                if(!string.IsNullOrEmpty(opts.Contains))
                                {
                                    tables = tables.Where(x => x.Name.Contains(opts.Contains)).ToList();
                                }

                                foreach (var table in tables)
                                {
                                    AnsiConsole.MarkupLine(table.Name);
                                }
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"Fetching tables ... [bold red]Failed[/]");
                                AnsiConsole.MarkupLine($"[bold red]No queues found[/]");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Fetching tables ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}