using System.Collections.Generic;
using System.Threading.Tasks;

namespace az_lazy.Commands.Connection
{
    public class ConnectionRunner : IConnectionRunner<ConnectionOptions>
    {
        public readonly IEnumerable<ICommandExecutor<ConnectionOptions>> CommandExecutors;

        public ConnectionRunner(
            IEnumerable<ICommandExecutor<ConnectionOptions>> commandExecutors)
        {
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