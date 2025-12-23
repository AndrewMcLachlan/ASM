using static System.Console;

namespace System;

/// <summary>
/// Allows writing to the console in a specified colour.
/// </summary>
public static class ConsoleExtensions
{
    extension(Console)
    {
        /// <summary>
        /// Writes a line to the console in the specified colour.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="colour">The colour to use.</param>
        public static void WriteLine(string value, ConsoleColor colour)
        {
            Write(value, colour, Console.WriteLine);
        }

        /// <summary>
        /// Writes to the console in the specified colour.
        /// </summary>
        /// <param name="value">The text to write.</param>
        /// <param name="colour">The colour to use.</param>
        public static void Write(string value, ConsoleColor colour)
        {
            Write(value, colour, Console.Write);
        }
    }

    private static void Write(string value, ConsoleColor colour, Action<string> writer)
    {
        var currentColour = ForegroundColor;
        ForegroundColor = colour;
        writer(value);
        ForegroundColor = currentColour;
    }
}
