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
        //public static TcpListener? listener;
        public static IPEndPoint? peerEndPoint;

        public static void Main(string[] args)
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            _ipAddress = File.ReadAllText("ip.txt");

            ipLocalEndPoint = new IPEndPoint(IPAddress.Any, 7777);

            client = new TcpClient(ipLocalEndPoint);
            /*
            listener = new TcpListener(ipLocalEndPoint);

            listener.Start();
            listener.BeginAcceptTcpClient(OnClientConnect, null);
            */
            peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);

            Console.WriteLine("Starting Peer ...");
            HolePunching();
        }

        public static void OnClientConnect(IAsyncResult ar)
        {
            Console.WriteLine("Client connected");
        }

        public static void HolePunching()
        {

            if (peerEndPoint == null)
            {
                peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), 7777);
            }
            
            Console.WriteLine("Trying to connect to: " + _ipAddress);
            
            if (!client.Connected) {
                try 
                {
                    client.ConnectAsync(peerEndPoint);
                    Console.WriteLine("Connected to peer");
                }
                catch (Exception e)
                {
                }
            } else {
                Console.WriteLine("Already connected");
            }

            if (client.Connected)
            {
                Console.WriteLine("Connected");
                while (true)
                {
                    
                }
            } else
            {
                System.Threading.Thread.Sleep(250);
                HolePunching();
            }
            //
        }
    }
}