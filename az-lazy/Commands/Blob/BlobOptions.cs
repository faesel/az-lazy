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

        [Option('r', "uploadFile", Required = false, HelpText = "File path to upload")]
        public string UploadFile { get; set; }

        [Option('r', "uploadPath", Required = false, HelpText = "Path and file name to upload to")]
        public string UploadPath { get; set; }

        [Option('s', "uploadDirectory", Required = false, HelpText = "Folder to upload and synchronies into a given container, you can use uploadPath to optionally specify a subroute in the container")]
        public string Directory { get; set; }
    }
}
