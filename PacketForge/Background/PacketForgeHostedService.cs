using System.ComponentModel;
using Microsoft.Extensions.Hosting;
using PacketForge.Core;

namespace PacketForge;

public class PacketForgeHostedService(Server server) : BackgroundService
{
    private readonly Server _server = server;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _server.StartAsync(cancellationToken);
    }
}
