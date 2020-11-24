using CommandLine;

namespace az_lazy.Commands.Table
{
    [Verb("table", HelpText = "Manage azure tables")]
    public class TableOptions : ICommandOptions
    {
        [Option('l', "list", Required = false, HelpText = "List all tables available")]
        public bool List { get; set; }

        [Option("contains", Required = false, HelpText = "Use in combination with list, allows you to filter the list returned")]
        public string Contains { get; set; }
    }
}