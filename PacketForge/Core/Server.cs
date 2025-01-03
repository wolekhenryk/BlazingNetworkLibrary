﻿using System.Text;
using Newtonsoft.Json;
using PacketForge.Extensions;
using PacketForge.Interfaces;

namespace PacketForge.Core;

public class Server(TcpListener tcpListener, INetworkLogger logger)
{
    /// <summary>
    /// The TCP listener that the server is using.
    /// </summary>
    private readonly TcpListener _tcpListener = tcpListener;

    /// <summary>
    /// The network logger that the server is using.
    /// </summary>
    private readonly INetworkLogger _logger = logger;

    /// <summary>
    /// The clients that are connected to the server. The key is the client's GUID and the value is the client's TCP client.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, TcpClient> _clients = [];

    /// <summary>
    /// Tracks the last activity (heartbeat or data received) for each client.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, DateTime> _lastHeartbeats = [];

    /// <summary>
    /// The streams that are connected to the server. The key is the client's GUID and the value is the client's network stream.
    /// </summary>
    private readonly ConcurrentDictionary<Guid, NetworkStream> _streams = [];

    /// <summary>
    /// Event triggered whenever a client sends data.
    /// </summary>
    public event Action<Guid, string>? ClientDataReceived;

    /// <summary>
    /// The number of clients that are connected to the server.
    /// </summary>
    public int ClientCount => _clients.Count;

    /// <summary>
    /// Starts the server and listens for incoming connections.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _tcpListener.Start();

        while (!cancellationToken.IsCancellationRequested)
        {
            var tcpClient = await _tcpListener.AcceptTcpClientAsync(cancellationToken);
            _ = Task.Run(() => HandleClientAsync(tcpClient, cancellationToken), cancellationToken);
        }
    }

    /// <summary>
    /// Handles a client that is connected to the server.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        var stream = client.GetStream();

        // The first data that the client sends is the guid.
        var guidBytes = new byte[16];
        var bytesRead = await stream.ReadAsync(guidBytes, 0, guidBytes.Length, cancellationToken);

        if (bytesRead != guidBytes.Length)
        {
            client.Close();
            throw new Exception($"Invalid GUID length. The client sent {bytesRead} bytes, but the expected length is {guidBytes.Length} bytes.");
        }

        // Convert the bytes to a guid.
        var clientGuid = new Guid(guidBytes);

        // The client is now connected.
        if (_clients.TryAdd(clientGuid, client)) {
            _streams.TryAdd(clientGuid, stream);
        } else {
            client.Close();
            throw new Exception($"Client {clientGuid} is already connected.");
        }

        _lastHeartbeats.TryAdd(clientGuid, DateTime.UtcNow);

        _logger.LogInfo($"Client {clientGuid} connected.");

        try
        {
            // Continuously listen for data from the client
            var buffer = new byte[1024];
            while (!cancellationToken.IsCancellationRequested)
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                if (bytesRead > 0)
                {
                    // Update the last activity timestamp
                    _lastHeartbeats[clientGuid] = DateTime.UtcNow;

                    // Process the received data
                    var receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Raise the event to notify subscribers
                    ClientDataReceived?.Invoke(clientGuid, receivedData);

                    _logger.LogDebug($"Received data from client {clientGuid}: {receivedData}");
                }
                else
                {
                    // Client disconnected gracefully
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while handling client {clientGuid}: {ex.Message}");
        }
        finally
        {
            // Clean up resources when the client disconnects or an error occurs
            _clients.TryRemove(clientGuid, out _);
            _lastHeartbeats.TryRemove(clientGuid, out _);
            _streams.TryRemove(clientGuid, out _);
            client.Close();
            Console.WriteLine($"Client {clientGuid} disconnected.");
        }
    }

    /// <summary>
    /// Broadcasts a message to all connected clients.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task BroadcastAsync<T>(T message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        foreach (var stream in _streams.Values)
        {
            await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
        }
    }

    /// <summary>
    /// Stops the server.
    /// </summary>
    public void Stop()
    {
        _tcpListener.Stop();
    }
}
