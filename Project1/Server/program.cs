using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2PeerServer
{
    class Peer
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            server.SetupServer();
            while (true)
            {
                
            }
        } 
    }
}