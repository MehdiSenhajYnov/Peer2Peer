using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TcpPeer2Peer
{
    public class SocketPeer {
        public static string _ipAddress = String.Empty;
        public static IPEndPoint? ipLocalEndPoint;
        public static Socket? client;
        //public static TcpListener? listener;
        public static IPEndPoint? peerEndPoint;
        
        public static int MyPort;
        public static int EndPort;

        public static void Start()
        {
            MyPort = Int32.Parse(File.ReadAllText("myport.txt"));
            EndPort = Int32.Parse(File.ReadAllText("endport.txt"));
            

            _ipAddress = File.ReadAllText("ip.txt");

            ipLocalEndPoint = new IPEndPoint(IPAddress.Any, MyPort);

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), EndPort);


            Console.WriteLine("Starting Peer ...");
            HolePunching();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            Console.WriteLine("Client Accepted");
        }

        public static void OnClientConnect(IAsyncResult ar)
        {
            Console.WriteLine("Client connected");
        }

        public static async void HolePunching()
        {

            if (peerEndPoint == null)
            {
                peerEndPoint = new IPEndPoint(IPAddress.Parse(_ipAddress), EndPort);
            }
            
            Console.WriteLine("Trying to connect to: " + _ipAddress + " on port: " + EndPort);
            
            if (!client.Connected) {
                try 
                {
                    client.Bind(peerEndPoint);
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
                Thread.Sleep(350);
                HolePunching();
            }
            //
        }
    }
}