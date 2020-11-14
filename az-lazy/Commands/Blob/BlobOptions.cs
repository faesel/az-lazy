using CommandLine;

namespace az_lazy.Commands.Blob
{
    [Verb("blob", HelpText = "Manage azure storage blobs")]
    public class BlobOptions : ICommandOptions
    {
        [Option('c', "container", Required = true, HelpText = "Name of the container to operate in")]
        public string Container { get; set; }

        [Option('r', "remove", Required = false, HelpText = "Name of the blob to remove")]
        public string Remove { get; set; }
    }
}
