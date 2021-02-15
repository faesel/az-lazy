using System;
using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Connection.Executor
{
    public class RemoveConnectionExecutor : ICommandExecutor<ConnectionOptions>
    {
        public readonly ILocalStorageManager LocalStorageManager;

        public RemoveConnectionExecutor(
            ILocalStorageManager localStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
        }

        public Task Execute(ConnectionOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.RemoveConnection))
            {
                AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Removing connection {opts.RemoveConnection} ...", _ =>
                    {
                        try
                        {
                            var isSuccessfull = LocalStorageManager.RemoveConnection(opts.RemoveConnection);

                            if (isSuccessfull)
                            {
                                AnsiConsole.MarkupLine($"Removing connection {opts.RemoveConnection} ... [bold green]Successful[/]");
                                AnsiConsole.MarkupLine($"Finished removing connection {opts.RemoveConnection}");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"Removing connection {opts.RemoveConnection} ... [bold red]Failed[/]");
                                AnsiConsole.MarkupLine("Check the connection name exists and try again");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Error whilst removing connection {opts.RemoveConnection} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }

                        return Task.CompletedTask;
                    });
            }

            return Task.CompletedTask;
        }
    }
}