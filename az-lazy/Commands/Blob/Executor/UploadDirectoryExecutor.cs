using az_lazy.Manager;
using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob.Executor
{
    public class UploadDirectoryExecutor : ICommandExecutor<BlobOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureContainerManager AzureContainerManager;

        public UploadDirectoryExecutor(
            ILocalStorageManager localStorageManager,
            IAzureContainerManager azureContainerManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureContainerManager = azureContainerManager;
        }

        public async Task Execute(BlobOptions opts)
        {
            if(!string.IsNullOrEmpty(opts.Directory) && !string.IsNullOrEmpty(opts.Container))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Syncing directory {opts.Directory} ... ", async _ =>
                    {
                        try
                        {
                            var selectedConnection = LocalStorageManager.GetSelectedConnection();
                            await AzureContainerManager.UploadBlobFromFolder(selectedConnection.ConnectionString, opts.Container, opts.Directory, opts.UploadPath);

                            AnsiConsole.MarkupLine($"Syncing directory {opts.Directory} ... [bold green]Successful[/]");
                            AnsiConsole.MarkupLine($"Finished syncing directory");
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Syncing directory {opts.Directory} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }
                    });
            }
        }
    }
}