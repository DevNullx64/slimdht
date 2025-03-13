using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlimDHT.Test
{
    class Utils
    {
        static int port = 15000;
        public static PeerInfo NewPeerInfo()
        {
            return new PeerInfo(
                Key.CreateRandomKey(),
                new IPEndPoint(IPAddress.Loopback, port++));
        }

        public static CancellationTokenSource RunPeer(PeerInfo peerInfo = null)
        {
            peerInfo ??= NewPeerInfo();
            CancellationTokenSource ctx = new();
            EndPoint[] initialContactlist = [];
            var s = Peer.RunPeer(
                peerInfo, 5, 100, TimeSpan.FromDays(1),
                initialContactlist,
                null,
                ctx.Token
            );
            return ctx;
        }
    }
}
