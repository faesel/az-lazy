using System;
using System.Linq;
using System.Threading.Tasks;
using Alba.CsConsoleFormat;
using az_lazy.Extensions;
using az_lazy.Helpers;
using az_lazy.Manager;

namespace az_lazy.Commands.Queue.Executor
{
    public class PeekQueueExecutor : ICommandExecutor<QueueOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureQueueManager AzureStorageManager;

        public PeekQueueExecutor(
            ILocalStorageManager localStorageManager,
            IAzureQueueManager azureStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureStorageManager = azureStorageManager;
        }

        public async Task Execute(QueueOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.Peek))
            {
                var infoMessage = $"Peeking queue {opts.Peek}";
                ConsoleHelper.WriteInfoWaiting(infoMessage, true);

                try
                {
                    var selectedConnection = LocalStorageManager.GetSelectedConnection();
                    var peekedMessages = await AzureStorageManager.PeekMessages(selectedConnection.ConnectionString, opts.Peek, opts.PeekCount).ConfigureAwait(false);

                    ConsoleHelper.WriteLineSuccessWaiting(infoMessage);

                    var headerThickness = new LineThickness(LineWidth.Double, LineWidth.Double, LineWidth.Double, LineWidth.Double);
                    var doc = new Document(
                        new Span("Message Count:") { Color = ConsoleColor.Yellow }, peekedMessages.Length, "\n",
                        new Grid {
                            Color = ConsoleColor.Gray,
                            Columns = { GridLength.Auto, GridLength.Auto, GridLength.Auto, GridLength.Auto },
                            Children = {

                                new Cell("Number") { Stroke = headerThickness, Color = ConsoleColor.Yellow },
                                new Cell("Message Id") { Stroke = headerThickness, Color = ConsoleColor.Yellow },
                                new Cell("Message Text") { Stroke = headerThickness, TextWrap = TextWrap.WordWrap, Color = ConsoleColor.Yellow },
                                new Cell("Inserted On") { Stroke = headerThickness, Color = ConsoleColor.Yellow },

                                peekedMessages.Select((value, index) => {
                                    var rawMessageText = value.MessageText;
                                    var isBase64 = rawMessageText.IsBase64();
                                    var messageText = isBase64 ?
                                        System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(rawMessageText)) :
                                        rawMessageText;

                                    return new[] {
                                        new Cell(index + 1),
                                        new Cell(value.MessageId),
                                        new Cell(messageText),
                                        new Cell(value.InsertedOn),
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