using CommandLine;

namespace az_lazy.Commands.Queue
{
    [Verb("queue", HelpText = "Manage azure storage queues")]
    public class QueueOptions : ICommandOptions
    {
        [Option('l', "list", Required = false, HelpText = "List all connections available")]
        public bool List { get; set; }

        [Option("contains", Required = false, HelpText = "Use in combination with list, allows you to filter the list returned")]
        public string Contains { get; set; }

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

        [Option('w', "watch", Required = false, HelpText = "Queue name to begin watching for new messages")]
        public string Watch { get; set; }

        [Option('p', "peek", Required = false, HelpText = "Queue name to begin peeking messages from")]
        public string Peek { get; set; }

        [Option("peekCount", Required = false, HelpText = "The number of messages to peek from the queue")]
        public int PeekCount { get; set; }

        [Option('f', "from", Required = false, HelpText = "Queue you want to move messages from")]
        public string From { get; set; }

        [Option('t', "to", Required = false, HelpText = "Queue you want to move messages to")]
        public string To { get; set; }
    }
}