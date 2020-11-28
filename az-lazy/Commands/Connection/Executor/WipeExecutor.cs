using System.Threading.Tasks;
using az_lazy.Helpers;
using az_lazy.Manager;

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
                const string removeMessage = "Removing all connections";
                ConsoleHelper.WriteInfoWaiting(removeMessage, true);

                var isSuccessfull = LocalStorageManager.RemoveAllConnections(opts.RemoveConnection);

                if(isSuccessfull)
                {
                    ConsoleHelper.WriteLineSuccessWaiting(removeMessage);
                    ConsoleHelper.WriteLineNormal("Finished removing connections");
                }
                else
                {
                    ConsoleHelper.WriteLineFailedWaiting(removeMessage);
                    ConsoleHelper.WriteLineError("Failed to remove all connections");
                }
            }

            return Task.CompletedTask;
        }
    }
}