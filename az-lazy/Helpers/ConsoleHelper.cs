using System.Drawing;
using System;
using Pastel;
using Spectre.Console;

namespace az_lazy.Helpers
{
    public static class ConsoleHelper
    {
        public static void WriteLineNormal(string information)
        {
            Console.WriteLine(information);
        }

        public static void WriteLineNormal(string information, string extraInformation)
        {
            Console.WriteLine($"{information} - {extraInformation.Pastel(Colours.InformationColour)}");
        }

        public static void WriteInfo(string information)
        {
            Console.Write(information.Pastel(Colours.InformationColour));
        }

        public static void WriteLineInfo(string information)
        {
            Console.WriteLine(information.Pastel(Colours.InformationColour));
        }

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

        public static void WriteInfoWaitingPct(string information, int pct, bool resetPosition = false)
        {
            Console.Write($"{information.Pastel(Colours.InformationColour)} ... %{pct}");

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

        public static void WriteLineAdditionalInfo(string information, string additionalInformation)
        {
            Console.WriteLine($"{information} - {additionalInformation.Pastel(Colours.InformationColour)}");
        }

        public static void WriteLineError(string information)
        {
            Console.WriteLine($"{information.Pastel(Colours.FailedColour)}");
        }

        public static void WriteSuccessWaiting(string information)
        {
            Console.Write($"{information.Pastel(Colours.InformationColour)} ... {"Successfull".Pastel(Colours.SuccessColour)}");
        }

        public static void WriteLineSuccessWaiting(string information)
        {
            Console.WriteLine($"{information.Pastel(Colours.InformationColour)} ... {"Successfull".Pastel(Colours.SuccessColour)}");
        }

        public static void WriteFailedWaiting(string information)
        {
            Console.Write($"{information.Pastel(Colours.InformationColour)} ... {"Failed".Pastel(Colours.FailedColour)}");
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