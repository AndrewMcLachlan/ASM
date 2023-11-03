using static System.Console;

namespace Asm;

public static class ColouredConsole
{
    public static void WriteLine(string value, ConsoleColor colour)
    {
        Write(value, colour, Console.WriteLine);
    }

    public static void Write(string value, ConsoleColor colour)
    {
        Write(value, colour, Console.Write);
    }

    private static void Write(string value, ConsoleColor colour, Action<string> writer)
    {
        var currentColour = ForegroundColor;
        ForegroundColor = colour;
        writer(value);
        ForegroundColor = currentColour;
    }
}
