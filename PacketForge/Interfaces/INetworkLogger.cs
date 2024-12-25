using PacketForge.Enums;

namespace PacketForge.Interfaces;

public interface INetworkLogger
{
    /// <summary>
    /// Logs a message to the network logger.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Log(string message, LogLevel logLevel = LogLevel.Info);
}
