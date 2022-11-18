using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting ...");
            TcpClientPeer.Start();
            while (true)
            {
                
            }
        } 
    }
}