using System;
using System.Collections.Generic;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace az_lazy
{
    class Program
    {
        [Verb("connection", HelpText = "Manage azure storage connections")]
        public class ConnectionOptions 
        {
            [Option('l', "list", Required = false, HelpText = "List all connections available")]
            public bool List { get; set; }

            [Option('r', "remove", Required = false, HelpText = "Remove an existing connection by name")]
            public string RemoveConnection { get; set; }

            [Option('s', "select", Required = false, HelpText = "Select an existing connection by name")]
            public string SelectConnection { get; set; }

            [Option('a', "add", Required = false, HelpText = "Connectionstring to the storage account")]
            public string ConnectionString { get; set; }
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var versionString = Assembly.GetEntryAssembly()
                                        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                                        .InformationalVersion;

                Console.WriteLine($"az-lazy v{versionString}");
                Console.WriteLine($"Checkout https://www.help.com/az-lazy for examples");
                Console.WriteLine("-------------");
            }

            var parsedResult = Parser.Default.ParseArguments<ConnectionOptions>(args);

            var result = parsedResult
                .MapResult(
                    (ConnectionOptions opts) => RunConnectionOptions(opts),
                errs => DisplayHelp(parsedResult, errs));

            return;
        }

        public static bool DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            // var helpText = HelpText.AutoBuild(result, h =>
            // {
            //     h.AdditionalNewLineAfterOption = false;
            //     h.Heading = "Myapp 2.0.0-beta"; //change header
            //     h.Copyright = "Copyright (c) 2019 Global.com"; //change copyright text
            //     return HelpText.DefaultParsingErrorsHandler(result, h);
            // }, e => e);
            // Console.WriteLine(helpText);

            return true;
        }

        private static bool RunConnectionOptions(ConnectionOptions opts)
        {
            if(opts.List)
            {
                Console.WriteLine("ConnectionOne [*]");
                Console.WriteLine("ConnectionTwo");
            }

            if(!string.IsNullOrEmpty(opts.ConnectionString))
            {
                Console.WriteLine("Connection added");
            }

            if(!string.IsNullOrEmpty(opts.RemoveConnection))
            {
                Console.WriteLine("Connection removed");
            }

            if(!string.IsNullOrEmpty(opts.SelectConnection))
            {
                Console.WriteLine("Connection selected");
            }

            return true;
        }
    }
}
