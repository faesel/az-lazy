using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Table.Executor
{
    public class QueryExecutor: ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public QueryExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Query))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Querying table {opts.Query} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            var sampledEntities = await AzureTableManager.Query(
                                selectedConnection.ConnectionString,
                                opts.Query,
                                opts.PartitionKey,
                                opts.RowKey,
                                opts.Take);

                            if(sampledEntities == null || sampledEntities.Count == 0)
                            {
                                AnsiConsole.MarkupLine($"No rows found ... [bold red]Failed[/]");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"Querying table {opts.Query} ... [bold green]Successful[/]");

                                var table = new Spectre.Console.Table();
                                table.Border(TableBorder.DoubleEdge);
                                table.Expand();
                                table.LeftAligned();

                                table.AddColumn("[yellow]Number[/]");
                                table.AddColumn("[yellow]Partition Key[/]");
                                table.AddColumn("[yellow]Row Key[/]");

                                foreach (var column in sampledEntities[0].Properties)
                                {
                                    table.AddColumn($"[yellow]{column.Key}[/]");
                                }
                                table.AddColumn("[yellow]Timestamp[/]");

                                foreach (var row in sampledEntities)
                                {
                                    var values = new List<Markup>();
                                    values.Add(new Markup($"[grey62]{sampledEntities.IndexOf(row) + 1}[/]"));
                                    values.Add(new Markup($"[grey93]{row.PartitionKey}[/]"));
                                    values.Add(new Markup($"[grey93]{row.RowKey}[/]"));
                                    values.AddRange(row.Properties.Select(x => new Markup($"[grey62]{Markup.Escape(x.Value.ToString())}[/]")).ToList());
                                    values.Add(new Markup($"[grey62]{row.Timestamp}[/]"));

                                    table.AddRow(values.ToArray());
                                }

                                AnsiConsole.Render(table);
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Querying table {opts.Query} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}