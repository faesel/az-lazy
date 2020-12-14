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

        [Option('r', "remove", Required = false, HelpText = "Container name to remove")]
        public string RemoveContainer { get; set; }

        [Option('t', "tree", Required = false, HelpText = "Shows the contents of the container in a tree view")]
        public string Tree { get; set; }

        [Option('d', "depth", Required = false, HelpText = "Specifies the depth of view the container in a tree view")]
        public int? Depth { get; set; }

        [Option('p', "prefix", Required = false, HelpText = "Used in combination with the tree command, searches within the prefixed path")]
        public string Prefix { get; set; }

        [Option("detailed", Required = false, HelpText = "Uncovers more detail for each blob")]
        public bool Detailed { get; set; }
    }
}