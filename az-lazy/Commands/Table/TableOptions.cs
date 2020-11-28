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

        [Option('s', "sample", Required = false, HelpText = "The name of the table to sample data from")]
        public string Sample { get; set; }

        [Option("sampleCount", Required = false, HelpText = "Used in combination with sample, specify how many rows to sample out of the table. Default sample is 10")]
        public int SampleCount { get; set; }
    }
}