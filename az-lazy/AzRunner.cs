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
        private readonly IConnectionRunner<ContainerOptions> ContainerOptions;
        private readonly IConnectionRunner<AddContainerOptions> AddContainerOptions;

        public AzRunner(
            IConnectionRunner<ConnectionOptions> connectionRunner,
            IConnectionRunner<AddConnectionOptions> addConnectionRunner,
            IConnectionRunner<QueueOptions> queueRunner,
            IConnectionRunner<AddQueueOptions> addQueueRunner,
            IConnectionRunner<ContainerOptions> containerOptions,
            IConnectionRunner<AddContainerOptions> addContainerOptions)
        {
            this.ConnectionRunner = connectionRunner;
            this.AddConnectionRunner = addConnectionRunner;
            this.QueueRunner = queueRunner;
            this.AddQueueRunner = addQueueRunner;
            this.ContainerOptions = containerOptions;
            this.AddContainerOptions = addContainerOptions;
        }

        public async Task Startup(string[] args)
        {
            var parsedResult = Parser.Default.ParseArguments<ConnectionOptions, AddConnectionOptions, QueueOptions, AddQueueOptions, ContainerOptions, AddContainerOptions>(args);

            var result = await parsedResult
                    .MapResult(
                        (AddConnectionOptions opts) => AddConnectionRunner.Run(opts),
                        (ConnectionOptions opts) => ConnectionRunner.Run(opts),
                        (QueueOptions opts) => QueueRunner.Run(opts),
                        (AddQueueOptions opts) => AddQueueRunner.Run(opts),
                        (ContainerOptions opts) => ContainerOptions.Run(opts),
                        (AddContainerOptions opts) => AddContainerOptions.Run(opts),
                    errs => DisplayHelp(parsedResult, errs))
                .ConfigureAwait(false);

            return;
        }

        public Task<bool> DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            // var helpText = HelpText.AutoBuild(result, h =>
            // {
            //     h.AdditionalNewLineAfterOption = false;
            //     h.Heading = "Myapp 2.0.0-beta"; //change header
            //     h.Copyright = "Copyright (c) 2019 Global.com"; //change copyright text
            //     return HelpText.DefaultParsingErrorsHandler(result, h);
            // }, e => e);
            // Console.WriteLine(helpText);

            return Task.FromResult(true);
        }
    }
}