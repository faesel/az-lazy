using CommandLine;

namespace az_lazy.Commands.AddContainer
{
    [Verb("addcontainer", HelpText = "Creates a new storage container")]
    public class AddContainerOptions : ICommandOptions
    {
        [Option('n', "name", Required = true, HelpText = "Name of the container to create")]
        public string Name { get; set; }

        [Option('p', "publicAccess", Required = false, HelpText = "Options are None, Blob, BlobContainer")]
        public string PublicAccess { get; set; }
    }
}