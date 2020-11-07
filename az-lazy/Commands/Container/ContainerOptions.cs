using CommandLine;

namespace az_lazy.Commands.Container.Executor
{
    [Verb("container", HelpText = "Manage azure storage containers")]
    public class ContainerOptions : ICommandOptions
    {
        [Option('l', "list", Required = false, HelpText = "List all containers available")]
        public bool List { get; set; }

        [Option("contains", Required = false, HelpText = "Use in combination with list, allows you to filter the list returned")]
        public string Contains { get; set; }
    }
}