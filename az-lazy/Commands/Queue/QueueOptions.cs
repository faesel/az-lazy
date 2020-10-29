using CommandLine;

namespace az_lazy.Commands.Queue
{
    [Verb("queue", HelpText = "Manage azure storage connections")]
    public class QueueOptions : ICommandOptions
    {
        [Option('l', "list", Required = false, HelpText = "List all connections available")]
        public bool List { get; set; }

        [Option('r', "remove", Required = false, HelpText = "Queue name to remove")]
        public string RemoveQueue { get; set; }

        [Option('c', "cure", Required = false, HelpText = "Move poison queue messages back into main queue")]
        public string CureQueue { get; set; }

        [Option("clear", Required = false, HelpText = "Remove all messages from the queue")]
        public string ClearQueue { get; set; }

        [Option("addQueue", Required = false, HelpText = "Queue to add new message")]
        public string AddQueue { get; set; }

        [Option("addMessage", Required = false, HelpText = "Message to send")]
        public string AddMessage { get; set; }

        [Option("watch", Required = false, HelpText = "Queue name to begin watching for new messages")]
        public string Watch { get; set; }
    }
}