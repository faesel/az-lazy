using System.Drawing;
using System;
using Pastel;
using Spectre.Console;

namespace az_lazy.Helpers
{
    public static class ConsoleHelper
    {
        public static void WriteInfoWaiting(string information, bool resetPosition = false)
        {
            Console.Write($"{information.Pastel(Colours.InformationColour)} ...");

            if(resetPosition)
            {
                try
                {
                    Console.SetCursorPosition(0, Console.CursorTop);
                }
                catch(Exception ex)
                {
                    //This is most likely happening because there is no console
                    Console.WriteLine($"No console to write to {ex.Message}");
                }
            }
        }

        public static void WriteLineError(string information)
        {
            Console.WriteLine($"{information.Pastel(Colours.FailedColour)}");
        }

        public static void WriteLineSuccessWaiting(string information)
        {
            Console.WriteLine($"{information.Pastel(Colours.InformationColour)} ... {"Successfull".Pastel(Colours.SuccessColour)}");
        }

        public static void WriteLineFailedWaiting(string information)
        {
            Console.WriteLine($"{information.Pastel(Colours.InformationColour)} ... {"Failed".Pastel(Colours.FailedColour)}");
        }

        public static void WriteSepparator()
        {
            var rule = new Rule();
            AnsiConsole.Render(rule);
        }
    }
}