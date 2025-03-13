using CoCoL;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SlimDHT.Test
{
    public class PeerTests
    {
        [Fact]
        public async Task RunPeer_ShouldStartAndRespondToDiscoveryRequests()
        {
            // Arrange
            var selfinfo = Utils.NewPeerInfo();
            var initialContactlist = Array.Empty<EndPoint>();
            var requests = Channel.Create<PeerRequest>();

            using var cts = new CancellationTokenSource();

            // Act
            var peerTask = Peer.RunPeer(selfinfo, 3, 100, TimeSpan.FromMinutes(10), initialContactlist, requests, cts.Token);

            // Attendre un court instant pour être sûr que le serveur est opérationnel
            await Task.Delay(500);

            // Le client effectue une demande de découverte
            var discoveredEndpoint = await DiscoveryService.DiscoverNodeAsync();

            Assert.NotNull(discoveredEndpoint);
            // On vérifie que l'endpoint découvert correspond bien à celui du serveur
            Assert.Equal(selfinfo.Address, discoveredEndpoint);

            // Cleanup
            cts.Cancel();
            await requests.RetireAsync();
            await peerTask;
        }
    }
}
