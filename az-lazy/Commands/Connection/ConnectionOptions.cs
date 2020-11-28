using CommandLine;

namespace az_lazy.Commands.Connection
{
    [Verb("connection", HelpText = "Manage azure storage connections")]
    public class ConnectionOptions : ICommandOptions
    {
        [Option('l', "list", Required = false, HelpText = "List all connections available, [*] indicated selected connection")]
        public bool List { get; set; }

        [Option('r', "remove", Required = false, HelpText = "Name of a existing connection to remove")]
        public string RemoveConnection { get; set; }

        [Option('s', "select", Required = false, HelpText = "Name of an existing connection to select")]
        public string SelectConnection { get; set; }

        [Option('w', "wipe", Required = false, HelpText = "Wipes out al connections from the list")]
        public bool Wipe { get; set; }
    }
}