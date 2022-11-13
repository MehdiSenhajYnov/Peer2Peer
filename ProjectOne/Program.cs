using System;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    class Peer
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Peer ...");
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            TcpClient client = new TcpClient();

            client.Connect(new IPEndPoint(IPAddress.Parse("77.205.68.255"), 7777));
            
            while (true)
            {
                if (client.Connected)
                {
                    Console.WriteLine("Connected");
                }
            }
        }
    }
}