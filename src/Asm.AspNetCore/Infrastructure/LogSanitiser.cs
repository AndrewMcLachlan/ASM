using System.Text;

namespace Asm.AspNetCore;

internal static class LogSanitiser
{
    /// <summary>
    /// Strips control characters, other than tab, from a value that came from the request, so it
    /// cannot forge or split log entries.
    /// </summary>
    public static string Sanitise(string? input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return String.Empty;
        }

        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            // Keep printable and tab (tab is common in JSON whitespace); drop other control chars.
            if (c == '\t' || !Char.IsControl(c))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
