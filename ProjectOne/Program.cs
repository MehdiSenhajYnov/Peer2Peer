using System;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    class Peer
    {
        public static void Main(string[] args)
        {
           
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("77.205.68.255"), 7777));

            while (true)
            {
                if (socket.Connected) {
                    Console.WriteLine("Connected");
                }
            }
        }
    }
}