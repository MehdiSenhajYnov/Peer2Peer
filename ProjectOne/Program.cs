using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    class Peer
    {
        public static string _ipAddress = String.Empty;
        public static IPEndPoint? ipLocalEndPoint;
        public static TcpClient? client;
        public static IPEndPoint peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);

        public static void Main(string[] args)
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            _ipAddress = File.ReadAllText("ip.txt");

            ipLocalEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);
            client = new TcpClient(ipLocalEndPoint);

            Console.WriteLine("Starting Peer ...");
            HolePunching();
        }

        public static void HolePunching()
        {
            if (client == null)
            {
                ipLocalEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);
                client = new TcpClient(ipLocalEndPoint);
            }


            client.Connect(new IPEndPoint(IPAddress.Parse(_ipAddress), 7777));
            Console.WriteLine("Trying to connect to: " + _ipAddress);

            if (client.Connected)
            {
                Console.WriteLine("Connected");
                while (true)
                {
                    
                }
            } else
            {
                HolePunching();
            }
        }
    }
}