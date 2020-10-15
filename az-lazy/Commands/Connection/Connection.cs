using CommandLine;

namespace az_lazy.Commands
{
    [Verb("connection", HelpText = "Manage azure storage connections")]
    public class ConnectionOptions : ICommandOptions
    {
        [Option('l', "list", Required = false, HelpText = "List all connections available")]
        public bool List { get; set; }

        
        [Option('r', "remove", Required = false, HelpText = "Remove an existing connection by name")]
        public string RemoveConnection { get; set; }

        [Option('s', "select", Required = false, HelpText = "Select an existing connection by name")]
        public string SelectConnection { get; set; }


        [Option('a', "add", Required = false, HelpText = "Connection string to the storage account")]
        public string ConnectionString { get; set; }
        [Option('n', "name", Required = false, HelpText = "Name for the storage account being added")]
        public string ConnectionName { get; set; }


    }
}