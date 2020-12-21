using System.Collections.Generic;
using System.Threading.Tasks;

namespace az_lazy.Commands.Blob
{
    public class BlobRunner : ICommandRunner<BlobOptions>
    {
        public readonly IEnumerable<ICommandExecutor<BlobOptions>> CommandExecutors;

        public BlobRunner(
            IEnumerable<ICommandExecutor<BlobOptions>> commandExecutors)
        {
            this.CommandExecutors = commandExecutors;
        }

        public async Task<bool> Run(BlobOptions opts)
        {
            foreach (var executor in CommandExecutors)
            {
                await executor.Execute(opts);
            }

            return true;
        }
    }
}
