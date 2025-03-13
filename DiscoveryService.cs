using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SlimDHT
{
    public static class DiscoveryService
    {
        private const int DiscoveryPort = 8888;
        private const string DiscoveryMessage = "DISCOVER_SLIMDHT";

        public static async Task StartListeningAsync(PeerInfo selfinfo)
        {
            using (var udpClient = new UdpClient(DiscoveryPort))
            {
                while (true)
                {
                    var result = await udpClient.ReceiveAsync();
                    var message = Encoding.UTF8.GetString(result.Buffer);

                    if (message == DiscoveryMessage)
                    {
                        var responseMessage = $"SLIMDHT_NODE:{selfinfo.Address}";
                        var responseBytes = Encoding.UTF8.GetBytes(responseMessage);
                        await udpClient.SendAsync(responseBytes, responseBytes.Length, result.RemoteEndPoint);
                    }
                }
            }
        }

        public static async Task<EndPoint> DiscoverNodeAsync()
        {
            using (var udpClient = new UdpClient())
            {
                udpClient.EnableBroadcast = true;
                var requestBytes = Encoding.UTF8.GetBytes(DiscoveryMessage);
                await udpClient.SendAsync(requestBytes, requestBytes.Length, new IPEndPoint(IPAddress.Broadcast, DiscoveryPort));

                var result = await udpClient.ReceiveAsync();
                var message = Encoding.UTF8.GetString(result.Buffer);

                if (message.StartsWith("SLIMDHT_NODE:"))
                {
                    var address = message.Substring("SLIMDHT_NODE:".Length);
                    return IPEndPoint.Parse(address);
                }

                return null;
            }
        }
    }
}
