using System;
using System.Threading.Tasks;
using az_lazy.Manager;
using Spectre.Console;

namespace az_lazy.Commands.Connection.Executor
{
    public class SelectConnectionExecutor : ICommandExecutor<ConnectionOptions>
    {
        public readonly ILocalStorageManager LocalStorageManager;

        public SelectConnectionExecutor(
            ILocalStorageManager localStorageManager)
        {
            this.LocalStorageManager = localStorageManager;
        }

        public Task Execute(ConnectionOptions opts)
        {
            if (!string.IsNullOrEmpty(opts.SelectConnection))
            {
                AnsiConsole
                    .Status()
                    .Spinner(Spinner.Known.Star)
                    .SpinnerStyle(Style.Parse("green bold"))
                    .StartAsync($"Selecting connection {opts.SelectConnection} ...", _ =>
                    {
                        try
                        {
                            var isSuccessfull = LocalStorageManager.SelectConnection(opts.SelectConnection);

                            if (isSuccessfull)
                            {
                                AnsiConsole.MarkupLine($"Selecting connection {opts.SelectConnection} ... [bold green]Successful[/]");
                                AnsiConsole.MarkupLine($"Connection {opts.SelectConnection} is ready to use!");
                            }
                            else
                            {
                                AnsiConsole.MarkupLine($"Selecting connection {opts.SelectConnection} ... [bold red]Failed[/]");
                                AnsiConsole.MarkupLine("Check the connection name exists and try again");
                            }
                        }
                        catch (Exception ex)
                        {
                            AnsiConsole.MarkupLine($"Error whilst selecting connection {opts.SelectConnection} ... [bold red]Failed[/]");
                            AnsiConsole.MarkupLine($"[bold red]{ex.Message}[/]");
                        }

                        return Task.CompletedTask;
                    });
            }

            return Task.CompletedTask;
        }
    }
}