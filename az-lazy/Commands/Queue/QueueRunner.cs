using System.Collections.Generic;
using System.Threading.Tasks;

namespace az_lazy.Commands.Queue
{
    public class QueueRunner : ICommandRunner<QueueOptions>
    {
        public readonly IEnumerable<ICommandExecutor<QueueOptions>> CommandExecutors;

        public QueueRunner(
            IEnumerable<ICommandExecutor<QueueOptions>> commandExecutors)
        {
            this.CommandExecutors = commandExecutors;
        }

        public async Task<bool> Run(QueueOptions opts)
        {
            foreach(var executor in CommandExecutors)
            {
                await executor.Execute(opts).ConfigureAwait(false);
            }

            return true;
        }
    }
}