using PacketForge.Enums;

namespace PacketForge.Extensions;

/// <summary>
/// Contains extension methods for the <see cref="INetworkLogger"/> interface.
/// </summary>
public static class LoggerExtensions
{
    public static void LogDebug(this INetworkLogger logger, string message) => logger.Log(message, LogLevel.Debug);
    public static void LogInfo(this INetworkLogger logger, string message) => logger.Log(message, LogLevel.Info);
    public static void LogWarning(this INetworkLogger logger, string message) => logger.Log(message, LogLevel.Warning);
    public static void LogError(this INetworkLogger logger, string message) => logger.Log(message, LogLevel.Error);
}
