using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlimDHT
{
    public static class DiscoveryService
    {
        private const int DiscoveryPort = 4242;
        private const string DiscoveryMessage = "DISCOVER_SLIMDHT";
        private const string SLIMDHT_NODE = "SLIMDHT_NODE:";

        public static async Task StartListeningAsync(PeerInfo selfinfo, CancellationToken cancellationToken)
        {
            using var udpClient = new UdpClient(DiscoveryPort);
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await udpClient.ReceiveAsync(cancellationToken);
                var message = Encoding.UTF8.GetString(result.Buffer);

                if (message == DiscoveryMessage)
                {
                    var responseMessage = $"{SLIMDHT_NODE}{selfinfo.Address}";
                    var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                    await udpClient.SendAsync(responseBytes, responseBytes.Length, result.RemoteEndPoint);
                }
            }
        }

        public static async Task<EndPoint> DiscoverNodeAsync()
        {
            using UdpClient udpClient = new();
            udpClient.EnableBroadcast = true;
            var requestBytes = Encoding.UTF8.GetBytes(DiscoveryMessage);
            await udpClient.SendAsync(requestBytes, requestBytes.Length, new IPEndPoint(IPAddress.Broadcast, DiscoveryPort));

            var result = await udpClient.ReceiveAsync();
            var message = Encoding.UTF8.GetString(result.Buffer);

            if (message.StartsWith(SLIMDHT_NODE))
            {
                var address = message[SLIMDHT_NODE.Length..];
                return IPEndPoint.Parse(address);
            }

            return null;
        }
    }
}
