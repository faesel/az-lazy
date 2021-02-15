using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Connection.Executor
{
    public class WipeExecutor: ICommandExecutor<ConnectionOptions>
    {
        public readonly ILocalStorageManager LocalStorageManager;

        public WipeExecutor(
            ILocalStorageManager localStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
        }

        public Task Execute(ConnectionOptions opts)
        {
            if (opts.Wipe)
            {
                AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync("Removing all connections ...", _ =>
                    {
                        try
                        {
                            var isSuccessfull = LocalStorageManager.RemoveAllConnections(opts.SelectConnection);

                            if (isSuccessfull)
                            {
                                AnsiConsole.MarkupLine("Removing all connections ... [bold green]Successful[/]");
                                AnsiConsole.MarkupLine("Finished removing connections");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine("Removing all connections ... [bold red]Failed[/]");
                                AnsiConsole.MarkupLine("Failed to remove all connections");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Error whilst wiping connections ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }

                        return Task.CompletedTask;
                    });
            }

            return Task.CompletedTask;
        }
    }
}