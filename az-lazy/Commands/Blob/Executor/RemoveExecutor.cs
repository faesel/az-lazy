using az_lazy.Exceptions;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob.Executor
{
    public class RemoveExecutor : ICommandExecutor<BlobOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public RemoveExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(BlobOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.Container) && !string.IsNullOrEmpty(opts.Remove))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Removing blob {opts.Remove} from container {opts.Container} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureContainerManager.RemoveBlob(selectedConnection.ConnectionString, opts.Container, opts.Remove);

                            AnsiConsole.MarkupLine($"Removing blob {opts.Remove} from container {opts.Container} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished removing blob {opts.Remove}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Removing blob {opts.Remove} from container {opts.Container} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}
