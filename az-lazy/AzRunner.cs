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

namespace az_lazy
{
    public interface IAzRunner
    {
        Task Startup(string[] args);
    }

    public class AzRunner : IAzRunner
    {
        private readonly IConnectionRunner<ConnectionOptions> ConnectionRunner;
        private readonly IConnectionRunner<AddConnectionOptions> AddConnectionRunner;
        private readonly IConnectionRunner<QueueOptions> QueueRunner;
        private readonly IConnectionRunner<AddQueueOptions> AddQueueRunner;
        private readonly IConnectionRunner<ContainerOptions> ContainerRunner;
        private readonly IConnectionRunner<AddContainerOptions> AddContainerRunner;
        private readonly IConnectionRunner<BlobOptions> BlobRunner;

        public AzRunner(
            IConnectionRunner<ConnectionOptions> connectionRunner,
            IConnectionRunner<AddConnectionOptions> addConnectionRunner,
            IConnectionRunner<QueueOptions> queueRunner,
            IConnectionRunner<AddQueueOptions> addQueueRunner,
            IConnectionRunner<ContainerOptions> containerRunner,
            IConnectionRunner<AddContainerOptions> addContainerRunner,
            IConnectionRunner<BlobOptions> blobRunner)
        {
            this.ConnectionRunner = connectionRunner;
            this.AddConnectionRunner = addConnectionRunner;
            this.QueueRunner = queueRunner;
            this.AddQueueRunner = addQueueRunner;
            this.ContainerRunner = containerRunner;
            this.AddContainerRunner = addContainerRunner;
            this.BlobRunner = blobRunner;
        }

        public async Task Startup(string[] args)
        {
            var parsedResult = Parser.Default
                .ParseArguments<ConnectionOptions, AddConnectionOptions, QueueOptions, AddQueueOptions, ContainerOptions, AddContainerOptions, BlobOptions>(args);

            var result = await parsedResult
                    .MapResult(
                        (AddConnectionOptions opts) => AddConnectionRunner.Run(opts),
                        (ConnectionOptions opts) => ConnectionRunner.Run(opts),
                        (QueueOptions opts) => QueueRunner.Run(opts),
                        (AddQueueOptions opts) => AddQueueRunner.Run(opts),
                        (ContainerOptions opts) => ContainerRunner.Run(opts),
                        (AddContainerOptions opts) => AddContainerRunner.Run(opts),
                        (BlobOptions opts) => BlobRunner.Run(opts),
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