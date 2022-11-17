using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    public class TcpClientPeer {
        public static string _ipAddress = String.Empty;
        public static IPEndPoint? ipLocalEndPoint;
        public static TcpClient? client;
        //public static TcpListener? listener;
        public static IPEndPoint? peerEndPoint;
        public const int myPort = 8888;
        public const int EndPort = 8080; 

        public static void Start()
        {
            // my public ip (portable pc) = "77.205.68.255"
            // other side public ip (home pc) = "176.150.133.69"
            _ipAddress = File.ReadAllText("ip.txt");

            client = new TcpClient();



            peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), EndPort);

            Console.WriteLine("Starting Peer ...");
            HolePunching();
        }

        public void ConnectToMainServer()
        {
            client.Connect(IPAddress.Parse("127.0.0.1"), EndPort);
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
                    client.Connect(peerEndPoint);
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
                HolePunching();
            }
            //
        }
    }
}