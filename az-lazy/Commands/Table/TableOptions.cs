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

        [Option('q', "query", Required = false, HelpText = "The table name to query")]
        public string Table { get; set; }

        [Option('p', "partitionKey", Required = false, HelpText = "Partition key to query the table with")]
        public string PartitionKey { get; set; }

        [Option('r', "rowKey", Required = false, HelpText = "Row key to query the table with")]
        public string RowKey { get; set; }

        [Option('s', "sample", Required = false, HelpText = "Table name to sample")]
        public string Sample { get; set; }

        [Option("sampleCount", Required = false, HelpText = "Used in combination with sample, specify how many rows to sample out of the table. Default sample is 10")]
        public int SampleCount { get; set; }
    }
}