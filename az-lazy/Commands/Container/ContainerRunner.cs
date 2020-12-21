using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Commands.Container.Executor;

namespace az_lazy.Commands.Blob
{
    public class ContainerRunner : ICommandRunner<ContainerOptions>
    {
        public readonly IEnumerable<ICommandExecutor<ContainerOptions>> CommandExecutors;

        public ContainerRunner(
            IEnumerable<ICommandExecutor<ContainerOptions>> commandExecutors)
        {
            this.CommandExecutors = commandExecutors;
        }

        public async Task<bool> Run(ContainerOptions opts)
        {
            foreach(var executor in CommandExecutors)
            {
                await executor.Execute(opts);
            }

            return true;
        }
    }
}