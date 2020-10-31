using System.Drawing;
using System;
using Pastel;

namespace az_lazy.Helpers
{
    public static class ConsoleHelper
    {
        private static readonly Color InformationColour = Color.DarkGray;
        private static readonly Color FailedColour = Color.IndianRed;
        private static readonly Color SuccessColour = Color.LightGreen;

        public static void WriteLineNormal(string information)
        {
            Console.WriteLine(information);
        }

        public static void WriteInfo(string information)
        {
            Console.Write(information.Pastel(InformationColour));
        }

        public static void WriteLineInfo(string information)
        {
            Console.WriteLine(information.Pastel(InformationColour));
        }

        public static void WriteInfoWaiting(string information, bool resetPosition = false)
        {
            Console.Write($"{information.Pastel(InformationColour)} ...");

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
            Console.Write($"{information.Pastel(InformationColour)} ... %{pct}");

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
            Console.WriteLine($"{information} - {additionalInformation.Pastel(InformationColour)}");
        }

        public static void WriteLineError(string information)
        {
            Console.WriteLine($"{information.Pastel(FailedColour)}");
        }

        public static void WriteSuccessWaiting(string information)
        {
            Console.Write($"{information.Pastel(InformationColour)} ... {"Successfull".Pastel(SuccessColour)}");
        }

        public static void WriteLineSuccessWaiting(string information)
        {
            Console.WriteLine($"{information.Pastel(InformationColour)} ... {"Successfull".Pastel(SuccessColour)}");
        }

        public static void WriteFailedWaiting(string information)
        {
            Console.Write($"{information.Pastel(InformationColour)} ... {"Failed".Pastel(FailedColour)}");
        }

        public static void WriteLineFailedWaiting(string information)
        {
            Console.WriteLine($"{information.Pastel(InformationColour)} ... {"Failed".Pastel(FailedColour)}");
        }

        public static void WriteSepparator()
        {
            Console.WriteLine("------------------------------------------------------------");
        }
    }
}