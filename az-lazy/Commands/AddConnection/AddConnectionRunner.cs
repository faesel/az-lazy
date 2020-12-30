using System.Threading.Tasks;
using az_lazy.Exceptions;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.AddConnection
{
    public class AddConnectionRunner : ICommandRunner<AddConnectionOptions>
    {
        private readonly ILocalStorageManager LocalStorageManager;
        private readonly IAzureConnectionManager AzureConnectionManager;

        public AddConnectionRunner(
            ILocalStorageManager localStorageManager,
            IAzureConnectionManager azureConnectionManager)
        {
            this.LocalStorageManager = localStorageManager;
            this.AzureConnectionManager = azureConnectionManager;
        }

        public async Task<bool> Run(AddConnectionOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.ConnectionString) && !string.IsNullOrEmpty(opts.ConnectionName))
            {
                await AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Testing {opts.ConnectionName} connection ...", async _ =>
                    {
                        var errorMessage = string.Empty;
                        bool isConnected;

                        try
                        {
                            isConnected = await AzureConnectionManager.TestConnection(opts.ConnectionString);
                        }
                        catch (ConnectionException connectionException)
                        {
                            errorMessage = connectionException.Message;
                            isConnected = false;
                        }

                        if (!isConnected)
                        {
                            AnsiConsole.MarkupLine($"Testing {opts.ConnectionName} connection ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{errorMessage}[/]");

                            return;
                        }

                        AnsiConsole.MarkupLine($"Testing {opts.ConnectionName} connection ... [bold green]Successful[/]");
                        AnsiConsole.Markup($"Storing {opts.ConnectionName} connection ...");

                        LocalStorageManager.AddConnection(opts.ConnectionName, opts.ConnectionString, opts.Select);

                        AnsiConsole.MarkupLine($"Storing {opts.ConnectionName} connection ... [bold green]Successful[/]");
                        AnsiConsole.WriteLine($"Finished adding connection {opts.ConnectionName}");
                    });
            }

            return true;
        }
    }
}