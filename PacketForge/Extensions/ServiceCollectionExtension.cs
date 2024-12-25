using PacketForge.Core;
using PacketForge.Interfaces;
using PacketForge.Logging;
using PacketForge.Logging.LogDestinations;


namespace PacketForge;
/// <summary>
/// Builder for configuring PacketForge services.
/// </summary>
public class PacketForgeBuilder(IServiceCollection services)
{
    private readonly IServiceCollection _services = services;
    private IPAddress _ip = IPAddress.Loopback; // Default IP
    private int _port = 5000; // Default port

    /// <summary>
    /// Sets the IP address for the server.
    /// </summary>
    /// <param name="ip">The IP address to bind to.</param>
    /// <returns>The builder instance.</returns>
    public PacketForgeBuilder UseIPAddress(IPAddress ip)
    {
        _ip = ip;
        return this;
    }

    /// <summary>
    /// Sets the port for the server.
    /// </summary>
    /// <param name="port">The port to bind to.</param>
    /// <returns>The builder instance.</returns>
    public PacketForgeBuilder UsePort(int port)
    {
        _port = port;
        return this;
    }

    /// <summary>
    /// Configures and registers PacketForge services into the service collection.
    /// </summary>
    public void Build()
    {
        _services.AddSingleton(_ => new TcpListener(_ip, _port));
        _services.AddSingleton<ILogDestination, ConsoleLogDestination>();
        _services.AddSingleton<INetworkLogger, NetworkLogger>();
        _services.AddSingleton<Server>();

        _services.AddHostedService<PacketForgeHostedService>();
    }
}

/// <summary>
/// Extension methods for the <see cref="IServiceCollection"/> class.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds PacketForge services to the service collection using a builder. Default IP is IPAddress.Any and default port is 5000.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="configure">The configuration action to customize the builder.</param>
    /// <returns>The IServiceCollection instance.</returns>
    public static IServiceCollection AddPacketForge(this IServiceCollection services, Action<PacketForgeBuilder> configure)
    {
        var builder = new PacketForgeBuilder(services);
        configure(builder);
        builder.Build();

        return services;
    }
}
