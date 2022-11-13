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
        public static Socket? client;
        public static IPEndPoint? peerEndPoint;

        public static void Main(string[] args)
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            _ipAddress = File.ReadAllText("ip.txt");

            IPAddress myipAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
            ipLocalEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Bind(ipLocalEndPoint);
            peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);

            Console.WriteLine("Starting Peer ...");
            HolePunching();
        }

        public async static void HolePunching()
        {

            if (peerEndPoint == null)
            {
                peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);
            }

            Console.WriteLine("Trying to connect to: " + _ipAddress);
            
            try 
            {
                await client.ConnectAsync(peerEndPoint);
                Console.WriteLine("Connected to peer");
            }
            catch (Exception ex)
            {
                
            }


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