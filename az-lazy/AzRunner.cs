using System.Collections.Generic;
using System.Threading.Tasks;
using az_lazy.Commands;
using az_lazy.Commands.Queue;
using CommandLine;

namespace az_lazy
{
    public interface IAzRunner
    {
        Task Startup(string[] args);
    }

    public class AzRunner : IAzRunner
    {
        private readonly IConnectionRunner<ConnectionOptions> ConnectionRunner;
        private readonly IConnectionRunner<QueueOptions> QueueRunner;

        public AzRunner(
            IConnectionRunner<ConnectionOptions> connectionRunner,
            IConnectionRunner<QueueOptions> queueRunner)
        {
            this.ConnectionRunner = connectionRunner;
            this.QueueRunner = queueRunner;
        }

        public async Task Startup(string[] args)
        {
            var parsedResult = Parser.Default.ParseArguments<ConnectionOptions, QueueOptions>(args);

            var result = await parsedResult
                    .MapResult(
                        (ConnectionOptions opts) => ConnectionRunner.Run(opts),
                        (QueueOptions opts) => QueueRunner.Run(opts),
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