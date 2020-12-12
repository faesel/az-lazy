using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Table.Executor
{
    public class SampleExecutor : ICommandExecutor<TableOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureTableManager AzureTableManager;

        public SampleExecutor(
            ILocalStorageManager localStorageManager,
            IAzureTableManager azureTableManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureTableManager = azureTableManager;
        }

        public async Task Execute(TableOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Sample))
            {
                var infoMessage = $"Sampleing table {opts.Sample}";
                ConsoleHelper.WriteInfoWaiting(infoMessage, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var sampledEntities = await AzureTableManager.Sample(selectedConnection.ConnectionString, opts.Sample, opts.SampleCount).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(infoMessage);

                    if(sampledEntities == null || sampledEntities.Count == 0)
                    {
                        ConsoleHelper.WriteInfo("No rows found");
                    }

                    var table = new Spectre.Console.Table();
                    table.Border(TableBorder.DoubleEdge);
                    table.Expand();
                    table.LeftAligned();

                    table.AddColumn("[yellow]Number[/]");
                    table.AddColumn("[yellow]Partition Key[/]");
                    table.AddColumn("[yellow]Row Key[/]");

                    foreach(var column in sampledEntities[0].Properties)
                    {
                        table.AddColumn($"[yellow]{column.Key}[/]");
                    }
                    table.AddColumn("[yellow]Timestamp[/]");

                    foreach(var row in sampledEntities)
                    {
                        var values = new List<Markup>();
                        values.Add(new Markup($"[grey62]{sampledEntities.IndexOf(row) + 1}[/]"));
                        values.Add(new Markup($"[grey93]{row.PartitionKey}[/]"));
                        values.Add(new Markup($"[grey93]{row.RowKey}[/]"));
                        values.AddRange(row.Properties.Select(x => new Markup($"[grey62]{x.Value}[/]")).ToList());
                        values.Add(new Markup($"[grey62]{row.Timestamp}[/]"));

                        table.AddRow(values.ToArray());
                    }
                    
                    AnsiConsole.Render(table);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineError(ex.Message);
                    ConsoleHelper.WriteLineFailedWaiting($"Failed to sample table {opts.Sample}");
                }
            }
        }
    }
}