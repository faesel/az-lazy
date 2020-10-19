using CommandLine;

namespace az_lazy.Commands.Connection
{
    [Verb("addconnection", HelpText = "Add new storage connection")]
    public class AddConnectionOptions : ICommandOptions
    {
        [Option('c', "accessKey", Required = true, HelpText = "Connection string to the storage account")]
        public string AccessKey { get; set; }

        [Option('n', "name", Required = true, HelpText = "Name for the storage account being added")]
        public string ConnectionName { get; set; }
    }
}