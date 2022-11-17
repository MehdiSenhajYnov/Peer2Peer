using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    class Peer
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Peer ...");
            TcpClientPeer.Start();
            while (true)
            {
                
            }
        } 
    }
}