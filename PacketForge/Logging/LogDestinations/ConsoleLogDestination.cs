using PacketForge.Enums;
using PacketForge.Interfaces;

namespace PacketForge.Logging.LogDestinations;

/// <summary>
/// Provides a log destination that writes log messages to the console.
/// </summary>
public class ConsoleLogDestination : ILogDestination
{
    public void Write(string message, LogLevel logLevel)
    {
        // Save the current console foreground color
        var originalColor = Console.ForegroundColor;

        // Set the console color based on the log level
        Console.ForegroundColor = logLevel switch
        {
            LogLevel.Debug => ConsoleColor.Gray,// Light gray for debug logs
            LogLevel.Info => ConsoleColor.White,// White for informational logs
            LogLevel.Warning => ConsoleColor.Yellow,// Yellow for warnings
            LogLevel.Error => ConsoleColor.Red,// Red for errors
            _ => ConsoleColor.White,// Default color for unrecognized levels
        };

        // Write the log message
        string formattedMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{logLevel.ToString().ToUpper()}] {message}";
        Console.WriteLine(formattedMessage);

        // Restore the original console color
        Console.ForegroundColor = originalColor;
    }
}
