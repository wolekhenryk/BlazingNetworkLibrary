using System.Net.Sockets;
using System.Net;
using System.Text;

using var client = new TcpClient();

try {
    await client.ConnectAsync(IPAddress.Loopback, 5000);

    var stream = client.GetStream();

    // Write a guid to the server
    var guid = Guid.NewGuid().ToByteArray();
    await stream.WriteAsync(guid, 0, guid.Length);

    while (true) {
        // Each second write a message to the server
        await Task.Delay(1000);
        var message = "Hello, server!";

        var messageBytes = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
    }
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.InnerException?.Message);
}
