using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Manager;

namespace az_lazy.Commands.Connection
{
    public class ConnectionRunner : IConnectionRunner<ConnectionOptions>
    {
        public readonly ILocalStorageManager LocalStorageManager;
        public IEnumerable<ICommandExecutor<ConnectionOptions>> CommandExecutors;

        public ConnectionRunner(
            ILocalStorageManager localStorageManager,
            IEnumerable<ICommandExecutor<ConnectionOptions>> commandExecutors)
        {
            this.LocalStorageManager = localStorageManager;
            this.CommandExecutors = commandExecutors;
        }

        public Task<bool> Run(ConnectionOptions opts)
        {
            foreach(var executor in CommandExecutors)
            {
                executor.Execute(opts);
            }

            return Task.FromResult(true);
        }
    }
}