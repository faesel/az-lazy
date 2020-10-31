using CommandLine;

namespace az_lazy.Commands.AddQueue
{
    [Verb("addqueue", HelpText = "Create new azure storage queue")]
    public class AddQueueOptions : ICommandOptions
    {
        [Option('n', "name", Required = true, HelpText = "Name of the queue to create")]
        public string Name { get; set; }
    }
}