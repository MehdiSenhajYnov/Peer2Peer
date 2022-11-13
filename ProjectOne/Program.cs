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
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("176.150.133.69"), 7777));

            while (true)
            {
                if (socket.Connected)
                {
                    Console.WriteLine("Connected");
                }
            }
        }
    }
}