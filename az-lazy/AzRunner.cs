using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Commands;
using az_lazy.Commands.AddConnection;
using az_lazy.Commands.Connection;
using az_lazy.Commands.Queue;
using az_lazy.Commands.AddQueue;
using CommandLine;
using az_lazy.Commands.Container.Executor;
using az_lazy.Commands.AddContainer;
using az_lazy.Commands.Blob;
using az_lazy.Commands.Table;

namespace az_lazy
{
    public interface IAzRunner
    {
        Task Startup(string[] args);
    }

    public class AzRunner : IAzRunner
    {
        private readonly ICommandRunner<ConnectionOptions> ConnectionRunner;
        private readonly ICommandRunner<AddConnectionOptions> AddConnectionRunner;
        private readonly ICommandRunner<QueueOptions> QueueRunner;
        private readonly ICommandRunner<AddQueueOptions> AddQueueRunner;
        private readonly ICommandRunner<ContainerOptions> ContainerRunner;
        private readonly ICommandRunner<AddContainerOptions> AddContainerRunner;
        private readonly ICommandRunner<BlobOptions> BlobRunner;
        private readonly ICommandRunner<TableOptions> TableRunner;

        public AzRunner(
            ICommandRunner<ConnectionOptions> connectionRunner,
            ICommandRunner<AddConnectionOptions> addConnectionRunner,
            ICommandRunner<QueueOptions> queueRunner,
            ICommandRunner<AddQueueOptions> addQueueRunner,
            ICommandRunner<ContainerOptions> containerRunner,
            ICommandRunner<AddContainerOptions> addContainerRunner,
            ICommandRunner<BlobOptions> blobRunner,
            ICommandRunner<TableOptions> tableRunner)
        {
            this.ConnectionRunner = connectionRunner;
            this.AddConnectionRunner = addConnectionRunner;
            this.QueueRunner = queueRunner;
            this.AddQueueRunner = addQueueRunner;
            this.ContainerRunner = containerRunner;
            this.AddContainerRunner = addContainerRunner;
            this.BlobRunner = blobRunner;
            this.TableRunner = tableRunner;
        }

        public async Task Startup(string[] args)
        {
            var parsedResult = Parser.Default
                .ParseArguments<ConnectionOptions, AddConnectionOptions, QueueOptions, AddQueueOptions, ContainerOptions, AddContainerOptions, BlobOptions, TableOptions>(args);

            var result = await parsedResult
                    .MapResult(
                        (AddConnectionOptions opts) => AddConnectionRunner.Run(opts),
                        (ConnectionOptions opts) => ConnectionRunner.Run(opts),
                        (QueueOptions opts) => QueueRunner.Run(opts),
                        (AddQueueOptions opts) => AddQueueRunner.Run(opts),
                        (ContainerOptions opts) => ContainerRunner.Run(opts),
                        (AddContainerOptions opts) => AddContainerRunner.Run(opts),
                        (BlobOptions opts) => BlobRunner.Run(opts),
                        (TableOptions opts) => TableRunner.Run(opts),
                    errs => DisplayHelp(parsedResult, errs))
                .ConfigureAwait(false);

            return;
        }

        public Task<bool> DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            return Task.FromResult(true);
        }
    }
}