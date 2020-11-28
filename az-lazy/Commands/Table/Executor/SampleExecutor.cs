using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using az_lazy.Helpers;
using az_lazy.Manager;
using az_lazy.Model;

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

                    var headerThickness = new LineThickness(LineWidth.Double, LineWidth.Double, LineWidth.Double, LineWidth.Double);
                    var grid = new Grid {
                        Color = ConsoleColor.Gray,
                    };

                    var headingCells = new List<Cell>();
                    headingCells.Add(new Cell("Number") { Color = ConsoleColor.Yellow });
                    headingCells.Add(new Cell("Partition Key") { Color = ConsoleColor.Yellow });
                    headingCells.Add(new Cell("Row Key") { Color = ConsoleColor.Yellow });

                    foreach(var column in sampledEntities[0].Properties)
                    {
                        headingCells.Add(new Cell(column.Key) { Color = ConsoleColor.Yellow });
                    }
                    headingCells.Add(new Cell("Timestamp") { Color = ConsoleColor.Yellow });
                    grid.Children.Add(headingCells.ToArray());

                    foreach(var cell in headingCells)
                    {
                        grid.Columns.Add(new Column() { Name = cell.Name, Width = GridLength.Auto });
                        Console.WriteLine(cell.Name);
                    }

                    foreach(var row in sampledEntities)
                    {
                        var cellList = new List<Cell>();
                        cellList.Add(new Cell(sampledEntities.IndexOf(row) + 1));
                        cellList.Add(new Cell(row.PartitionKey));
                        cellList.Add(new Cell(row.RowKey));

                        foreach(var rowData in row.Properties)
                        {
                            cellList.Add(new Cell(rowData.Value));
                        }
                        cellList.Add(new Cell(row.Timestamp));

                        grid.Children.Add(cellList.ToArray());
                    }

                    var doc = new Document(
                        grid
                    );

                    ConsoleRenderer.RenderDocument(doc);
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