using System;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using az_lazy.Helpers;
using az_lazy.Manager;

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
                    var doc = new Document(
                        new Span("Entity Count:") { Color = ConsoleColor.Yellow }, sampledEntities.Count, "\n",
                        new Grid {
                            Color = ConsoleColor.Gray,
                            Columns = { GridLength.Auto, GridLength.Auto, GridLength.Auto, GridLength.Auto },
                            Children = {
                                new Cell("Number") { Stroke = headerThickness, Color = ConsoleColor.Yellow },
                                new Cell("PartitionKey") { Stroke = headerThickness, Color = ConsoleColor.Yellow },
                                new Cell("RowKey") { Stroke = headerThickness, TextWrap = TextWrap.WordWrap, Color = ConsoleColor.Yellow },

                                sampledEntities.First().Properties.Select(x => x.Key)

                                new Cell("Timestamp") { Stroke = headerThickness, Color = ConsoleColor.Yellow },

                                sampledEntities.Select((value, index) => {
                                    return new[] {
                                        new Cell(index + 1),
                                        new Cell(value.PartitionKey),
                                        new Cell(value.RowKey),
                                        new Cell(value.Timestamp.ToString()),
                                    };
                                })
                            }
                        }
                    );

                    ConsoleRenderer.RenderDocument(doc);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.WriteLineFailedWaiting($"Failed to watch queue {opts.Watch}");
                    ConsoleHelper.WriteLineError(ex.Message);
                }
            }
        }
    }
}