using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

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
                var selectMessage = $"Selecting connection {opts.SelectConnection}";
                ConsoleHelper.WriteInfoWaiting(selectMessage, true);

                var isSuccessfull = LocalStorageManager.SelectConnection(opts.SelectConnection);

                if(isSuccessfull)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(selectMessage);
                    ConsoleHelper.WriteLineNormal($"Connection {opts.SelectConnection} is ready to use!");
                }
                else
                {
                    ConsoleHelper.WriteLineFailedWaiting(selectMessage);
                    ConsoleHelper.WriteLineError("Check the connection name exists and try again");
                }
            }

            return Task.CompletedTask;
        }
    }
}