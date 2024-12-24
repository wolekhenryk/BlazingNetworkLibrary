using PacketForge.Enums;

namespace PacketForge;

public interface ILogDestination
{
    /// <summary>
    /// Writes a message to the log destination.
    /// </summary>
    /// <param name="message">The message to write.</param>
    void Write(string message, LogLevel logLevel);
}
