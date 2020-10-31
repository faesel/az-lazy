using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

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
                var removeMessage = $"Removing connection {opts.RemoveConnection}";
                ConsoleHelper.WriteInfoWaiting(removeMessage, true);

                var isSuccessfull = LocalStorageManager.RemoveConnection(opts.RemoveConnection);

                if(isSuccessfull)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(removeMessage);
                    ConsoleHelper.WriteLineNormal($"Finished removing connection {opts.RemoveConnection}");
                }
                else
                {
                    ConsoleHelper.WriteLineFailedWaiting(removeMessage);
                    ConsoleHelper.WriteLineError("Check the connection name exists and try again");
                }
            }

            return Task.CompletedTask;
        }
    }
}