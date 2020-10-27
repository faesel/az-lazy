using CommandLine;

namespace az_lazy.Commands.Connection
{
    [Verb("addconnection", HelpText = "Add new storage connection")]
    public class AddConnectionOptions : ICommandOptions
    {
        [Option('c', "connectionString", Required = true, HelpText = "Connection string to the storage account")]
        public string ConnectionString { get; set; }

        [Option('n', "name", Required = true, HelpText = "Name for the storage account being added")]
        public string ConnectionName { get; set; }

        [Option('s', "select", Required = false, HelpText = "Select connection when adding it to the list")]
        public bool Select { get; set; }
    }
}