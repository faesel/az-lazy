using az_lazy.Manager;
using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob.Executor
{
    public class UploadExecutor : ICommandExecutor<BlobOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public UploadExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(BlobOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.Container) && !string.IsNullOrEmpty(opts.UploadFile))
            {
                 await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Uploading {opts.UploadFile} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureContainerManager.UploadBlob(selectedConnection.ConnectionString, opts.Container, opts.UploadFile, opts.UploadPath);

                            AnsiConsole.MarkupLine($"Uploading {opts.UploadFile} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished uploading {opts.UploadFile}");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Uploading {opts.UploadFile} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}
