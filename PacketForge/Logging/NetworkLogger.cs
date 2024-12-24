using PacketForge.Enums;

namespace PacketForge.Logging;

public class NetworkLogger(ILogDestination logDestination) : INetworkLogger
{
    private readonly ILogDestination _logDestination = logDestination;

    public void Log(string message, LogLevel logLevel = LogLevel.Info)
    {
        _logDestination.Write(message, logLevel);
    }
}
