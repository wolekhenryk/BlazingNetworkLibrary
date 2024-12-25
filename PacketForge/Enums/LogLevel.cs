namespace PacketForge.Enums;

public enum LogLevel : byte
{
    /// <summary>
    /// Logs all messages.
    /// </summary>
    Debug = 0,

    /// <summary>
    /// Logs only informational messages and above.
    /// </summary>
    Info = 1,

    /// <summary>
    /// Logs only warning messages and above.
    /// </summary>
    Warning = 2,

    /// <summary>
    /// Logs only error messages.
    /// </summary>
    Error = 3
}
