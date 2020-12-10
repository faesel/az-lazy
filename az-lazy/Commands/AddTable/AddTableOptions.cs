using CommandLine;

namespace az_lazy.Commands.AddTable
{
    [Verb("addtable", HelpText = "Add azure tables")]
    public class AddTableOptions : ICommandOptions
    {
       [Option('n', "name", Required = true, HelpText = "Name of the table")]
        public string Name { get; set; }
    }
}